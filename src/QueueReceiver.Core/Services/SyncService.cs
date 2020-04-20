using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Core.Services
{
#pragma warning disable CA1303 // Do not pass literals as localized parameters
    public class SyncService : ISyncService
    {
        private readonly IPlantService _plantService;
        private readonly IGraphService _graphService;
        private readonly IPersonService _personService;
        private readonly IAccessService _accessService;
        private readonly ILogger<SyncService> _logger;

        public SyncService(
            IPlantService plantService, 
            IGraphService graphService,
            IPersonService personService, 
            IAccessService accessService,
            ILogger<SyncService> logger)
        {
            _plantService = plantService;
            _graphService = graphService;
            _personService = personService;
            _accessService = accessService;
            _logger = logger;
        }

        public async Task StartAccessSync(List<string> plantList, bool removeUserAccess)
        {
            _logger.LogInformation($"[GroupSync] : Started at {Timestamp}");

            _logger.LogInformation("[GroupSync] : Getting plants.");
            var plants = new List<Plant>();

            if (plantList.Any())
            {
                plants.AddRange(plantList.Select(plant => _plantService.GetPlant(plant)));
            }
            else
            {
                plants = _plantService.GetAllPlants();
            }

            // Set person CreatedBy cache
            await _personService.SetPersonCreatedByCache();

            foreach (var plant in plants)
            {
                _logger.LogInformation($"[GroupSync] : Started handling {plant.PlantId} at {Timestamp}");

                // Get PCS user OIDs
                var pcsPersonOidList = await GetPcsUserOidList(plant.PlantId);

                // Get AD member OIDs
                var adMemberOidList = await GetAdMemberOidList(new[] {plant.AffiliateGroupId, plant.InternalGroupId});

                // Get AD members that are not existing or mapped by OID in PCS
                var membersInAdNotInPcs = adMemberOidList.Except(pcsPersonOidList).ToList();
                
                if (membersInAdNotInPcs.Any())
                {
                    _logger.LogInformation($"[GroupSync] : Found {membersInAdNotInPcs.Count} members to update from AD.");
                    _logger.LogInformation("[GroupSync] : Starting AD members update.");

                    var members = membersInAdNotInPcs.Select(oid => new Member(oid, shouldRemove: false)).ToList();
                    await ProcessMembers(members, plant.PlantId);

                    _logger.LogInformation("[GroupSync] : Finished AD members update.");
                }
                else
                {
                    _logger.LogInformation("[GroupSync] : No AD members to update.");
                }

                if (removeUserAccess)
                {
                    // Get PCS users that are no longer a member of the AD group(s)
                    var usersInPcsNotInAd = pcsPersonOidList.Except(adMemberOidList).ToList();

                    if (usersInPcsNotInAd.Any())
                    {
                        _logger.LogInformation($"[GroupSync] : Found {usersInPcsNotInAd.Count} users in PCS (remove access from AD group).");
                        _logger.LogInformation("[GroupSync] : Starting PCS users update.");

                        var members = usersInPcsNotInAd.Select(oid => new Member(oid, shouldRemove: true)).ToList();
                        await ProcessMembers(members, plant.PlantId);

                        _logger.LogInformation("[GroupSync] : Finished PCS users update.");
                    }
                    else
                    {
                        _logger.LogInformation("[GroupSync] : No PCS users to update.");
                    }
                }
                else
                {
                    _logger.LogInformation("[GroupSync] : PCS users update (remove access) is disabled. This step will be skipped.");
                }

                _logger.LogInformation($"[GroupSync] : Finished handling {plant.PlantId} at {Timestamp}");
            }

            _logger.LogInformation($"[GroupSync] : Finished at {Timestamp}");
        }

        private async Task ProcessMembers(List<Member> members, string plantId)
        {
            await _accessService.UpdateMemberInfo(members);
            await _accessService.UpdateMemberAccess(members, plantId);
            await _accessService.UpdateMemberVoidedStatus(members);
        }

        private async Task<List<string>> GetPcsUserOidList(string plantId)
        {
            _logger.LogInformation("[GroupSync] : Finding users in PCS (having OID and access to plant).");

            var oids = await _personService.GetMembersWithOidAndAccessToPlant(plantId);
            var oidList = oids.ToList();

            _logger.LogInformation($"[GroupSync] : Found {oidList.Count} users.");

            return oidList;
        }

        private async Task<List<string>> GetAdMemberOidList(IEnumerable<string> groupOids)
        {
            var allMembers = new HashSet<string>();

            foreach (var oid in groupOids)
            {
                _logger.LogInformation($"[GroupSync] : Finding members in AD group {oid}");

                var newMembers = await _graphService.GetMemberOidsAsync(oid);
                var newMemberList = newMembers.ToList();

                _logger.LogInformation($"[GroupSync] : Found {newMemberList.Count} members.");
                
                allMembers.UnionWith(newMemberList);
            }

            _logger.LogInformation($"[GroupSync] : Total AD members: {allMembers.Count}.");

            return allMembers.ToList();
        }

        private static string Timestamp =>
            $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}";
    }
}
