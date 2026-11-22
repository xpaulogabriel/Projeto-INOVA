using SQLite;
using System.Text.Json.Serialization;

namespace InovaLog.Models
{
    public abstract class BaseModel : Postgrest.Models.BaseModel
    {
        [SQLite.Column("sincronizado")]
        [JsonIgnore]
        public bool Sincronizado { get; set; } = true;

        [SQLite.Column("precisa_atualizar")]
        [JsonIgnore]
        public bool PrecisaAtualizar { get; set; } = false;
    }
}