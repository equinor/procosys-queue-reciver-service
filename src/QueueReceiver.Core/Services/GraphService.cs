using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Settings;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;

namespace QueueReceiver.Core.Services
{
    public class GraphService : IGraphService
    {
        private readonly GraphSettings _settings;
        private readonly ILogger<GraphService> _log;

        public GraphService(GraphSettings graphSettings, ILogger<GraphService> logger)
        {
            _settings = graphSettings;
            _log = logger;
        }

        public async Task<AdPerson?> GetPersonByOid(string userOid)
        {
            AuthenticationResult auth = await GetAccessToken();

            var graphClient = new GraphServiceClient( //TODO: Don't new up here, accept interface through DI to make it testable.
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization =
                            new AuthenticationHeaderValue("bearer", auth.AccessToken);
                        return Task.FromResult(0);
                    }));
            try
            {
                _log.LogInformation($"Queuering microsoft graph for user with oid {userOid}");
                var user = await graphClient.Users[userOid].Request().GetAsync();
                var adPerson = new AdPerson(user.Id, user.UserPrincipalName, user.Mail)
                {
                    GivenName = user.GivenName,
                    Surname = user.Surname
                };
                return adPerson;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }

            return null;
        }

        private async Task<AuthenticationResult> GetAccessToken()
        {
            var authority = _settings.Authority;
            var graphUrl = _settings.GraphUrl;
            var clientId = _settings.ClientId;
            var clientSecret = _settings.ClientSecret;
            var authContext = new AuthenticationContext(authority);

            var clientCred = new ClientCredential(clientId, clientSecret);
            return await authContext.AcquireTokenAsync(graphUrl.OriginalString, clientCred);
        }
    }
}