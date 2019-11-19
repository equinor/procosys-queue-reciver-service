using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public class GraphService : IGraphService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GraphService> _logger;
        public GraphService(IConfiguration config, ILogger<GraphService> logger)
        {
            _config = config;
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

            if(user == null)
            {
                _logger.LogError($"User with oid {userOid} was not found when querying graph api. returning null");
                return null;
            }

            var adPerson = new AdPerson(user.Id, user.UserPrincipalName?.ToUpper(), user.Mail);

            return adPerson;
        }

        private async Task<AuthenticationResult> GetAccessToken()
        {
            var authority = _config["AzureAd:Authority"];
            var graphUrl = _config["GraphUrl"];
            var clientId = _config["AzureAd:ClientId"];
            var clientSecret = _config["Azure:ClientSecret"];
            var authContext = new AuthenticationContext(authority);

            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
            return await authContext.AcquireTokenAsync(graphUrl, clientCred);
        }
    }
}
