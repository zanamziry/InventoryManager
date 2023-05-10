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
            $"{nameof(SentOutside.Old)} INTEGER NOT NULL DEFAULT 0," +
            //$"FOREIGN KEY({nameof(SentOutside.ProductID)}) REFERENCES {nameof(Product)}({nameof(Product.ID)}) ON DELETE CASCADE," +
            $"PRIMARY KEY({nameof(SentOutside.ID)} AUTOINCREMENT));";
            _dataAccess.Execute(cmd);
        }

        public override void Delete(SentOutside param) {
            string cmd = $"DELETE FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ID)} = @{nameof(SentOutside.ID)};";
            _dataAccess.Execute(cmd, param);
        }

        public override SentOutside Insert(SentOutside param) {
            string cmd = $"INSERT INTO {nameof(SentOutside)} ({nameof(SentOutside.ProductID)}, {nameof(SentOutside.AmountSent)},{nameof(SentOutside.AmountSold)}, {nameof(SentOutside.Location)}, {nameof(SentOutside.Old)}) values(@{nameof(SentOutside.ProductID)}, @{nameof(SentOutside.AmountSent)}, @{nameof(SentOutside.AmountSold)}, @{nameof(SentOutside.Location)}, @{nameof(SentOutside.Old)});";
            _dataAccess.Execute(cmd, param);
            int id = LastInput();
            param.ID = id;
            return param;
        }

        public void Update(SentOutside param)
        {
            string cmd = $"UPDATE {nameof(SentOutside)} SET {nameof(SentOutside.AmountSent)} = @{nameof(SentOutside.AmountSent)}, {nameof(SentOutside.AmountSold)} = @{nameof(SentOutside.AmountSold)},{nameof(SentOutside.Old)} = @{nameof(SentOutside.Old)}  WHERE {nameof(SentOutside.ID)} = @{nameof(SentOutside.ID)};";
            _dataAccess.Execute(cmd, param);
        }

        public IEnumerable<SentOutside> SelectByInventoryID(LocalInventory inventory) {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ProductID)} = @{nameof(LocalInventory.ID)};";
            var sentOutsides = _dataAccess.ReadData<SentOutside>(cmd, inventory);
            return sentOutsides;
        }

        public IEnumerable<string> SelectAllLocations()
        {
            string cmd = $"SELECT {nameof(SentOutside.Location)} FROM {nameof(SentOutside)} GROUP BY {nameof(SentOutside.Location)};";
            var sentOutsides = _dataAccess.ReadData<string>(cmd);
            return sentOutsides;
        }

        public IEnumerable<SentOutside> SelectByLocation(string location)
        {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.Location)} = '{location}';";
            var sentOutsides = _dataAccess.ReadData<SentOutside>(cmd);
            return sentOutsides;
        }

        public IEnumerable<SentOutside> SelectByProduct(Product p)
        {
            string cmd = $"SELECT * FROM {nameof(SentOutside)} WHERE {nameof(SentOutside.ProductID)} = @{nameof(Product.ID)};";
            var sentOutsides = _dataAccess.ReadData<SentOutside>(cmd, p);
            return sentOutsides;
        }
    }
}
