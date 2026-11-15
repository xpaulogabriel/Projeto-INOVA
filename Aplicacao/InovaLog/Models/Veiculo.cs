using Postgrest.Attributes;
using Postgrest.Models;
using SQLite;
using System;

namespace InovaLog.Models
{
    [SQLite.Table("veiculos")]
    [Postgrest.Attributes.Table("veiculos")]
    public class Veiculo : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public string Id { get; set; }

        [SQLite.Column("placa"), Unique]
        [Postgrest.Attributes.Column("placa")]
        public string Placa { get; set; }

        [SQLite.Column("marca")]
        [Postgrest.Attributes.Column("marca")]
        public string Marca { get; set; }

        [SQLite.Column("modelo")]
        [Postgrest.Attributes.Column("modelo")]
        public string Modelo { get; set; }

        [SQLite.Column("capacidade_carga")]
        [Postgrest.Attributes.Column("capacidade_carga")]
        public decimal CapacidadeCarga { get; set; }

        [SQLite.Column("km_acumulado")]
        [Postgrest.Attributes.Column("km_acumulado")]
        public decimal? KmAcumulado { get; set; }

        [SQLite.Column("status")]
        [Postgrest.Attributes.Column("status")]
        public string Status { get; set; }

        [SQLite.Column("ativo")]
        [Postgrest.Attributes.Column("ativo")]
        public bool? Ativo { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}
