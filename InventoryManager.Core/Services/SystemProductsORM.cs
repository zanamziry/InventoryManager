using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class SystemProductsORM : TableBase<SystemProduct> {
        public SystemProductsORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override void Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(SystemProduct)}(" +
            $"{nameof(SystemProduct.ID)} TEXT NOT NULL UNIQUE," +
            $"{nameof(SystemProduct.Name)} TEXT NOT NULL," +
            $"{nameof(SystemProduct.OpenBalance)} INTEGER," +
            $"{nameof(SystemProduct.Sold)} INTEGER," +
            $"{nameof(SystemProduct.CloseBalance)} INTEGER," +
            $"PRIMARY KEY({nameof(SystemProduct.ID)})," +
            $"FOREIGN KEY({nameof(SystemProduct.ID)}) REFERENCES {nameof(Product)}({nameof(Product.ID)}) ON DELETE SET NULL);";
            _dataAccess.Execute(cmd);
        }

        public override async Task Delete(SystemProduct param) {
            string cmd = $"DELETE FROM {nameof(SystemProduct)} WHERE {nameof(SystemProduct.ID)} = @{nameof(SystemProduct.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task Insert(SystemProduct param) {
            string cmd = $"INSERT INTO {nameof(SystemProduct)} ({nameof(SystemProduct.ID)}, {nameof(SystemProduct.Name)}, {nameof(SystemProduct.OpenBalance)}, {nameof(SystemProduct.Sold)}, {nameof(SystemProduct.CloseBalance)}) values(@{nameof(SystemProduct.ID)}, @{nameof(SystemProduct.Name)}, @{nameof(SystemProduct.OpenBalance)}, @{nameof(SystemProduct.Sold)}, @{nameof(SystemProduct.CloseBalance)});";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<IEnumerable<SystemProduct>> SelectAll() {
            string cmd = $"SELECT * FROM {nameof(SystemProduct)};";
            var products = (await _dataAccess.ReadDataAsync<SystemProduct>(cmd));
            return products;
        }

        public async Task<SystemProduct> SelectProduct(Product product) {
            string cmd = $"SELECT * FROM {nameof(SystemProduct)} WHERE {nameof(SystemProduct.ID)} = @{nameof(Product.ID)};";
            var products = await _dataAccess.ReadDataAsync<SystemProduct>(cmd, product);
            if (products.Count() > 0)
                return products.First();
            return new SystemProduct{ CloseBalance = 0, Name = "NULL", OpenBalance = 0, Sold = 0, ID = "NULL" };
        }
    }
}
