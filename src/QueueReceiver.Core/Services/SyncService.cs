using System;
using System.Collections.Generic;
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
            var plants = _plantService.GetAllPlants();

            // Set person CreatedBy cache
            await _personService.SetPersonCreatedByCache();

            foreach (var plant in plants)
            {
                var dbpersons = await _personService.GetMembersWithOidAndAccessToPlant(plant.PlantId);
                var adMemberOids = await GetMemberOidsFromGroups(new string[] { plant.AffiliateGroupId, plant.InternalGroupId });
                dbpersons = dbpersons.ToList();
                var adMemberList = adMemberOids.ToList();

                var membersInAdNotInDb = adMemberList.Except(dbpersons);
                var membersInDbNotInAd = dbpersons.Except(adMemberList).ToList();

                if (membersInAdNotInDb.Any())
                {
                    var members = membersInAdNotInDb.Select(miad => new Member(miad, shouldRemove: false)).ToList();

                    await _accessService.UpdateMemberInfo(members);
                    await _accessService.UpdateMemberAccess(members, plant.PlantId);
                }
                //if (membersInDbNotInAd.Any()) //TODO: Not for production without check
                //{
                //    var members = membersInDbNotInAd.Select(midb => new Member(midb, shouldRemove: true)).ToList();
                //    await _accessService.UpdateMemberInfo(members);
                //    await _accessService.UpdateMemberAccess(members, plant.PlantId);
                //}
            }
        }

        private async Task<HashSet<string>> GetMemberOidsFromGroups(IEnumerable<string> groupOids)
        {
            var allMembers = new HashSet<string>();
            foreach (var oid in groupOids)
            {
                Console.WriteLine($"Finding members in {oid}");
                var newMembers = await _graphService.GetMemberOidsAsync(oid);
                var newMemberList = newMembers.ToList();
                Console.WriteLine($"Found: {newMemberList.Count}, adding new members to set");
                allMembers.UnionWith(newMemberList);
            }
            return allMembers;
        }
    }
}
