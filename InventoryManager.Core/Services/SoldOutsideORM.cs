using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class SoldOutsideORM : TableBase<SoldOutside> {
        public SoldOutsideORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override void Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(SoldOutside)}(" +
            $"{nameof(SoldOutside.ID)} INTEGER NOT NULL UNIQUE," +
            $"{nameof(SoldOutside.OutsideID)} INTEGER NOT NULL," +
            $"{nameof(SoldOutside.Amount)} INTEGER NOT NULL," +
            $"{nameof(SoldOutside.Date)} TEXT NOT NULL," +
            $"{nameof(SoldOutside.CCode)} TEXT," +
            $"FOREIGN KEY({nameof(SoldOutside.OutsideID)}) REFERENCES {nameof(SentOutside)}({nameof(SentOutside.ID)}) ON DELETE RESTRICT," +
            $"PRIMARY KEY({nameof(SoldOutside.ID)} AUTOINCREMENT));";
            _dataAccess.Execute(cmd);
        }

        public override async Task Delete(SoldOutside param) {
            string cmd = $"DELETE FROM {nameof(SoldOutside)} WHERE {nameof(SoldOutside.ID)} = @{nameof(SoldOutside.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public async Task DeleteByOutsideID(SoldOutside param)
        {
            string cmd = $"DELETE FROM {nameof(SoldOutside)} WHERE {nameof(SoldOutside.OutsideID)} = @{nameof(SoldOutside.OutsideID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task Insert(SoldOutside param) {
            string cmd = $"INSERT INTO {nameof(Product)} ({nameof(Product.ID)}, {nameof(Product.Name)}, {nameof(Product.Price)}) values(@{nameof(Product.ID)}, @{nameof(Product.Name)}, @{nameof(Product.Price)});";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<IEnumerable<SoldOutside>> SelectAll() {
            string cmd = $"SELECT * FROM {nameof(SoldOutside)};";
            var products = await _dataAccess.ReadDataAsync<SoldOutside>(cmd);
            return products;
        }
    }
}
