using Postgrest.Attributes;
using Postgrest.Models;
using SQLite;
using System;

namespace InovaLog.Models
{
    [SQLite.Table("planos_manutencao")]
    [Postgrest.Attributes.Table("planos_manutencao")]

    public class PlanoManutencao : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public string Id { get; set; }

        [SQLite.Column("veiculo_id")]
        [Postgrest.Attributes.Column("veiculo_id")]
        public string VeiculoId { get; set; }

        [SQLite.Column("titulo")]
        [Postgrest.Attributes.Column("titulo")]
        public string Titulo { get; set; }

        [SQLite.Column("intervalo_km")]
        [Postgrest.Attributes.Column("intervalo_km")]
        public decimal? IntervaloKm { get; set; }

        [SQLite.Column("intervalo_dias")]
        [Postgrest.Attributes.Column("intervalo_dias")]
        public int? IntervaloDias { get; set; }

        [SQLite.Column("alerta_antecipado_km")]
        [Postgrest.Attributes.Column("alerta_antecipado_km")]
        public decimal? AlertaAntecipadoKm { get; set; }

        [SQLite.Column("ativo")]
        [Postgrest.Attributes.Column("ativo")]
        public bool? Ativo { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}
