using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class ProductsORM : TableBase<Product> {
        public ProductsORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override void Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(Product)}(" +
            $"{nameof(Product.ID)} TEXT NOT NULL UNIQUE," +
            $"{nameof(Product.Name)} TEXT NOT NULL," +
            $"{nameof(Product.Price)} REAL NOT NULL," +
            $"PRIMARY KEY({nameof(Product.ID)}));";
            _dataAccess.Execute(cmd);
        }

        public override async Task Delete(Product param) {
            string cmd = $"DELETE FROM {nameof(Product)} WHERE {nameof(Product.ID)} = @{nameof(Product.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task Insert(Product param) {
            string cmd = $"INSERT INTO {nameof(Product)} ({nameof(Product.ID)}, {nameof(Product.Name)}, {nameof(Product.Price)}) values(@{nameof(Product.ID)}, @{nameof(Product.Name)}, @{nameof(Product.Price)});";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<IEnumerable<Product>> SelectAll() {
            string cmd = $"SELECT * FROM {nameof(Product)};";
            var products = (await _dataAccess.ReadDataAsync<Product>(cmd));
            return products;
        }
    }
}
