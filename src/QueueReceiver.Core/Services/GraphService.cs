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
        private readonly ILogger<GraphService> log;

        public GraphService(GraphSettings graphSettings, ILogger<GraphService> logger)
        {
            settings = graphSettings;
            log = logger;
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

            log.LogInformation($"Queuering microsoft graph for user with oid {userOid}");
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
            var graphUrl = settings.GraphUrl;
            var clientId = settings.ClientId;
            var clientSecret = settings.ClientSecret;
            var authContext = new AuthenticationContext(authority);

            var clientCred = new ClientCredential(clientId, clientSecret);
            return await authContext.AcquireTokenAsync(graphUrl, clientCred);
        }
    }
}
