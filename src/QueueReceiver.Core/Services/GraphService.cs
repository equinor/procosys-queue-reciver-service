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

        public GraphService(GraphSettings graphSettings, ILogger<GraphService> logger)
        {
            _settings = graphSettings;
            _log = logger;
        }

        public async Task<IEnumerable<string>> GetMemberOids(string oid)
        {
            var graphClient = await CreateClient();

            var members = await graphClient.Groups[oid].Members
                .Request()
                .GetAsync();
            var result = members.Select(m => m.Id).ToList();

            while (members.NextPageRequest != null)
            {
                members = await members.NextPageRequest.GetAsync();
                result.AddRange(members.Select(m => m.Id));
            }

            return result;
          

            //TODO itterate all pages
            //return result;
        }

        public async Task<AdPerson> GetPersonByOid(string userOid)
        {
            var graphClient = await CreateClient();

            _log.LogInformation($"Queuering microsoft graph for user with oid {userOid}");
            var user = await graphClient.Users[userOid].Request().GetAsync();
            var adPerson = new AdPerson(user.Id, user.UserPrincipalName, user.Mail)
            {
                GivenName = user.GivenName,
                Surname = user.Surname,
                MobileNumber = user.MobilePhone
            };
            return adPerson;
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