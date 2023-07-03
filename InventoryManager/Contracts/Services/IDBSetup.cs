using SQLClient.Contracts;
using System.Threading.Tasks;

namespace InventoryManager.Contracts.Services {
    public interface IDBSetup {
        string ConnectionString { get; set; }

        T GetTable<T>();
        void CreateTables(IDataAccess dataAccess);
        void DropTables();
        void InitializeDatabase();
    }
}