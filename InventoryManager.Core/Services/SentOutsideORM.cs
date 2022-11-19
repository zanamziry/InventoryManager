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
            $"{nameof(SentOutside.InventoryID)} INTEGER NOT NULL," +
            $"{nameof(SentOutside.AmountSent)} INTEGER NOT NULL DEFAULT 0," +
            $"{nameof(SentOutside.AmountSold)} INTEGER NOT NULL DEFAULT 0," +
            $"{nameof(SentOutside.Location)} TEXT NOT NULL," +
            $"FOREIGN KEY({nameof(SentOutside.InventoryID)}) REFERENCES {nameof(LocalInventory)}({nameof(LocalInventory.ID)}) ON DELETE RESTRICT," +
            $"PRIMARY KEY({nameof(SentOutside.ID)} AUTOINCREMENT));";
            _dataAccess.Execute(cmd);
        }

*/
        public override async Task Delete(SentOutside param) {
            string cmd = $"DELETE FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ID)} = @{nameof(SentOutside.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task Insert(SentOutside param) {
            string cmd = $"INSERT INTO {nameof(SentOutside)} ({nameof(SentOutside.InventoryID)}, {nameof(SentOutside.AmountSent)},{nameof(SentOutside.AmountSold)}, {nameof(SentOutside.Location)}) values(@{nameof(SentOutside.InventoryID)}, @{nameof(SentOutside.AmountSent)}, @{nameof(SentOutside.AmountSold)}, @{nameof(SentOutside.Location)});";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<IEnumerable<SentOutside>> SelectAll() {
            string cmd = $"SELECT * FROM {nameof(SentOutside)};";
            var products = await _dataAccess.ReadDataAsync<SentOutside>(cmd);
            return products;
        }

        public async Task<IEnumerable<SentOutside>> SelectByInventoryID(LocalInventory inventory) {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.InventoryID)} = @{nameof(LocalInventory.ID)};";
            var sentOutsides = await _dataAccess.ReadDataAsync<SentOutside>(cmd, inventory);
            return sentOutsides;
        }
        public async Task<int> SelectTotalAmountSent(LocalInventory inventory)
        {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.InventoryID)} = @{nameof(LocalInventory.ID)};";
            var sentOutsides = await _dataAccess.ReadDataAsync<SentOutside>(cmd, inventory);
            int total = 0;
            foreach(var i in sentOutsides)
            {
                total += i.AmountSent;
            }
            return total;
        } 
    }
}
