using System;
using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("dividas")]
    [Postgrest.Attributes.Table("dividas")]
    public class Divida : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", false)]
        public string Id { get; set; }

        [SQLite.Column("categoria_id")]
        [Postgrest.Attributes.Column("categoria_id")]
        public int CategoriaId { get; set; }

        [SQLite.Column("descricao")]
        [Postgrest.Attributes.Column("descricao")]
        public string Descricao { get; set; }

        [SQLite.Column("valor_total")]
        [Postgrest.Attributes.Column("valor_total")]
        public decimal ValorTotal { get; set; }

        [SQLite.Column("quantidade_parcelas")]
        [Postgrest.Attributes.Column("quantidade_parcelas")]
        public int QuantidadeParcelas { get; set; }

        [SQLite.Column("valor_parcela")]
        [Postgrest.Attributes.Column("valor_parcela")]
        public decimal ValorParcela { get; set; }

        [SQLite.Column("data_inicio_vencimento")]
        [Postgrest.Attributes.Column("data_inicio_vencimento")]
        public DateTime DataInicioVencimento { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}