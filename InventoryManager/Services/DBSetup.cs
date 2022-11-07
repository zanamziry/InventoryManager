using System;
using Microsoft.Data.Sqlite;
using InventoryManager.Contracts.Services;
using System.IO;
using Microsoft.Extensions.Options;
using InventoryManager.Models;
using SQLClient.Contracts;
using SQLClient;
using InventoryManager.Core.Services;

namespace InventoryManager.Services {
    public class DBSetup : IDBSetup {
        private readonly Dictionary<string, ITable> _listOfTables = new Dictionary<string, ITable>();
        private const string preferenceKey = "ConnectionString";
        private const string DBName = "SystemData.db";
        private readonly string _localFolder;
        private string _connectionStr;
        public IDataAccess dataAccess { get; private set; }

        public string ConnectionString
        {
            get => _connectionStr;
            set
            {
                if (string.IsNullOrEmpty(value) || value == _connectionStr)
                    return;
                _connectionStr = value;
                SaveConnectionString(value);
            }
        }

        public DBSetup(IOptions<AppConfig> appConfig) {
            _localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appConfig.Value.ConfigurationsFolder);
        }

        public void InitializeDatabase() {
            _connectionStr = LoadConnectionString();
            if (!Directory.Exists(_localFolder))
                Directory.CreateDirectory(_localFolder);
            if (string.IsNullOrEmpty(ConnectionString)) {
                SqliteConnectionStringBuilder CSBuilder = new SqliteConnectionStringBuilder
                {
                    DataSource = Path.Combine(_localFolder, DBName)
                };
                ConnectionString = CSBuilder.ConnectionString;
            }
            dataAccess = new DataAccess(ConnectionString);
            CreateTables();
        }

        private void SaveConnectionString(string ConnectionStr) => App.Current.Properties[preferenceKey] = ConnectionStr;
        private string LoadConnectionString() => App.Current.Properties.Contains(preferenceKey) ? (string)App.Current.Properties[preferenceKey] : null;
        
        public void CreateTables() {

            //TODO: Add your Database Tables here, and place them correctly based on the connection between 
            Configure(new ProductsORM(dataAccess));
            Configure(new LocalInventoryORM(dataAccess));
            Configure(new SystemProductsORM(dataAccess));
            foreach (ITable table in _listOfTables.Values) {
                table.Create();
            }
        }

        public void DropTables() {
            foreach (ITable table in _listOfTables.Values) {
                table.Drop();
            }
        }
        private void Configure<T>(T table)
        where T : ITable {
            string key = typeof(T).FullName;
            if (_listOfTables.ContainsKey(key))
                throw new ArgumentException($"Key {key} already exists in {nameof(_listOfTables)}");
            if (_listOfTables.ContainsValue(table))
                throw new ArgumentException($"this value is already paired with key {key}");
            _listOfTables.Add(key, table);
        }
        public T GetTable<T>() {
            string key = typeof(T).FullName;
            if (!_listOfTables.TryGetValue(key, out ITable table)) {
                throw new KeyNotFoundException($"Table not found: {key}. Did you forget to call DBSetup.Configure?");
            }
            return (T)table;
        }
    }
}
