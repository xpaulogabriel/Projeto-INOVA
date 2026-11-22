using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Microsoft.Maui.Storage;
using InovaLog.Models;

namespace InovaLog.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "inovalog_local.db3");
        }

        private async Task InitAsync()
        {
            if (_database != null)
                return;

            var flags = SQLiteOpenFlags.ReadWrite |
                        SQLiteOpenFlags.Create |
                        SQLiteOpenFlags.SharedCache;

            _database = new SQLiteAsyncConnection(_dbPath, flags);

            // Criação de todas as tabelas baseadas nas suas Models
            await _database.CreateTableAsync<Perfil>();
            await _database.CreateTableAsync<Usuario>();
            await _database.CreateTableAsync<Sessao>();
            await _database.CreateTableAsync<Veiculo>();
            await _database.CreateTableAsync<Rota>();
            await _database.CreateTableAsync<Viagem>();
            await _database.CreateTableAsync<PlanoManutencao>();
            await _database.CreateTableAsync<HistoricoManutencao>();
            await _database.CreateTableAsync<Categoria>();
            await _database.CreateTableAsync<FormaPagamento>();
            await _database.CreateTableAsync<Divida>();
            await _database.CreateTableAsync<Movimentacao>();
            await _database.CreateTableAsync<AuditoriaLog>();
        }

        /// <summary>
        /// Retorna a conexão ativa para consultas LINQ personalizadas nos outros Services.
        /// </summary>
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
            return await _database.Table<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(object id) where T : BaseModel, new()
        {
            await InitAsync();
            return await _database.FindAsync<T>(id);
        }

        /// <summary>
        /// Insere ou atualiza um registro no banco local.
        /// </summary>
        public async Task<int> SaveAsync<T>(T item) where T : BaseModel, new()
        {
            await InitAsync();
            return await _database.InsertOrReplaceAsync(item);
        }

        public async Task<int> SaveAllAsync<T>(IEnumerable<T> items) where T : BaseModel, new()
        {
            await InitAsync();
            int count = 0;
            await _database.RunInTransactionAsync(conn =>
            {
                foreach (var item in items)
                {
                    count += conn.InsertOrReplace(item);
                }
            });
            return count;
        }

        public async Task<int> DeleteAsync<T>(T item) where T : BaseModel, new()
        {
            await InitAsync();
            return await _database.DeleteAsync(item);
        }

        // ==========================================
        // SUPORTE AO OFFLINE-FIRST
        // ==========================================

        /// <summary>
        /// Busca todos os registros de uma tabela que ainda não foram enviados ao Supabase.
        /// </summary>
        public async Task<List<T>> GetPendentesSincronizacaoAsync<T>() where T : BaseModel, new()
        {
            await InitAsync();
            return await _database.Table<T>()
                .Where(x => !x.Sincronizado || x.PrecisaAtualizar)
                .ToListAsync();
        }
    }
}