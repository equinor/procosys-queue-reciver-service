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
        public string PlantOid { get; set; } // TODO: Remove setter. Change to type Guid

        [JsonProperty("members")]
        public List<Member> Members { get; }
    }

    public class Member
    {
        public Member(string userOid, bool shouldRemove)
        {
            UserOid = userOid;
            ShouldRemove = shouldRemove;
        }

        [JsonProperty("id")]
        public string UserOid { get; set; }// TODO: Remove setter. Change to type Guid

        [JsonProperty("remove")]
        public bool ShouldRemove { get; set; }// TODO: Remove setter. Consider renaming to "RemoveAccess" or "Void" or something. Current name could indicate that the user should be deleted from the database.
    }
}
