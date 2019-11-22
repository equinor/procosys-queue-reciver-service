using Newtonsoft.Json;
using System.Collections.Generic;

namespace QueueReceiver.Core.Models
{
    public class AccessInfo
    {
        public AccessInfo(string plantOid, List<Member> members)
        {
            PlantOid = plantOid;
            Members = members;
        }

        [JsonProperty("groupId")]
        public string PlantOid { get; set; }

        [JsonProperty("members")]
        public List<Member> Members { get; set; }
    }

    public class Member
    {
        public Member(string userOid, bool shouldRemove)
        {
            UserOid = userOid;
            ShouldRemove = shouldRemove;
        }

        [JsonProperty("id")]
        public string UserOid { get; set; }

        [JsonProperty("remove")]
        public bool ShouldRemove { get; set; }
    }
}
