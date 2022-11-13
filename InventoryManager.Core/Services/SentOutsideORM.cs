using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class SentOutsideORM : TableBase<SentOutside> {
        public SentOutsideORM(IDataAccess dataAccess) : base(dataAccess) {
        }
        /*
            CREATE TABLE "SentOutside" (
	        "ID"	INTEGER NOT NULL UNIQUE,
	        "InventoryID"	INTEGER NOT NULL,
	        "Amount"	INTEGER NOT NULL DEFAULT 0,
	        "Location"	TEXT NOT NULL,
	        PRIMARY KEY("ID" AUTOINCREMENT),
	        FOREIGN KEY("InventoryID") REFERENCES "localinventory"("ID") ON DELETE RESTRICT
)
         */
        public override void Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(SentOutside)}(" +
            $"{nameof(SentOutside.ID)} TEXT NOT NULL UNIQUE," +
            $"{nameof(SentOutside.Name)} TEXT NOT NULL," +
            $"{nameof(SentOutside.Price)} REAL NOT NULL," +
            $"FOREIGN KEY({nameof(SentOutside.InventoryID)}) REFERENCES {nameof(LocalInventory)}({nameof(LocalInventory.ID)}) ON DELETE RESTRICT," +
            $"PRIMARY KEY({nameof(SentOutside.ID)}));";
            _dataAccess.Execute(cmd);
        }

        public override async Task Delete(SentOutside param) {
            string cmd = $"DELETE FROM {nameof(Product)} WHERE {nameof(Product.ID)} = @{nameof(Product.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task Insert(SentOutside param) {
            string cmd = $"INSERT INTO {nameof(Product)} ({nameof(Product.ID)}, {nameof(Product.Name)}, {nameof(Product.Price)}) values(@{nameof(Product.ID)}, @{nameof(Product.Name)}, @{nameof(Product.Price)});";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<IEnumerable<SentOutside>> SelectAll() {
            string cmd = $"SELECT * FROM {nameof(SentOutside)};";
            var products = (await _dataAccess.ReadDataAsync<SentOutside>(cmd));
            return products;
        }
    }
}
