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
            Console.WriteLine($"found {allMembers.Count} members in {sw.ElapsedMilliseconds}ms, checking against db");
            sw.Restart();
            var allNotInDb = _personService.GetAllNotInDb(allMembers);
            var tasks = allNotInDb.Select(async m => await _graphService.GetPersonByOid(m));

           var results = await Task.WhenAll(tasks);

            foreach (var aadPerson in results)
            {
                await _personService.FindAndUpdate(aadPerson);
            }

            var added = await _personService.SaveAsync();
            Console.WriteLine($" Syncing with db took: {sw.ElapsedMilliseconds} ms, added {added} persons");
            sw.Stop();

            /**
             * for each group in AAD
             *  loop through members
             *    check if member exist in db
             *      if exists, continue
             *        if not, call graph with member oid 
             *        then check db against shortname firstname and lastname
             *        if 3/3 add oid to user, log
             *          if not, log
             **/

        }
    }
}
