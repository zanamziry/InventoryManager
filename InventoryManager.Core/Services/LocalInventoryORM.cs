using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class LocalInventoryORM : TableBase<LocalInventory> {
        public LocalInventoryORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override void Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(LocalInventory)}(" +
            $"{nameof(LocalInventory.ID)} INTEGER NOT NULL UNIQUE," +
            $"{nameof(LocalInventory.ProductID)} TEXT NOT NULL," +
            $"{nameof(LocalInventory.Inventory)} INTEGER NOT NULL," +
            $"{nameof(LocalInventory.Open)} INTEGER NOT NULL," +
            $"{nameof(LocalInventory.ExpireDate)} TEXT," +
            $"PRIMARY KEY({nameof(LocalInventory.ID)} AUTOINCREMENT)," +
            $"FOREIGN KEY({nameof(LocalInventory.ProductID)}) REFERENCES {nameof(Product)}({nameof(Product.ID)}) ON DELETE CASCADE);";
            _dataAccess.Execute(cmd);
        }

        public override async Task Delete(LocalInventory param) {
            string cmd = $"DELETE FROM {nameof(LocalInventory)} WHERE {nameof(LocalInventory.ID)} = @{nameof(LocalInventory.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<LocalInventory> Insert(LocalInventory param) {
            string cmd = $"INSERT INTO {nameof(LocalInventory)} ({nameof(LocalInventory.ProductID)}, {nameof(LocalInventory.Inventory)}, {nameof(LocalInventory.Open)}, {nameof(LocalInventory.ExpireDate)}) values(@{nameof(LocalInventory.ProductID)}, @{nameof(LocalInventory.Inventory)}, @{nameof(LocalInventory.Open)}, @{nameof(LocalInventory.ExpireDate)});";
            await _dataAccess.ExecuteAsync(cmd, param);
            int id = await LastInput();
            param.ID = id;
            return param;
        }
        public async Task Update(LocalInventory param)
        {
            string cmd = $"UPDATE {nameof(LocalInventory)} SET {nameof(LocalInventory.Inventory)} = @{nameof(LocalInventory.Inventory)}, {nameof(LocalInventory.Open)} = @{nameof(LocalInventory.Open)}, {nameof(LocalInventory.ExpireDate)} = @{nameof(LocalInventory.ExpireDate)} WHERE {nameof(LocalInventory.ID)} = @{nameof(LocalInventory.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public async Task<int> GetReal(Product product)
        {
            string cmd = $"SELECT * FROM {nameof(LocalInventory)} WHERE {nameof(LocalInventory.ProductID)} = @{nameof(Product.ID)};";
            var products = await _dataAccess.ReadDataAsync<LocalInventory>(cmd, product);
            if (products.Count() > 0)
                return products.First().Total;
            return 0;
        }
        public async Task<LocalInventory> GetByID(int InvID)
        {
            string cmd = $"SELECT * FROM {nameof(LocalInventory)} WHERE {nameof(LocalInventory.ID)} = {InvID};";
            var products = await _dataAccess.ReadDataAsync<LocalInventory>(cmd);
            if (products.Count() > 0)
                return products.First();
            return null;
        }
        public async Task<IEnumerable<LocalInventory>> SelectProduct(Product product)
        {
            string cmd = $"SELECT * FROM {nameof(LocalInventory)} WHERE {nameof(LocalInventory.ProductID)} = @{nameof(Product.ID)};";
            var products = await _dataAccess.ReadDataAsync<LocalInventory>(cmd, product);
            return products;
        }
    }
}
