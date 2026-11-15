using Postgrest.Attributes;
using Postgrest.Models;
using SQLite;
using System;

namespace InovaLog.Models
{
    public abstract class BaseModel
    {
        // Salvo no SQLite, mas ignorado pelo Supabase na hora de enviar
        [SQLite.Column("sincronizado")]
        [Postgrest.Attributes.Column(ignore: true)]
        public bool Sincronizado { get; set; } = true;

        [SQLite.Column("precisa_atualizar")]
        [Postgrest.Attributes.Column(ignore: true)]
        public bool PrecisaAtualizar { get; set; } = false;
    }
}