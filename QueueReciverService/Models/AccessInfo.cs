using Newtonsoft.Json;

namespace QueueReciverService.Models
{
    public class AccessInfo
    {
        [JsonProperty("groupId")]
        public string PlantOid { get; set; }

        [JsonProperty("userOid")]
        public string UserOid { get; set; }

        [JsonProperty("remove")]
        public bool ShouldRemove { get; set; }
    }
}
