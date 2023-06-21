namespace InventoryManager.Services
{
    public interface IUpdatingService
    {
        void Initialize();
        Task Update();
    }
}