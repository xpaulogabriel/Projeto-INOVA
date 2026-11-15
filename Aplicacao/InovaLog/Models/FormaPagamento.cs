using Postgrest.Attributes;
using Postgrest.Models;
using SQLite;
using System;

namespace InovaLog.Models
{
    [SQLite.Table("formas_pagamento")]
    [Postgrest.Attributes.Table("formas_pagamento")]
    public class FormaPagamento : BaseModel
    {
        [PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.Column("id")]
        public int Id { get; set; }

        [SQLite.Column("nome"), Unique]
        [Postgrest.Attributes.Column("nome")]
        public string Nome { get; set; }

        [SQLite.Column("ativo")]
        [Postgrest.Attributes.Column("ativo")]
        public bool? Ativo { get; set; }
    }
}
