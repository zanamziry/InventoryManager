namespace InventoryManager.Core.Contracts.Services
{
    public interface ISystemDataGather
    {
        Task<string> getDataAsync(string id, DateTime date);
    }
}