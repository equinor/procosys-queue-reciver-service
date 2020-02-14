using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetParentProjectsByPlant(string plantId);
    }
}
