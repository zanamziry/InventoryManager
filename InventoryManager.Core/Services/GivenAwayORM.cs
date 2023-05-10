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
            $"{nameof(GivenAway.ProductID)} TEXT NOT NULL," +
            $"{nameof(GivenAway.Amount)} INTEGER NOT NULL DEFAULT 1," +
            $"{nameof(GivenAway.Event)} TEXT NOT NULL," +
            $"{nameof(GivenAway.Date)} TEXT NOT NULL," +
            //$"FOREIGN KEY({nameof(GivenAway.ProductID)}) REFERENCES {nameof(Product)}({nameof(Product.ID)}) ON DELETE CASCADE," +
            $"PRIMARY KEY({nameof(GivenAway.ID)} AUTOINCREMENT));";
            _dataAccess.Execute(cmd);
        }

        public override void Delete(GivenAway param) {
            string cmd = $"DELETE FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.ID)} = @{nameof(GivenAway.ID)};";
            _dataAccess.Execute(cmd, param);
        }

        public override GivenAway Insert(GivenAway param) {
            string cmd = $"INSERT INTO {nameof(GivenAway)} ({nameof(GivenAway.ProductID)}, {nameof(GivenAway.Amount)}, {nameof(GivenAway.Event)}, {nameof(GivenAway.Date)}) values(@{nameof(GivenAway.ProductID)}, @{nameof(GivenAway.Amount)}, @{nameof(GivenAway.Event)}, '{param.Date.ToShortDateString()}');";
            _dataAccess.Execute(cmd, param);
            int id = LastInput();
            param.ID = id;
            return param;
        }

        public IEnumerable<GivenAway> SelectByProduct(Product p) {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.ProductID)} = @{nameof(Product.ID)};";
            var products = _dataAccess.ReadData<GivenAway>(cmd, p);
            return products;
        }

        public IEnumerable<GivenAway> SelectAllEvents()
        {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} GROUP BY {nameof(GivenAway.Event)}, {nameof(GivenAway.Date)};";
            var products = _dataAccess.ReadData<GivenAway>(cmd);
            return products;
        }

        public IEnumerable<GivenAway> SelectByEvent(GivenAway giveaway)
        {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.Event)} = @{nameof(giveaway.Event)} AND {nameof(GivenAway.Date)} = '{giveaway.Date.ToShortDateString()}';";
            var products = _dataAccess.ReadData<GivenAway>(cmd, giveaway);
            return products;
        }

        public int SelectTotalAmount(Product p)
        {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.ProductID)} = @{nameof(Product.ID)};";
            var givenAways = _dataAccess.ReadData<GivenAway>(cmd, p);
            int total = 0;
            foreach (var i in givenAways) {
                total += i.Amount;
            }
            return total;
        }
    }
}
