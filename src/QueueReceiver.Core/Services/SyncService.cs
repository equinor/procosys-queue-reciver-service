using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class SyncService : ISyncService
    {
        private readonly IPlantService _plantService;
        private readonly IGraphService _graphService;
        private readonly IPersonService _personService;
        private readonly IUnitOfWork _unitOfWork;

        public SyncService(IPlantService plantService, IGraphService graphService,
            IPersonService personService, IUnitOfWork unitOfWork)
        {
            _plantService = plantService;
            _graphService = graphService;
            _personService = personService;
            _unitOfWork = unitOfWork;
        }


        public async Task ExcecuteAccessSync()
        {
            //Find all groups we want to sync
            //List<Plant> plants = _plantService.GetAllPlants();

            //plants.ForEach(async plant =>
            //{
            //    var AdMemberOids = await GetMemberOidsFromGroups(new string[] { plant.AffiliateGroupId, plant.InternalGroupId });

            // //   List<string> dbMemberOids = _plantService.getAllMemberOids(plant.PlantId);

            //    //var membersInDbNotInAd = dbMemberOids.Except(AdMemberOids).ToList();
            //    //var membersInAdNotInDb = AdMemberOids.Except(dbMemberOids).ToList();

            //    if (membersInAdNotInDb.Any())
            //    {
            //        //GiveAccess;
            //    }
            //    if (membersInDbNotInAd.Any())
            //    {
            //        //RemoveAccess;
            //    }

            //});

            /**
             * 
             * do smt with queue (ie. empty queue and pause queue service)?
             * 
             * For each plant
             *   Find group members in graph with that plant id (internal/affiliate)
             *   for each member in plant(from graph)
             *   {
             *     find members in db thats not in graph
             *       void access
             *       
             *     find members in graph thats not in db,
             *       add/unvoid access
             *    }  
             *    
             *    do smt with queue(ie. empty deadletter queue, start service)?
             * */
        }

        public async Task ExcecuteOidSync()
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
            var allNotInDb = _personService.GetAllNotInDb(allMembers);
            Console.WriteLine($"{allMembers.Count - allNotInDb.Count()} removed : {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            Console.Write("Finding info on members in graph");
            var tasks = allNotInDb.Select(async m => await _graphService.GetPersonByOid(m));
            var results = await Task.WhenAll(tasks);
            Console.WriteLine($" : {sw.ElapsedMilliseconds} ms");

            var resultList = results.ToList();
            Console.WriteLine($"Checking {resultList.Count} members against db using Name and phonenumer, setting oid");
            const int batchSize = 10;
            for (int i = 0; i < resultList.Count; i += batchSize)
            {
                int count = ((resultList.Count - i) < batchSize) ? resultList.Count - i - 1 : batchSize;

                sw.Restart();
                Console.WriteLine($"Adding from {i} to {i+count}");
                foreach (var aadPerson in resultList.GetRange(i,count))
                {
                    await _personService.FindAndUpdate(aadPerson);
                }

                Console.Write($"Starting save  from {i} to {i+count} ");
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
                var newMembers = await _graphService.GetMemberOids(oid);
                Console.WriteLine($" found: {newMembers.Count()}, adding new memebers to set");
                allMembers.UnionWith(newMembers);
            }
            return allMembers;
        }
    }
}
