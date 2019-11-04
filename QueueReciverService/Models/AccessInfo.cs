using Newtonsoft.Json;

namespace QueueReciverService.Models
{
    public class AccessInfo
    {
        [JsonProperty("userOid")]
        public string UserOid { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("updatedByUserOid")]
        public string UpdatedByUserOid { get; set; }

        [JsonProperty("plantId")]
        public string PlantId { get; set; }

        [JsonProperty("hasAccess")]
        public bool HasAccess { get; set; }
    }
}
