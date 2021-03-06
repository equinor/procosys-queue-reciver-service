﻿using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class GraphService : IGraphService
    {
        private readonly GraphSettings _settings;
        private readonly ILogger<GraphService> _log;

        private const string EquinorInternalAzureName = "StatoilSRM.onmicrosoft.com";

        public GraphService(GraphSettings graphSettings, ILogger<GraphService> logger)
        {
            _settings = graphSettings;
            _log = logger;
        }

        public async Task<IEnumerable<string>> GetMemberOidsAsync(string groupOid)
        {
            var graphClient = await CreateClient();

            var members = await graphClient.Groups[groupOid].Members
                .Request()
                .GetAsync();

            var result = members.Select(m => m.Id).ToList();

            while (members.NextPageRequest != null)
            {
                members = await members.NextPageRequest.GetAsync();
                result.AddRange(members.Select(m => m.Id));
            }

            return result;
        }

        public async Task<AdPerson?> GetAdPersonByOidAsync(string userOid)
        {
             var graphClient = await CreateClient();

            _log.LogInformation($"Querying microsoft graph for user with oid {userOid}");

            try
            {
                var user = await graphClient.Users[userOid].Request().GetAsync();

                // Username is e-mail by default.
                // Check that it does not contain the internal name (result of bad data in AD), use UPN in that case.
                var userNameFromGraph = !string.IsNullOrEmpty(user.Mail) && user.Mail.Contains(EquinorInternalAzureName)
                    ? user.UserPrincipalName
                    : user.Mail;

                var userName = userNameFromGraph;
                var email = userNameFromGraph;

                if (string.IsNullOrEmpty(userName))
                {
                    // set UserPrincipalName if e-mail is undefined
                    userName = user.UserPrincipalName;

                    if (user.UserPrincipalName.Contains("@"))
                    {
                        email = user.UserPrincipalName;
                    }
                }

                var adPerson = new AdPerson(user.Id, userName, email)
                {
                    GivenName = user.GivenName,
                    Surname = user.Surname,
                    MobileNumber = user.MobilePhone,
                    DisplayName = user.DisplayName
                };

                return adPerson;
            }
            catch (ServiceException)
            {
                _log.LogInformation($"User with oid {userOid} not found in user directory");
                return null;
            }
        }

        public async Task<bool> AdPersonFoundInDeletedDirectory(string userOid)
        {
            var graphClient = await CreateClient();
            try
            {
                var deletedItem = await graphClient.Directory.DeletedItems[userOid].Request().GetAsync();
                _log.LogInformation($"User with oid {userOid} found in deleted directory in AAD");
                return deletedItem != null;    
            }
            catch (ServiceException)
            {
                return false;
            }
        }

        private async Task<GraphServiceClient> CreateClient()
        {
            AuthenticationResult auth = await GetAccessToken();
            return new GraphServiceClient(
                   new DelegateAuthenticationProvider(
                      (requestMessage) =>
                      {
                          requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", auth.AccessToken);
                          return Task.FromResult(0);
                      }));
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