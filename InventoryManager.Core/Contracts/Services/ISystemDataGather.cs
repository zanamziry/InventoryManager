namespace InventoryManager.Core.Contracts.Services
{
    public interface ISystemDataGather
    {
        Task<string> GetInventoryAsync(string id, DateTime date);
        string ServerAddress { get; set; }
        string SettingsKey { get; }
    }
}