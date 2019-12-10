using QueueReceiver.Core.Interfaces;
using System;
using System.Collections.Generic;
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
            var groupOids = _plantService.GetAllGroupOids();

            foreach(var oid in groupOids)
            {
               var members = await _graphService.GetMemberOids(oid);

                foreach(var memberOid in members)
                {
                    if (await _personService.FindByOid(memberOid) != null)
                        continue;

                    _personService.FindAndUpdate(memberOid);
                    
                }
            }
            /*
             * for each group in AAD
             *  loop through members
             *    check if member exist in db
             *      if exists, continue
             *        if not, call graph with member oid 
             *        then check db against shortname firstname and lastname
             *        if 3/3 add oid to user, log
             *          if not, log
             * 
             * 
             * 
             * 
             */

        }
    }
}
