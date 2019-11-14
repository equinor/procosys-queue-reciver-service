namespace QueueReciverService.Services
{
    public interface IProjectService
    {
        void GiveAccessToPlant(int id, string plantId);
        void RemoveAccessToPlant(int id, string plantId);
    }
}