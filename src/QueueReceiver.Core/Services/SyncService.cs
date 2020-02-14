using QueueReceiver.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Core.Services
{
    public class SyncService : ISyncService
    {
        private readonly IPlantService _plantService;
        private readonly IGraphService _graphService;
        private readonly IPersonService _personService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;

        public SyncService(IPlantService plantService, IGraphService graphService,
            IPersonService personService, IUnitOfWork unitOfWork, IAccessService accessService)
        {
            _plantService = plantService;
            _graphService = graphService;
            _personService = personService;
            _unitOfWork = unitOfWork;
            _accessService = accessService;
        }

        public async Task StartAccessSync()
        {
            var plants = _plantService.GetAllPlants();

            foreach(var plant in plants)
            {
                var dbpersons = await _personService.GetMembersWithAccessToPlant(plant.PlantId);
                var adMemberOids = await GetMemberOidsFromGroups(new string[] { plant.AffiliateGroupId, plant.InternalGroupId });
                dbpersons = dbpersons.ToList();
                var adMemberList = adMemberOids.ToList();
                var membersInAdNotInDb = adMemberList.Except(dbpersons);
                var membersInDbNotInAd = dbpersons.Except(adMemberList).ToList();

                if (membersInAdNotInDb.Any())
                {
                    var members = membersInAdNotInDb.Select(miad => new Member(miad, shouldRemove: false)).ToList();

                    foreach(Member m in members)
                    {
                        await _accessService.UpdateMemberInfo(m);

                    }
                   
                    await _accessService.UpdateMemberAccess(members, plant.PlantId);
                }

                //if (membersInDbNotInAd.Any()) //TODO: Not for production without check
                //{
                //    var members = membersInDbNotInAd.Select(midb => new Member(midb, shouldRemove: true)).ToList();
                //    await _accessService.UpdateMemberInfo(members);
                //    await _accessService.UpdateMemberAccess(members, plant.PlantId);
                //}
            }

            // //   List<string> dbMemberOids = _plantService.getAllMemberOids(plant.PlantId);

            // lykke pseudo
            // Step 1: Find all groups we want to sync (all plants or projects??)
            // Step 2: Find members in PCS, not in Ad
            // Step 3: Find members in Ad, not in PCS
            // Step 4: If there are any members found in Ad and not in PCS db, give them access, i.e call OG add access method
            // Step 5: If there are any members found in PCS db and not in Ad, remove their access, i.e call OG remove access method

            // 1 plant = 1 group or 1 project = 1 group? 
            // rename methods...

        }

        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
        public async Task ExcecuteOidUpdateAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var groupOids = _plantService.GetAllGroupOids();
            Console.WriteLine($"get all groups took: {sw.ElapsedMilliseconds} ms, found {groupOids.Count()} groups");

            sw.Restart();
            HashSet<string> allMembers = await GetMemberOidsFromGroups(groupOids);
            Console.WriteLine($"found {allMembers.Count} members in azureAD:  {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            Console.Write("Removing members not already in db members ");
            var allNotInDb = _personService.GetAllNotInDb(allMembers).ToList();
            Console.WriteLine($"{allMembers.Count - allNotInDb.Count} removed : {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            Console.Write("Finding info on members in graph");
            var tasks = allNotInDb.Select(async m =>
            {
                try
                {
                    return await _graphService.GetAdPersonByOidAsync(m);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                return null;
            });
            var results = await Task.WhenAll(tasks);
            Console.WriteLine($" : {sw.ElapsedMilliseconds} ms");

            var resultList = results.ToList();
            Console.WriteLine($"Checking {resultList.Count} members against db using Name and phonenumer, setting oid");
            const int batchSize = 10;
            for (int i = 0; i < resultList.Count; i += batchSize)
            {
                int count = ((resultList.Count - i) < batchSize) ? resultList.Count - i - 1 : batchSize;

                sw.Restart();
                Console.WriteLine($"Adding from {i} to {i + count}");
                foreach (var adPerson in resultList.GetRange(i, count))
                {
                    await _personService.FindAndUpdateAsync(adPerson);
                }

                Console.Write($"Starting save  from {i} to {i + count} ");
                var added = await _unitOfWork.SaveChangesAsync();
                Console.WriteLine($" added {added} persons with oid to the db :  {sw.ElapsedMilliseconds} ms");
            }
            sw.Stop();
        }

        private async Task<HashSet<string>> GetMemberOidsFromGroups(IEnumerable<string> groupOids)
        {
            var allMembers = new HashSet<string>();
            foreach (var oid in groupOids)
            {
                Console.WriteLine($"finding members in {oid}");
                var newMembers = await _graphService.GetMemberOidsAsync(oid);
                var newmemberList = newMembers.ToList();
                Console.WriteLine($" found: {newmemberList.Count}, adding new memebers to set");
                allMembers.UnionWith(newmemberList);
            }
            return allMembers;
        }
    }
}
