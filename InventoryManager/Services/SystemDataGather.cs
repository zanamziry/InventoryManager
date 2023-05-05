using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using InventoryManager.Contracts.Services;

namespace InventoryManager.Services
{
    public class SystemDataGather : ISystemDataGather
    {
        public string DEFAULT { get => "https://zanamziry.pythonanywhere.com"; }
        public string base_url { get; set; }
        
        public string SettingsKey => "ServerAddress";

        HttpClient client = new HttpClient();

        public Task<string> GetInventoryAsync(string id, DateTime date)
        {
            var d = date.ToString("dd-M-yyyy");
            string url = $"{base_url}/api/inventory/{id}/?format=json&date={d}";
            return client.GetStringAsync(url);
        }

        public string SHA1Gen(string text)
        {
            byte[] hashBytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public async Task<string> GetProductsAsync()
        {
            string url = $"{base_url}/api/?format=json";
            return await client.GetStringAsync(url);
        }

        public async Task<string> GetAgentsAsync()
        {
            string url = $"{base_url}/api/agent/?format=json";
            return await client.GetStringAsync(url);
        }

        public void LoadSettings()
        {
            if (App.Current.Properties.Contains(SettingsKey))
                base_url = App.Current.Properties[SettingsKey].ToString();
            else base_url = DEFAULT;
        }

        public void SaveSettings(string newUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl) || newUrl == base_url)
                return;
            base_url = newUrl;
            App.Current.Properties[SettingsKey] = newUrl;
        }
    }
}
