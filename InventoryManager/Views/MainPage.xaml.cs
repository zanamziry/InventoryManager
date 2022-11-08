using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using InventoryManager.Contracts.Services;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;

namespace InventoryManager.Views;

public partial class MainPage : Page, INotifyPropertyChanged
{
    public MainPage(IDBSetup dBSetup)
    {
        InitializeComponent();
        DataContext = this;
        _dBSetup = dBSetup;
        ProductsORM = _dBSetup.GetTable<ProductsORM>();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<Product> ProductList { get; set; } = new ObservableCollection<Product>();

    private readonly IDBSetup _dBSetup;
    public ProductsORM ProductsORM { get; set; }
    async void updateList()
    {
        ProductList.Clear();
        foreach (var item in await ProductsORM.SelectAll())
        {
            ProductList.Add(item);
        }
    }
    async void AddProduct(Product value)
    {
        await ProductsORM.Insert(value);
        ProductList.Add(value);
    }
    bool canAdd(Product value)
    {
        if (value is Product product && product.ID != null && product.Name != null)
            return true;
        return false;
    }
    private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        updateList();
    }
}
