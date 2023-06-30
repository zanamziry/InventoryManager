using InventoryManager.Core.Models;

namespace InventoryManager.Contracts.Services
{
    public interface ISystemDataGather
    {
        Task<SystemAPI> GetInventoryAsync(string id, DateTime date);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<ServiceCenter>> GetServiceCentersAsync();
        Task<bool> TestLogin(string username, string password);

        string DEFAULT { get; }
        string BASE_URL { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}