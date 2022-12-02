using System;
using System.Diagnostics;
using InventoryManager.Core.Contracts.Services;

namespace InventoryManager.Core.Services
{
    public class SystemDataGather : ISystemDataGather
    {
        string base_url { get; set; } = "http://127.0.0.1";
        public string ServerAddress { get => base_url; set => base_url = value; }

        string ISystemDataGather.SettingsKey => "ServerAddress";

        HttpClient client = new HttpClient();

        public Task<string> GetInventoryAsync(string id, DateTime date)
        {
            var d = date.ToString("dd-M-yyyy");
            string url = $"{base_url}/api/inventory/{id}/?format=json&date={d}";
            return client.GetStringAsync(url);
        }

        public Task<string> GetProductsAsync()
        {
            string url = $"{base_url}/?format=json";
            return client.GetStringAsync(url);
        }
    }
}
