using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using InventoryManager.Contracts.Services;
using InventoryManager.Core.Models;
using Newtonsoft.Json;

namespace InventoryManager.Services
{
    public class SystemDataGather : ISystemDataGather
    {
        private string _password;
        private string _username;
        private string _base_url;
        private readonly string ServerSettingsKey = "ServerAddress";
        private readonly string UserSettingsKey = "Username";
        private readonly string PassSettingsKey = "Password";
        public string DEFAULT { get => Properties.Resources.Server; }

        public string BASE_URL
        {
            get 
            {
                if (string.IsNullOrEmpty(_base_url))
                {
                    if (App.Current.Properties.Contains(ServerSettingsKey))
                        _base_url = App.Current.Properties[ServerSettingsKey].ToString();
                    else _base_url = DEFAULT;
                }
                return _base_url;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value == _base_url)
                    return;
                _base_url = value;
                App.Current.Properties[ServerSettingsKey] = value;
            }
        }

        public string Username
        {
            get 
            {
                if (string.IsNullOrEmpty(_username))
                {
                    if (App.Current.Properties.Contains(UserSettingsKey))
                        _username = App.Current.Properties[UserSettingsKey].ToString();
                    else _username = "";
                }
                return _username;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value == _username)
                    return;
                _username = value;
                App.Current.Properties[UserSettingsKey] = value;
            }
        }

        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_password))
                {
                    if (App.Current.Properties.Contains(PassSettingsKey))
                        _password = App.Current.Properties[PassSettingsKey].ToString();
                    else _password = "";
                }
                return _password;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value == _password)
                    return;
                _password = value;
                App.Current.Properties[PassSettingsKey] = value;
            }
        }

        public async Task<SystemAPI> GetInventoryAsync(string id, DateTime date)
        {
            HttpResponseMessage resp = new();
            var d = date.ToString("dd-M-yyyy");
            string url = $"{BASE_URL}/api/inventory/{id}/?format=json&date={d}";
            var agent = new Agent
            {
                username = Username,
                password = Password,
            };
            using StringContent content = new StringContent(JsonConvert.SerializeObject(agent));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using HttpClient client = new();
            try
            {
                resp = await client.PostAsync(url, content);
                return JsonConvert.DeserializeObject<SystemAPI>(await resp.Content.ReadAsStringAsync());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public string SHA1Gen(string text)
        {
            byte[] hashBytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            string url = $"{BASE_URL}/api/?format=json";
            IEnumerable<Product> products;
            using HttpClient client = new();
            try
            {
                string resp = await client.GetStringAsync(url);
                products = JsonConvert.DeserializeObject<IEnumerable<Product>>(resp);
            }
            catch (Exception ex)
            {
                products = Enumerable.Empty<Product>();
                MessageBox.Show(ex.Message);
            }
            return products;
        }

        public async Task<IEnumerable<ServiceCenter>> GetServiceCentersAsync()
        {
            IEnumerable<ServiceCenter> centers;
            string url = $"{BASE_URL}/api/service_centers/?format=json";
            using (HttpClient client = new())
            {
                try
                {
                    var resp = await client.GetStringAsync(url);
                    centers = JsonConvert.DeserializeObject<IEnumerable<ServiceCenter>>(resp);
                }
                catch (Exception ex)
                {
                    centers = Enumerable.Empty<ServiceCenter>();
                    MessageBox.Show(ex.Message);
                }
            }
            return centers;
        }

        public async Task<bool> TestLogin(string user, string passwd)
        {
            string url = $"{BASE_URL}/api/testLogin/";
            var agent = new Agent
            {
                username = user,
                password = passwd,
            };
            //i didnt use jsonContent and PostAsJsonAsync because for some reason it was not working
            using StringContent content = new StringContent(JsonConvert.SerializeObject(agent));
            content.Headers.ContentType =new MediaTypeHeaderValue("application/json");
            using HttpClient client = new();
            using HttpResponseMessage resp = await client.PostAsync(url, content);
            string result = await resp.Content.ReadAsStringAsync();
            if (bool.TryParse(result, out bool resultOfLogin))
                return resultOfLogin;
            else return false;
        }
    }
}
