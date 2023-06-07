using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class ProductsORM : TableBase<Product> {
        public ProductsORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override async Task Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(Product)}(" +
            $"{nameof(Product.ID)} TEXT NOT NULL UNIQUE," +
            $"{nameof(Product.Name)} TEXT NOT NULL," +
            $"{nameof(Product.Name_AR)} TEXT," +
            $"{nameof(Product.Price)} REAL NOT NULL," +
            $"{nameof(Product.Old_Price)} REAL," +
            $"{nameof(Product.PV)} REAL NOT NULL," +
            $"PRIMARY KEY({nameof(Product.ID)}));";
            await _dataAccess.ExecuteAsync(cmd);
        }

        public override async Task Delete(Product param) {
            string cmd = $"DELETE FROM {nameof(Product)} WHERE {nameof(Product.ID)} = @{nameof(Product.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }
        public async Task<Product> GetByID(string ProductID)
        {
            string cmd = $"SELECT * FROM {nameof(Product)} WHERE {nameof(Product.ID)} = '{ProductID}';";
            var products = await _dataAccess.ReadDataAsync<Product>(cmd);
            if (products.Count() > 0)
                return products.First();
            return null;
        }
        public async Task DeleteAll()
        {
            string cmd = $"DELETE FROM {nameof(Product)} WHERE 1 = 1;";
            await _dataAccess.ExecuteAsync(cmd);
        }
        
        public override async Task<Product> Insert(Product param) {
            string cmd = $"INSERT INTO {nameof(Product)} ({nameof(Product.ID)}, {nameof(Product.Name)}, {nameof(Product.Name_AR)}, {nameof(Product.Price)}, {nameof(Product.Old_Price)}, {nameof(Product.PV)}) values(@{nameof(Product.ID)}, @{nameof(Product.Name)}, @{nameof(Product.Name_AR)}, @{nameof(Product.Price)},@{nameof(Product.Old_Price)}, @{nameof(Product.PV)});";
            await _dataAccess.ExecuteAsync(cmd, param);
            return param;
        }
    }
}
