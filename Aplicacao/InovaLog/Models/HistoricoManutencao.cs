using System;
using System.Collections.Generic;
using System.Text;

namespace InovaLog.Models
{
    [SQLite.Table("historico_manutencoes")]
    [Postgrest.Attributes.Table("historico_manutencoes")]
    public class HistoricoManutencao : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public string Id { get; set; }

        [SQLite.Column("veiculo_id")]
        [Postgrest.Attributes.Column("veiculo_id")]
        public string VeiculoId { get; set; }

        [SQLite.Column("plano_manutencao_id")]
        [Postgrest.Attributes.Column("plano_manutencao_id")]
        public string PlanoManutencaoId { get; set; }

        [SQLite.Column("tipo_manutencao")]
        [Postgrest.Attributes.Column("tipo_manutencao")]
        public string TipoManutencao { get; set; }

        [SQLite.Column("descricao_servico")]
        [Postgrest.Attributes.Column("descricao_servico")]
        public string DescricaoServico { get; set; }

        [SQLite.Column("km_realizado")]
        [Postgrest.Attributes.Column("km_realizado")]
        public decimal KmRealizado { get; set; }

        [SQLite.Column("data_realizada")]
        [Postgrest.Attributes.Column("data_realizada")]
        public DateTime DataRealizada { get; set; }

        [SQLite.Column("oficina_fornecedor")]
        [Postgrest.Attributes.Column("oficina_fornecedor")]
        public string OficinaFornecedor { get; set; }

        [SQLite.Column("custo_total")]
        [Postgrest.Attributes.Column("custo_total")]
        public decimal CustoTotal { get; set; }

        [SQLite.Column("comprovante_url")]
        [Postgrest.Attributes.Column("comprovante_url")]
        public string ComprovanteUrl { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}
