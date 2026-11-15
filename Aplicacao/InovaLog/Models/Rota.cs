using Postgrest.Attributes;
using Postgrest.Models;
using SQLite;
using System;

namespace InovaLog.Models
{
    [SQLite.Table("rotas")]
    [Postgrest.Attributes.Table("rotas")]

    public class Rota : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public string Id { get; set; }

        [SQLite.Column("cidade_origem")]
        [Postgrest.Attributes.Column("cidade_origem")]
        public string CidadeOrigem { get; set; }

        [SQLite.Column("cidade_destino")]
        [Postgrest.Attributes.Column("cidade_destino")]
        public string CidadeDestino { get; set; }

        [SQLite.Column("distancia_estimada_km")]
        [Postgrest.Attributes.Column("distancia_estimada_km")]
        public decimal DistanciaEstimadaKm { get; set; }

        [SQLite.Column("ativo")]
        [Postgrest.Attributes.Column("ativo")]
        public bool? Ativo { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}
