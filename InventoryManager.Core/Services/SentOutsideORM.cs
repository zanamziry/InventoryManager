using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class SentOutsideORM : TableBase<SentOutside> {
        public SentOutsideORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override void Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(SentOutside)}(" +
            $"{nameof(SentOutside.ID)} INTEGER NOT NULL UNIQUE," +
            $"{nameof(SentOutside.ProductID)} TEXT NOT NULL," +
            $"{nameof(SentOutside.AmountSent)} INTEGER NOT NULL DEFAULT 0," +
            $"{nameof(SentOutside.AmountSold)} INTEGER NOT NULL DEFAULT 0," +
            $"{nameof(SentOutside.Location)} TEXT NOT NULL," +
            //$"FOREIGN KEY({nameof(SentOutside.ProductID)}) REFERENCES {nameof(Product)}({nameof(Product.ID)}) ON DELETE CASCADE," +
            $"PRIMARY KEY({nameof(SentOutside.ID)} AUTOINCREMENT));";
            _dataAccess.Execute(cmd);
        }

        public override async Task Delete(SentOutside param) {
            string cmd = $"DELETE FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ID)} = @{nameof(SentOutside.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<SentOutside> Insert(SentOutside param) {
            string cmd = $"INSERT INTO {nameof(SentOutside)} ({nameof(SentOutside.ProductID)}, {nameof(SentOutside.AmountSent)},{nameof(SentOutside.AmountSold)}, {nameof(SentOutside.Location)}) values(@{nameof(SentOutside.ProductID)}, @{nameof(SentOutside.AmountSent)}, @{nameof(SentOutside.AmountSold)}, @{nameof(SentOutside.Location)});";
            await _dataAccess.ExecuteAsync(cmd, param);
            int id = await LastInput();
            param.ID = id;
            return param;
        }

        public async Task Update(SentOutside param)
        {
            string cmd = $"UPDATE {nameof(SentOutside)} SET {nameof(SentOutside.AmountSent)} = @{nameof(SentOutside.AmountSent)}, {nameof(SentOutside.AmountSold)} = @{nameof(SentOutside.AmountSold)} WHERE {nameof(SentOutside.ID)} = @{nameof(SentOutside.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public async Task<IEnumerable<SentOutside>> SelectByInventoryID(LocalInventory inventory) {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ProductID)} = @{nameof(LocalInventory.ID)};";
            var sentOutsides = await _dataAccess.ReadDataAsync<SentOutside>(cmd, inventory);
            return sentOutsides;
        }
        public async Task<IEnumerable<string>> SelectAllLocations()
        {
            string cmd = $"SELECT {nameof(SentOutside.Location)} FROM {nameof(SentOutside)} GROUP BY {nameof(SentOutside.Location)};";
            var sentOutsides = await _dataAccess.ReadDataAsync<string>(cmd);
            return sentOutsides;
        }
        public async Task<int> SelectTotalAmountSent(Product p)
        {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ProductID)} = @{nameof(Product.ID)};";
            var sentOutsides = await _dataAccess.ReadDataAsync<SentOutside>(cmd, p);
            int total = 0;
            foreach(var i in sentOutsides)
            {
                total += i.AmountSent;
            }
            return total;
        }
        public async Task<int> SelectTotalAmountSold(Product p)
        {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ProductID)} = @{nameof(Product.ID)};";
            var sentOutsides = await _dataAccess.ReadDataAsync<SentOutside>(cmd, p);
            int total = 0;
            foreach (var i in sentOutsides)
            {
                total += i.AmountSold;
            }
            return total;
        }
        public async Task<IEnumerable<SentOutside>> SelectByLocation(string location)
        {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.Location)} = '{location}';";
            var sentOutsides = await _dataAccess.ReadDataAsync<SentOutside>(cmd);
            return sentOutsides;
        }
        public async Task<IEnumerable<SentOutside>> SelectByProduct(Product p)
        {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.Location)} = @{nameof(Product.ID)};";
            var sentOutsides = await _dataAccess.ReadDataAsync<SentOutside>(cmd, p);
            return sentOutsides;
        }
    }
}
