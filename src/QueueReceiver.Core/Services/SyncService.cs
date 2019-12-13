using QueueReceiver.Core.Interfaces;
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

        public SyncService(IPlantService plantService, IGraphService graphService, IPersonService personService)
        {
            _plantService = plantService;
            _graphService = graphService;
            _personService = personService;
        }

        public async Task ExcecuteOidSync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var groupOids = _plantService.GetAllGroupOids();
            Console.WriteLine($"get all groups took: {sw.ElapsedMilliseconds} ms, found {groupOids.Count()} groups");
            
            sw.Restart();
            var allMembers = new HashSet<string>();
            foreach (var oid in groupOids)
            {
                Console.WriteLine($"finding members in {oid}");
                var newMembers = await _graphService.GetMemberOids(oid);
                Console.WriteLine($" found: {newMembers.Count()}, adding new memebers to set");
                allMembers.UnionWith(newMembers);
            }
            Console.WriteLine($"found {allMembers.Count} members in azureAD:  {sw.ElapsedMilliseconds} ms");
            
            sw.Restart();
            Console.Write("removing members not already in db members ");
            var allNotInDb = _personService.GetAllNotInDb(allMembers);
            Console.WriteLine($"{allMembers.Count - allNotInDb.Count()} removed in: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            Console.Write("Finding info on members in graph");
            var tasks = allNotInDb.Select(async m => await _graphService.GetPersonByOid(m));
            var results = await Task.WhenAll(tasks);
            Console.WriteLine($"  : {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            Console.Write("Checking against db and tracking tracking to update");
            foreach (var aadPerson in results)
            {
                await _personService.FindAndUpdate(aadPerson);
            }
            Console.WriteLine($"  : {sw.ElapsedMilliseconds}");


            sw.Restart();
            Console.Write("Starting save");
            var added = await _personService.SaveAsync();
            Console.WriteLine($" added {added} persons with oid to the db in:  {sw.ElapsedMilliseconds} ms");
            sw.Stop();
        }
    }
}
