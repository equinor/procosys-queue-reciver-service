using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Settings;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class GraphService : IGraphService
    {
        private readonly GraphSettings settings;
        private readonly ILogger<GraphService> _logger;

        public GraphService(GraphSettings graphSettings, ILogger<GraphService> logger)
        {
            settings = graphSettings;
            _logger = logger;
        }

        public async Task<AdPerson> GetPersonByOid(string userOid)
        {
            AuthenticationResult auth = await GetAccessToken();

            var graphClient = new GraphServiceClient(
                   new DelegateAuthenticationProvider(
                      (requestMessage) =>
                      {
                          requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", auth.AccessToken);
                          return Task.FromResult(0);
                      }));

            var user = await graphClient.Users[userOid].Request().GetAsync();
            var adPerson = new AdPerson(user.Id, user.UserPrincipalName, user.Mail)
            {
                GivenName = user.GivenName,
                Surname = user.Surname
            };
            return adPerson;
        }

        private async Task<AuthenticationResult> GetAccessToken()
        {
            var authority = settings.Authority;
            //["AzureAd:Authority"];
            var graphUrl = settings.GraphUrl;//_config["GraphUrl"];
            var clientId = settings.ClientId;// _config["AzureAd:ClientId"];
            var clientSecret = settings.ClientSecret;// _config["Azure:ClientSecret"];
            var authContext = new AuthenticationContext(authority);

            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
            return await authContext.AcquireTokenAsync(graphUrl, clientCred);
        }
    }
}
