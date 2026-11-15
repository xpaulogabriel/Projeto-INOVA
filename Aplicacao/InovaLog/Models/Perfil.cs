using System;
using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("perfis")]
    [Postgrest.Attributes.Table("perfis")]
    public class Perfil : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", true)]
        public int Id { get; set; }

        [SQLite.Column("nome"), SQLite.Unique]
        [Postgrest.Attributes.Column("nome")]
        public string Nome { get; set; }

        [SQLite.Column("descricao")]
        [Postgrest.Attributes.Column("descricao")]
        public string Descricao { get; set; }

        [SQLite.Column("criado_em")]
        [Postgrest.Attributes.Column("criado_em")]
        public DateTime? CriadoEm { get; set; }
    }
}