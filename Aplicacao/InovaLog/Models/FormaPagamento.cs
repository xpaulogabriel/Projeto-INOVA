using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("formas_pagamento")]
    [Postgrest.Attributes.Table("formas_pagamento")]
    public class FormaPagamento : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", true)]
        public int Id { get; set; }

        [SQLite.Column("nome"), SQLite.Unique]
        [Postgrest.Attributes.Column("nome")]
        public string Nome { get; set; }

        [SQLite.Column("ativo")]
        [Postgrest.Attributes.Column("ativo")]
        public bool? Ativo { get; set; }
    }
}