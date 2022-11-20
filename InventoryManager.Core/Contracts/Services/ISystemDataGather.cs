namespace InventoryManager.Core.Contracts.Services
{
    public interface ISystemDataGather
    {
        Task<string> GetInventoryAsync(string id, DateTime date);
    }
}