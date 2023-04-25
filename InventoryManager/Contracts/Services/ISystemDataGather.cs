namespace InventoryManager.Contracts.Services
{
    public interface ISystemDataGather
    {
        Task<string> GetInventoryAsync(string id, DateTime date);
        Task<string> GetProductsAsync();
        Task<string> GetAgentsAsync();

        void LoadSettings();
        void SaveSettings(string NewUrl);
        string DEFAULT { get; }
        string SettingsKey { get; }
        string base_url { get; set; }
    }
}