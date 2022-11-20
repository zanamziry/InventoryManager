using System;
using System.Diagnostics;
using InventoryManager.Core.Contracts.Services;

namespace InventoryManager.Core.Services
{
    public class SystemDataGather : ISystemDataGather
    {
        string base_url = "http://127.0.0.1/api";
        HttpClient client = new HttpClient();

        public Task<string> GetInventoryAsync(string id, DateTime date)
        {
            var d = date.ToString("dd-M-yyyy");
            string url = $"{base_url}/inventory/{id}/?format=json&date={d}";
            try
            {
                return client.GetStringAsync(url);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public Task<string> GetProductsAsync()
        {
            string url = $"{base_url}/?format=json";
            try
            {
                return client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
