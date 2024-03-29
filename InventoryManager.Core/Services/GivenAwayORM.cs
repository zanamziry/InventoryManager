﻿using System;
using InventoryManager.Core.Models;
using SQLClient;
using SQLClient.Contracts;

namespace InventoryManager.Core.Services {
    public class GivenAwayORM : TableBase<GivenAway> {
        public GivenAwayORM(IDataAccess dataAccess) : base(dataAccess) {
        }

        public override async Task Create() {
            string cmd =
            $"CREATE TABLE IF NOT EXISTS {nameof(GivenAway)}(" +
            $"{nameof(GivenAway.ID)} INTEGER NOT NULL UNIQUE," +
            $"{nameof(GivenAway.ProductID)} TEXT NOT NULL," +
            $"{nameof(GivenAway.Amount)} INTEGER NOT NULL DEFAULT 1," +
            $"{nameof(GivenAway.Event)} TEXT NOT NULL," +
            $"{nameof(GivenAway.Date)} TEXT NOT NULL," +
            //$"FOREIGN KEY({nameof(GivenAway.ProductID)}) REFERENCES {nameof(Product)}({nameof(Product.ID)}) ON DELETE CASCADE," +
            $"PRIMARY KEY({nameof(GivenAway.ID)} AUTOINCREMENT));";
            await _dataAccess.ExecuteAsync(cmd);
        }

        public override async Task Delete(GivenAway param) {
            string cmd = $"DELETE FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.ID)} = @{nameof(GivenAway.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public override async Task<GivenAway> Insert(GivenAway param) {
            string cmd = $"INSERT INTO {nameof(GivenAway)} ({nameof(GivenAway.ProductID)}, {nameof(GivenAway.Amount)}, {nameof(GivenAway.Event)}, {nameof(GivenAway.Date)}) values(@{nameof(GivenAway.ProductID)}, @{nameof(GivenAway.Amount)}, @{nameof(GivenAway.Event)}, @{nameof(GivenAway.Date)});";
            await _dataAccess.ExecuteAsync(cmd, param);
            param.ID = await LastInput();
            return param;
        }

        public async Task Update(GivenAway param)
        {
            string cmd = $"UPDATE {nameof(GivenAway)} SET {nameof(GivenAway.Amount)} = @{nameof(GivenAway.Amount)} WHERE {nameof(GivenAway.ID)} = @{nameof(GivenAway.ID)};";
            await _dataAccess.ExecuteAsync(cmd, param);
        }

        public async Task<IEnumerable<GivenAway>> SelectByProduct(Product p) {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.ProductID)} = @{nameof(Product.ID)};";
            var products = await _dataAccess.ReadDataAsync<GivenAway>(cmd, p);
            return products;
        }

        public async Task<IEnumerable<GivenAway>> SelectAllEvents()
        {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} GROUP BY {nameof(GivenAway.Event)}, {nameof(GivenAway.Date)};";
            var products = await _dataAccess.ReadDataAsync<GivenAway>(cmd);
            return products;
        }

        public async Task<IEnumerable<GivenAway>> SelectByEvent(GivenAway giveaway)
        {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.Event)} = @{nameof(giveaway.Event)} AND {nameof(GivenAway.Date)} = @{nameof(GivenAway.Date)};";
            var products = await _dataAccess.ReadDataAsync<GivenAway>(cmd, giveaway);
            return products;
        }

        public async Task<int> SelectTotalAmount(Product p)
        {
            string cmd = $"SELECT * FROM {nameof(GivenAway)} WHERE {nameof(GivenAway.ProductID)} = @{nameof(Product.ID)};";
            var givenAways = await _dataAccess.ReadDataAsync<GivenAway>(cmd, p);
            int total = 0;
            foreach (var i in givenAways) {
                total += i.Amount;
            }
            return total;
        }
    }
}
