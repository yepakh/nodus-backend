using Newtonsoft.Json;

namespace Nodus.Database.Migrator
{
    [JsonObject("ConnectionStrings")]
    internal class ConnectionStringModel
    {
        [JsonProperty("AdminDatabase")]
        public string AdminDatabase { get; set; }
    }
}
