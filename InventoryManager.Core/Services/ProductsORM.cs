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
        public async Task DeleteAll()
        {
            string cmd = $"DELETE FROM {nameof(Product)} WHERE 1 = 1;";
            await _dataAccess.ExecuteAsync(cmd);
        }

        public override async Task<Product> Insert(Product param) {
            string cmd = $"INSERT INTO {nameof(Product)} ({nameof(Product.ID)}, {nameof(Product.Name)}, {nameof(Product.Price)}) values(@{nameof(Product.ID)}, @{nameof(Product.Name)}, @{nameof(Product.Price)});";
            await _dataAccess.ExecuteAsync(cmd, param);
            return param;
        }
    }
}
