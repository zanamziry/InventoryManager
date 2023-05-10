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
            $"PRIMARY KEY({nameof(SystemProduct.ID)}));";
            //$"FOREIGN KEY({nameof(SystemProduct.ID)}) REFERENCES {nameof(Product)}({nameof(Product.ID)}) on delete no action);";
            _dataAccess.Execute(cmd);
        }

        public override void Delete(SystemProduct param) {
            string cmd = $"DELETE FROM {nameof(SystemProduct)} WHERE {nameof(SystemProduct.ID)} = @{nameof(SystemProduct.ID)};";
            _dataAccess.Execute(cmd, param);
        }

        public override SystemProduct Insert(SystemProduct param) {
            string cmd = $"INSERT INTO {nameof(SystemProduct)} ({nameof(SystemProduct.ID)}, {nameof(SystemProduct.Name)}, {nameof(SystemProduct.OpenBalance)}, {nameof(SystemProduct.Sold)}, {nameof(SystemProduct.CloseBalance)}) values(@{nameof(SystemProduct.ID)}, @{nameof(SystemProduct.Name)}, @{nameof(SystemProduct.OpenBalance)}, @{nameof(SystemProduct.Sold)}, @{nameof(SystemProduct.CloseBalance)});";
            _dataAccess.Execute(cmd, param);
            return param;
        }
        public void DeleteAll()
        {
            string cmd = $"DELETE FROM {nameof(SystemProduct)} WHERE 1 = 1;";
            _dataAccess.Execute(cmd);
        }

        public SystemProduct SelectProduct(Product product)
        {
            string cmd = $"SELECT * FROM {nameof(SystemProduct)} WHERE {nameof(SystemProduct.ID)} = @{nameof(Product.ID)};";
            var products = _dataAccess.ReadData<SystemProduct>(cmd,product);
            if (products.Count() > 0)
                return products.First();
            return new SystemProduct { ID = "NULL", Name = "NULL", CloseBalance = 0, OpenBalance = 0, Sold = 0};
        }
    }
}
