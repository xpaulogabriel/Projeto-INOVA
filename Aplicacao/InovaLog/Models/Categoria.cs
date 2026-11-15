using Postgrest.Attributes;
using Postgrest.Models;
using SQLite;
using System;

namespace InovaLog.Models
{
    [SQLite.Table("categorias")]
    [Postgrest.Attributes.Table("categorias")]

    public class Categoria : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public int Id { get; set; }

        [SQLite.Column("nome"), Unique]
        [Postgrest.Attributes.Column("nome")]
        public string Nome { get; set; }

        [SQLite.Column("tipo")]
        [Postgrest.Attributes.Column("tipo")]
        public string Tipo { get; set; }

        [SQLite.Column("ativo")]
        [Postgrest.Attributes.Column("ativo")]
        public bool? Ativo { get; set; }
    }
}
