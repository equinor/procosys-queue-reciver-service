using System;

namespace QueueReceiver.Core.Settings
{
#nullable disable
    public class GraphSettings
    {
        public Uri GraphUrl { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
