using SQLClient.Contracts;
using System.Threading.Tasks;

namespace InventoryManager.Contracts.Services {
    public interface IDBSetup {
        string ConnectionString { get; set; }
        IDataAccess dataAccess { get; }

        T GetTable<T>();
        void CreateTables();
        void DropTables();
        void InitializeDatabase();
    }
}