namespace InventoryManager.Contracts.Services
{
    public interface ISystemDataGather
    {
        Task<string> GetInventoryAsync(string id, DateTime date);
        Task<string> GetProductsAsync();
        void LoadSettings();
        void SaveSettings(string NewUrl);
        string base_url { get; set; }
        string SettingsKey { get; }
    }
}