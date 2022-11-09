using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        updateList();
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<Product> ProductList { get; } = new ObservableCollection<Product>();

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
    bool canAdd()
    {
        if (string.IsNullOrEmpty(ProductName.Text) || string.IsNullOrEmpty(ProductCode.Text) || string.IsNullOrEmpty(ProductPrice.Text))
            return false;
        return true;
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

    private void OnAddButtonClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        if (canAdd())
        {
            if (decimal.TryParse(ProductPrice.Text, out decimal price))
                AddProduct(new Product { ID = ProductCode.Text, Name = ProductName.Text, Price = price });
        }
    }
    private async void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Delete:
                if ((e.OriginalSource as FrameworkElement).DataContext is Product p)
                {
                    await ProductsORM.Delete(p);
                    ProductList.Remove(p);
                }
                break;
        }
    }
    private async void OnRemoveButtonClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        if(GridOfProducts.SelectedItem is Product p)
        {
            await ProductsORM.Delete(p);
            ProductList.Remove(p);
        }
    }
}
