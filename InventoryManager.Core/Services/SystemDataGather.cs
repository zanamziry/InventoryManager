using System;
using System.Collections.Generic;
using System.Text;
using InventoryManager.Core.Contracts.Services;

namespace InventoryManager.Core.Services
{
    public class SystemDataGather : ISystemDataGather
    {
        string base_url = "http://127.0.0.1/api/inventory";
        HttpClient client = new HttpClient();
        public Task<string> getDataAsync(string id, DateTime date)
        {
            var d = date.ToString("dd-M-yyyy");
            string url = $"{base_url}/{id}/{d}/";
            try
            {
                return client.GetStringAsync(url);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
