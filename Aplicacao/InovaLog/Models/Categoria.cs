using SQLite;
using Postgrest.Attributes;

namespace InovaLog.Models
{
    [SQLite.Table("categorias")]
    [Postgrest.Attributes.Table("categorias")]
    public class Categoria : BaseModel
    {
        [SQLite.PrimaryKey, SQLite.Column("id")]
        [Postgrest.Attributes.PrimaryKey("id", true)]
        public int Id { get; set; }

        [SQLite.Column("nome"), SQLite.Unique]
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