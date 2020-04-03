using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Core.Services
{
    public class SyncService : ISyncService
    {
        private readonly IPlantService _plantService;
        private readonly IGraphService _graphService;
        private readonly IPersonService _personService;
        private readonly IAccessService _accessService;

        public SyncService(
            IPlantService plantService, 
            IGraphService graphService,
            IPersonService personService, 
            IAccessService accessService)
        {
            _plantService = plantService;
            _graphService = graphService;
            _personService = personService;
            _accessService = accessService;
        }

        public async Task StartAccessSync()
        {
            LogProgress("Getting plants.");
            var plants = _plantService.GetAllPlants();

            // Set person CreatedBy cache
            await _personService.SetPersonCreatedByCache();

            foreach (var plant in plants)
            {
                LogProgress($"Current plant: {plant.PlantId}");

                // Get PCS user OIDs
                var dbPersonOidList = await GetPcsUserOidList(plant.PlantId);

                // Get AD member OIDs
                var adMemberOidList = await GetAdMemberOidList(new[] {plant.AffiliateGroupId, plant.InternalGroupId});

                // Get AD members that are not existing or mapped by OID in PCS
                var membersInAdNotInPcs = adMemberOidList.Except(dbPersonOidList).ToList();
                
                // Get PCS users that are no longer a member of the AD group(s)
                var usersInPcsNotInAd = dbPersonOidList.Except(adMemberOidList).ToList();

                if (membersInAdNotInPcs.Any())
                {
                    LogProgress($"Found {membersInAdNotInPcs.Count} members to update from AD.");
                    LogProgress("Starting AD members update.");

                    var members = membersInAdNotInPcs.Select(oid => new Member(oid, shouldRemove: false)).ToList();

                    await _accessService.UpdateMemberInfo(members);
                    await _accessService.UpdateMemberAccess(members, plant.PlantId);
                    await _accessService.UpdateMemberVoidedStatus(members);

                    LogProgress("Finished AD members update.");
                }
                else
                {
                    LogProgress("No AD members to update.");
                }

                if (usersInPcsNotInAd.Any())
                {
                    LogProgress($"Found {usersInPcsNotInAd.Count} users in PCS to remove access from AD group.");
                    LogProgress("Starting PCS users update.");

                    var members = usersInPcsNotInAd.Select(oid => new Member(oid, shouldRemove: true)).ToList();

                    await _accessService.UpdateMemberInfo(members);
                    await _accessService.UpdateMemberAccess(members, plant.PlantId);
                    await _accessService.UpdateMemberVoidedStatus(members);

                    LogProgress("Finished PCS users update.");
                }
                else
                {
                    LogProgress("No PCS users to update.");
                }
            }
        }

        private async Task<List<string>> GetPcsUserOidList(string plantId)
        {
            LogProgress("Finding users in PCS (having OID and access to plant).");

            var oids = await _personService.GetMembersWithOidAndAccessToPlant(plantId);
            var oidList = oids.ToList();

            LogProgress($"Found: {oidList.Count} users.");

            return oidList;
        }

        private async Task<List<string>> GetAdMemberOidList(IEnumerable<string> groupOids)
        {
            var allMembers = new HashSet<string>();

            foreach (var oid in groupOids)
            {
                LogProgress($"Finding members in AD group {oid}");

                var newMembers = await _graphService.GetMemberOidsAsync(oid);
                var newMemberList = newMembers.ToList();

                LogProgress($"Found: {newMemberList.Count} members.");
                
                allMembers.UnionWith(newMemberList);
            }

            LogProgress($"Total AD members: {allMembers.Count}.");

            return allMembers.ToList();
        }

        private static string Timestamp =>
            $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}";

        private void LogProgress(string message)
        {
            message = $"{Timestamp}: {message}";

            Console.WriteLine(message);

            // TODO: log to AI
        }
    }
}
