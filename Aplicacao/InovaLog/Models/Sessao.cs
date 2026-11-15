using Postgrest.Attributes;
using Postgrest.Models;
using SQLite;
using System;

namespace InovaLog.Models
{
    [SQLite.Table("sessoes")]
    [Postgrest.Attributes.Table("sessoes")]

    public class Sessao : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public string Id { get; set; }

        [SQLite.Column("usuario_id")]
        [Postgrest.Attributes.Column("usuario_id")]
        public string UsuarioId { get; set; }

        [SQLite.Column("refresh_token")]
        [Postgrest.Attributes.Column("refresh_token")]
        public string RefreshToken { get; set; }

        [SQLite.Column("expira_em")]
        [Postgrest.Attributes.Column("expira_em")]
        public DateTime ExpiraEm { get; set; }

        [SQLite.Column("revogado")]
        [Postgrest.Attributes.Column("revogado")]
        public bool? Revogado { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}
