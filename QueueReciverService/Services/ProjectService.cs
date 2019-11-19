using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public class ProjectService : IProjectService
    {
        public Task<bool> GiveAccessToPlant(string personOid, string plantOid)
        {
            //Get all projects in plant
            //Get all persons projects
            //Update personProjects with projects from plant
            //Save
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveAccessToPlant(string personOid, string plantOid)
        {
            //Get all person projects for user
            //Update persons project and remove all projects belonging to plant
            //Update personProjects with projects from plant
            //Save
            throw new System.NotImplementedException();
        }
    }
}
