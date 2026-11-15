using System;
using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("viagens")]
    [Postgrest.Attributes.Table("viagens")]
    public class Viagem : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", false)]
        public string Id { get; set; }

        [SQLite.Column("veiculo_id")]
        [Postgrest.Attributes.Column("veiculo_id")]
        public string VeiculoId { get; set; }

        [SQLite.Column("rota_id")]
        [Postgrest.Attributes.Column("rota_id")]
        public string RotaId { get; set; }

        [SQLite.Column("motorista_id")]
        [Postgrest.Attributes.Column("motorista_id")]
        public string MotoristaId { get; set; }

        [SQLite.Column("viagem_origem_id")]
        [Postgrest.Attributes.Column("viagem_origem_id")]
        public string ViagemOrigemId { get; set; }

        [SQLite.Column("distancia_estimada_km")]
        [Postgrest.Attributes.Column("distancia_estimada_km")]
        public decimal DistanciaEstimadaKm { get; set; }

        [SQLite.Column("hodometro_inicial")]
        [Postgrest.Attributes.Column("hodometro_inicial")]
        public decimal HodometroInicial { get; set; }

        [SQLite.Column("hodometro_final")]
        [Postgrest.Attributes.Column("hodometro_final")]
        public decimal? HodometroFinal { get; set; }

        [SQLite.Column("distancia_real_km")]
        [Postgrest.Attributes.Column("distancia_real_km")]
        public decimal? DistanciaRealKm { get; set; }

        [SQLite.Column("desvio_km")]
        [Postgrest.Attributes.Column("desvio_km")]
        public decimal? DesvioKm { get; set; }

        [SQLite.Column("data_inicio")]
        [Postgrest.Attributes.Column("data_inicio")]
        public DateTime? DataInicio { get; set; }

        [SQLite.Column("data_fim")]
        [Postgrest.Attributes.Column("data_fim")]
        public DateTime? DataFim { get; set; }

        [SQLite.Column("status")]
        [Postgrest.Attributes.Column("status")]
        public string Status { get; set; }

        [SQLite.Column("tipo_viagem")]
        [Postgrest.Attributes.Column("tipo_viagem")]
        public string TipoViagem { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}