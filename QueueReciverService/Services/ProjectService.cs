using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public class ProjectService : IProjectService
    {
        public void GiveAccessToPlant(int id, string plantId)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<bool> GiveAccessToPlant(string oid, string plantId)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAccessToPlant(int id, string plantId)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<bool> RemoveAccessToPlant(string id, string plantId)
        {
            throw new System.NotImplementedException();
        }
    }
}
