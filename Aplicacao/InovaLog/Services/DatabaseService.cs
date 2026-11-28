using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InovaLog.Models;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using SQLite;

namespace InovaLog.Services
{
    [SQLite.Table("registros_locais")]
    internal class RegistroLocal
    {
        [SQLite.PrimaryKey]
        public string Chave { get; set; } = string.Empty; // Formato: "NomeTabela:Id"

        [SQLite.Indexed]
        public string Tabela { get; set; } = string.Empty;

        public string RegistroId { get; set; } = string.Empty;
        public string JsonDados { get; set; } = string.Empty;
        public bool Sincronizado { get; set; } = true;
        public bool PrecisaAtualizar { get; set; } = false;
    }

    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "inovalog_local.db3");
        }

        private async Task InitAsync()
        {
            if (_database != null)
                return;

            await _initLock.WaitAsync();
            try
            {
                if (_database != null)
                    return;

                var flags = SQLiteOpenFlags.ReadWrite |
                            SQLiteOpenFlags.Create |
                            SQLiteOpenFlags.SharedCache;

                var conn = new SQLiteAsyncConnection(_dbPath, flags);
                await conn.CreateTableAsync<RegistroLocal>();

                _database = conn;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            await InitAsync();
            return _database;
        }

        // ==========================================
        // MÉTODOS GENÉRICOS (CRUD LOCAL)
        // ==========================================

        public async Task<List<T>> GetAllAsync<T>() where T : BaseModel, new()
        {
            await InitAsync();
            var tabela = typeof(T).Name;
            var linhas = await _database.Table<RegistroLocal>()
                .Where(x => x.Tabela == tabela)
                .ToListAsync();

            return linhas.Select(Deserializar<T>).Where(x => x != null).ToList()!;
        }

        public async Task<T> GetByIdAsync<T>(object id) where T : BaseModel, new()
        {
            await InitAsync();
            var chave = MontarChave<T>(id?.ToString());
            var linha = await _database.FindAsync<RegistroLocal>(chave);
            return linha == null ? null! : Deserializar<T>(linha)!;
        }

        public async Task<int> SaveAsync<T>(T item) where T : BaseModel, new()
        {
            await InitAsync();
            var registro = CriarRegistro(item);
            return await _database.InsertOrReplaceAsync(registro);
        }

        public async Task<int> SaveAllAsync<T>(IEnumerable<T> items) where T : BaseModel, new()
        {
            await InitAsync();
            int count = 0;
            await _database.RunInTransactionAsync(conn =>
            {
                foreach (var item in items)
                {
                    count += conn.InsertOrReplace(CriarRegistro(item));
                }
            });
            return count;
        }

        /// <summary>
        /// Substitui o cache local sincronizado pela lista atual da nuvem, removendo registros deletados no Supabase.
        /// </summary>
        public async Task ReplaceAllSyncedAsync<T>(IEnumerable<T> items) where T : BaseModel, new()
        {
            await InitAsync();
            var tabela = typeof(T).Name;

            await _database.RunInTransactionAsync(conn =>
            {
                conn.Execute(
                    "DELETE FROM registros_locais WHERE Tabela = ? AND Sincronizado = 1 AND PrecisaAtualizar = 0",
                    tabela);

                foreach (var item in items)
                {
                    item.Sincronizado = true;
                    item.PrecisaAtualizar = false;
                    conn.InsertOrReplace(CriarRegistro(item));
                }
            });
        }

        public async Task<int> DeleteAsync<T>(T item) where T : BaseModel, new()
        {
            await InitAsync();
            var id = ObterId(item);
            var chave = MontarChave<T>(id);
            return await _database.DeleteAsync<RegistroLocal>(chave);
        }

        // ==========================================
        // SUPORTE AO OFFLINE-FIRST
        // ==========================================

        public async Task<List<T>> GetPendentesSincronizacaoAsync<T>() where T : BaseModel, new()
        {
            await InitAsync();
            var tabela = typeof(T).Name;
            var linhas = await _database.Table<RegistroLocal>()
                .Where(x => x.Tabela == tabela && (!x.Sincronizado || x.PrecisaAtualizar))
                .ToListAsync();

            return linhas.Select(Deserializar<T>).Where(x => x != null).ToList()!;
        }

        // ==========================================
        // AUXILIARES
        // ==========================================

        private static RegistroLocal CriarRegistro<T>(T item) where T : BaseModel
        {
            var id = ObterId(item);
            return new RegistroLocal
            {
                Chave = MontarChave<T>(id),
                Tabela = typeof(T).Name,
                RegistroId = id,
                JsonDados = JsonConvert.SerializeObject(item),
                Sincronizado = item.Sincronizado,
                PrecisaAtualizar = item.PrecisaAtualizar
            };
        }

        private static T? Deserializar<T>(RegistroLocal linha) where T : BaseModel
        {
            var obj = JsonConvert.DeserializeObject<T>(linha.JsonDados);
            if (obj != null)
            {
                obj.Sincronizado = linha.Sincronizado;
                obj.PrecisaAtualizar = linha.PrecisaAtualizar;
            }
            return obj;
        }

        private static string ObterId<T>(T item)
        {
            var prop = typeof(T).GetProperty("Id");
            return prop?.GetValue(item)?.ToString() ?? Guid.NewGuid().ToString();
        }

        private static string MontarChave<T>(string? id) => $"{typeof(T).Name}:{id}";
    }
}