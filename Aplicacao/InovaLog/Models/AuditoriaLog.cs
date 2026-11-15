using System;
using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("auditoria_logs")]
    [Postgrest.Attributes.Table("auditoria_logs")]
    public class AuditoriaLog : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", false)]
        public string Id { get; set; }

        [SQLite.Column("usuario_id")]
        [Postgrest.Attributes.Column("usuario_id")]
        public string UsuarioId { get; set; }

        [SQLite.Column("tabela_afetada")]
        [Postgrest.Attributes.Column("tabela_afetada")]
        public string TabelaAfetada { get; set; }

        [SQLite.Column("registro_id")]
        [Postgrest.Attributes.Column("registro_id")]
        public string RegistroId { get; set; }

        [SQLite.Column("acao")]
        [Postgrest.Attributes.Column("acao")]
        public string Acao { get; set; }

        [SQLite.Column("dados_antigos")]
        [Postgrest.Attributes.Column("dados_antigos")]
        public string DadosAntigos { get; set; }

        [SQLite.Column("dados_novos")]
        [Postgrest.Attributes.Column("dados_novos")]
        public string DadosNovos { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}