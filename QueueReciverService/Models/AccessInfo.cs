using Newtonsoft.Json;
using System.Collections.Generic;

namespace QueueReciverService.Models
{
    public class AccessInfo
    {
        [JsonProperty("groupId")]
        public string PlantOid { get; set; }

        [JsonProperty("members")]
        public List<Member> Members { get; set; }
    }

    public class Member
    {
        [JsonProperty("id")]
        public string UserOid { get; set; }

        [JsonProperty("remove")]
        public bool ShouldRemove { get; set; }

    }
}
