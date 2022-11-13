using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class GivenAwayORM : TableBase<GivenAway> {
        public GivenAwayORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override void Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(GivenAway)}(" +
            $"{nameof(GivenAway.ID)} INTEGER NOT NULL UNIQUE," +
            $"{nameof(GivenAway.InventoryID)} INTEGER NOT NULL," +
            $"{nameof(GivenAway.Amount)} INTEGER NOT NULL DEFAULT 1," +
            $"{nameof(GivenAway.Event)} TEXT NOT NULL," +
            $"FOREIGN KEY({nameof(GivenAway.InventoryID)}) REFERENCES {nameof(LocalInventory)}({nameof(LocalInventory.ID)}) ON DELETE RESTRICT," +
            $"PRIMARY KEY({nameof(GivenAway.ID)} AUTOINCREMENT));";
            _dataAccess.Execute(cmd);
        }

        public override async Task Delete(GivenAway param) {
            string cmd = $"DELETE FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.ID)} = @{nameof(GivenAway.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task Insert(GivenAway param) {
            string cmd = $"INSERT INTO {nameof(GivenAway)} ({nameof(GivenAway.InventoryID)}, {nameof(GivenAway.Amount)}, {nameof(GivenAway.Event)}) values(@{nameof(GivenAway.InventoryID)}, @{nameof(GivenAway.Amount)}, @{nameof(GivenAway.Event)});";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<IEnumerable<GivenAway>> SelectAll() {
            string cmd = $"SELECT * FROM {nameof(GivenAway)};";
            var products = (await _dataAccess.ReadDataAsync<GivenAway>(cmd));
            return products;
        }

        public async Task<IEnumerable<GivenAway>> SelectByInventoryID(LocalInventory inventory) {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.InventoryID)} = @{nameof(LocalInventory.ID)};";
            var products = await _dataAccess.ReadDataAsync<GivenAway>(cmd, inventory);
            return products;
        }
        public async Task<int> SelectTotalAmount(LocalInventory inventory)
        {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.InventoryID)} = @{nameof(LocalInventory.ID)};";
            var givenAways = await _dataAccess.ReadDataAsync<GivenAway>(cmd, inventory);
            int total = 0;
            foreach (var i in givenAways) {
                total += i.Amount;
            }
            return total;
        }
    }
}
