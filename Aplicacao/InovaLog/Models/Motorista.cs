using System;
using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("motoristas")]
    [Postgrest.Attributes.Table("motoristas")]
    public class Motorista : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", false)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [SQLite.Column("usuario_id"), SQLite.Unique]
        [Postgrest.Attributes.Column("usuario_id")]
        public string UsuarioId { get; set; } = string.Empty;

        [SQLite.Column("cnh_numero"), SQLite.Unique]
        [Postgrest.Attributes.Column("cnh_numero")]
        public string CnhNumero { get; set; } = string.Empty;

        [SQLite.Column("cnh_categoria")]
        [Postgrest.Attributes.Column("cnh_categoria")]
        public string CnhCategoria { get; set; } = "D";

        [SQLite.Column("cnh_validade")]
        [Postgrest.Attributes.Column("cnh_validade")]
        public DateTime? CnhValidade { get; set; }

        [SQLite.Column("ear")]
        [Postgrest.Attributes.Column("ear")]
        public bool? Ear { get; set; } = true;

        [SQLite.Column("validade_toxicologico")]
        [Postgrest.Attributes.Column("validade_toxicologico")]
        public DateTime? ValidadeToxicologico { get; set; }

        [SQLite.Column("status_operacional")]
        [Postgrest.Attributes.Column("status_operacional")]
        public string StatusOperacional { get; set; } = "Disponível";

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }

        [SQLite.Column("atualizado_em")]
        [Postgrest.Attributes.Column("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; }
    }
}