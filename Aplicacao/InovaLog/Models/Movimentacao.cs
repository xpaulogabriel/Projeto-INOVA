using System;
using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("movimentacoes")]
    [Postgrest.Attributes.Table("movimentacoes")]

    public class Movimentacao : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public string Id { get; set; }

        [SQLite.Column("categoria_id")]
        [Postgrest.Attributes.Column("categoria_id")]
        public int CategoriaId { get; set; }

        [SQLite.Column("forma_pagamento_id")]
        [Postgrest.Attributes.Column("forma_pagamento_id")]
        public int? FormaPagamentoId { get; set; }

        [SQLite.Column("usuario_lancamento_id")]
        [Postgrest.Attributes.Column("usuario_lancamento_id")]
        public string UsuarioLancamentoId { get; set; }

        [SQLite.Column("viagem_id")]
        [Postgrest.Attributes.Column("viagem_id")]
        public string ViagemId { get; set; }

        [SQLite.Column("divida_id")]
        [Postgrest.Attributes.Column("divida_id")]
        public string DividaId { get; set; }

        [SQLite.Column("numero_parcela")]
        [Postgrest.Attributes.Column("numero_parcela")]
        public int? NumeroParcela { get; set; }

        [SQLite.Column("descricao")]
        [Postgrest.Attributes.Column("descricao")]
        public string Descricao { get; set; }

        [SQLite.Column("valor")]
        [Postgrest.Attributes.Column("valor")]
        public decimal Valor { get; set; }

        [SQLite.Column("quantidade_litros")]
        [Postgrest.Attributes.Column("quantidade_litros")]
        public decimal? QuantidadeLitros { get; set; }

        [SQLite.Column("preco_litro")]
        [Postgrest.Attributes.Column("preco_litro")]
        public decimal? PrecoLitro { get; set; }

        [SQLite.Column("comprovante_url")]
        [Postgrest.Attributes.Column("comprovante_url")]
        public string ComprovanteUrl { get; set; }

        [SQLite.Column("status")]
        [Postgrest.Attributes.Column("status")]
        public string Status { get; set; }

        [SQLite.Column("data_vencimento")]
        [Postgrest.Attributes.Column("data_vencimento")]
        public DateTime DataVencimento { get; set; }

        [SQLite.Column("data_pagamento")]
        [Postgrest.Attributes.Column("data_pagamento")]
        public DateTime? DataPagamento { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}
