using System;
using System.Text.Json.Serialization;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [Table("motoristas")]
    [SQLite.Table("motoristas")]
    public class Motorista : BaseModel
    {
        [PrimaryKey("id", false)]
        [SQLite.PrimaryKey]
        [SQLite.Column("id")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("usuario_id")]
        [SQLite.Column("usuario_id")]
        [JsonPropertyName("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Column("cnh_numero")]
        [SQLite.Column("cnh_numero")]
        [JsonPropertyName("cnh_numero")]
        public string CnhNumero { get; set; } = string.Empty;

        [Column("cnh_categoria")]
        [SQLite.Column("cnh_categoria")]
        [JsonPropertyName("cnh_categoria")]
        public string CnhCategoria { get; set; } = "D";

        [Column("cnh_validade")]
        [SQLite.Column("cnh_validade")]
        [JsonPropertyName("cnh_validade")]
        public DateTime CnhValidade { get; set; }

        [Column("ear")]
        [SQLite.Column("ear")]
        [JsonPropertyName("ear")]
        public bool Ear { get; set; } = true;

        [Column("validade_toxicologico")]
        [SQLite.Column("validade_toxicologico")]
        [JsonPropertyName("validade_toxicologico")]
        public DateTime? ValidadeToxicologico { get; set; }

        [Column("status_operacional")]
        [SQLite.Column("status_operacional")]
        [JsonPropertyName("status_operacional")]
        public string StatusOperacional { get; set; } = "Disponível";

        [Column("data_criacao")]
        [SQLite.Column("data_criacao")]
        [JsonPropertyName("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [Column("data_modificacao")]
        [SQLite.Column("data_modificacao")]
        [JsonPropertyName("data_modificacao")]
        public DateTime DataModificacao { get; set; } = DateTime.UtcNow;
    }
}