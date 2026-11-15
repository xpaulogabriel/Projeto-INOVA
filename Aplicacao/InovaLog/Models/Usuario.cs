using System;
using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("usuarios")]
    [Postgrest.Attributes.Table("usuarios")]
    public class Usuario : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", false)]
        public string Id { get; set; }

        [SQLite.Column("google_id"), SQLite.Unique]
        [Postgrest.Attributes.Column("google_id")]
        public string GoogleId { get; set; }

        [SQLite.Column("nome")]
        [Postgrest.Attributes.Column("nome")]
        public string Nome { get; set; }

        [SQLite.Column("email"), SQLite.Unique]
        [Postgrest.Attributes.Column("email")]
        public string Email { get; set; }

        [SQLite.Column("foto_url")]
        [Postgrest.Attributes.Column("foto_url")]
        public string FotoUrl { get; set; }

        [SQLite.Column("perfil_id")]
        [Postgrest.Attributes.Column("perfil_id")]
        public int? PerfilId { get; set; }

        [SQLite.Column("ativo")]
        [Postgrest.Attributes.Column("ativo")]
        public bool? Ativo { get; set; }

        [SQLite.Column("ultimo_login")]
        [Postgrest.Attributes.Column("ultimo_login")]
        public DateTime? UltimoLogin { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }

        [SQLite.Column("atualizado_em")]
        [Postgrest.Attributes.Column("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; }
    }
}