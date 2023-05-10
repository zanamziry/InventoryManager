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
            $"{nameof(Product.Name_AR)} TEXT," +
            $"{nameof(Product.Price)} REAL NOT NULL," +
            $"{nameof(Product.Old_Price)} REAL," +
            $"{nameof(Product.PV)} REAL NOT NULL," +
            $"PRIMARY KEY({nameof(Product.ID)}));";
            _dataAccess.Execute(cmd);
        }

        public override void Delete(Product param) {
            string cmd = $"DELETE FROM {nameof(Product)} WHERE {nameof(Product.ID)} = @{nameof(Product.ID)};";
            _dataAccess.Execute(cmd, param);
        }
        public async Task<Product> GetByID(string ProductID)
        {
            string cmd = $"SELECT * FROM {nameof(Product)} WHERE {nameof(Product.ID)} = '{ProductID}';";
            var products = await _dataAccess.ReadDataAsync<Product>(cmd);
            if (products.Count() > 0)
                return products.First();
            return null;
        }
        public void DeleteAll()
        {
            string cmd = $"DELETE FROM {nameof(Product)} WHERE 1 = 1;";
            _dataAccess.Execute(cmd);
        }
        
        public override Product Insert(Product param) {
            string cmd = $"INSERT INTO {nameof(Product)} ({nameof(Product.ID)}, {nameof(Product.Name)}, {nameof(Product.Name_AR)}, {nameof(Product.Price)}, {nameof(Product.Old_Price)}, {nameof(Product.PV)}) values(@{nameof(Product.ID)}, @{nameof(Product.Name)}, @{nameof(Product.Name_AR)}, @{nameof(Product.Price)},@{nameof(Product.Old_Price)}, @{nameof(Product.PV)});";
            _dataAccess.Execute(cmd, param);
            return param;
        }
    }
}
