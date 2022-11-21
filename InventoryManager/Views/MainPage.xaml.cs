using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using InventoryManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InventoryManager.Views;

public partial class MainPage : Page, INotifyPropertyChanged, INavigationAware
{
    public MainPage(IDBSetup dBSetup,INavigationService navigationService, ISystemDataGather dataGather)
    {
        InitializeComponent();
        DataContext = this;
        _navigationService = navigationService;
        _dBSetup = dBSetup;
        _dataGather = dataGather;
        ProductsORM = _dBSetup.GetTable<ProductsORM>();
        SystemORM = _dBSetup.GetTable<SystemProductsORM>();
        GivenORM = _dBSetup.GetTable<GivenAwayORM>();
        OutsideORM = _dBSetup.GetTable<SentOutsideORM>();
    }
    
    private readonly INavigationService _navigationService;
    private readonly ISystemDataGather _dataGather;
    private readonly IDBSetup _dBSetup;
    private readonly SystemProductsORM SystemORM;
    private readonly ProductsORM ProductsORM;
    private readonly GivenAwayORM GivenORM;
    private readonly SentOutsideORM OutsideORM;

    public ObservableCollection<MainInventory> ProductList { get; } = new ObservableCollection<MainInventory>();
    public event PropertyChangedEventHandler PropertyChanged;

    async void AddProduct(Product value)
    {
        if (ProductList.Where(o => o.Product == value).Count() > 0)
            return;
        try
        {
            await ProductsORM.Insert(value);
            ProductList.Add(new MainInventory {Product = value });
        }
        catch (SqliteException ex)
        {
            MessageBox.Show(ex.Message,"Database Error!",MessageBoxButton.OK,MessageBoxImage.Error);
        }
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

    private void OnAddButtonClicked(object sender, RoutedEventArgs e)
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
                    var ns = ProductList.Where(o => o.Product == p);
                    if (ns.Count() < 1)
                        return;
                    ProductList.Remove(ns.First());
                }
                break;
        }
    }
    private async void OnRemoveButtonClicked(object sender, RoutedEventArgs e)
    {
        if(GridOfProducts.SelectedItem is Product p)
        {
            await ProductsORM.Delete(p);
            var ns = ProductList.Where(o => o.Product == p);
            if (ns.Count() < 1)
                return;
            ProductList.Remove(ns.First());
        }
    }
    private async void OnRemoveAllButtonClicked(object sender, RoutedEventArgs e)
    {
        await ProductsORM.DeleteAll();
        ProductList.Clear();
    }
    private async void OnImportClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.DefaultExt = ".json"; // Default file extension
        dialog.Filter = "Json files (*.json)|*.json"; // Filter files by extension
        var result = dialog.ShowDialog();
        if(result == true)
        {
            string filename = dialog.FileName;
            string json = await File.ReadAllTextAsync(filename);
            var obj = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
            foreach(Product product in obj)
            {
                if (ProductList.Where(o => o.Product == product).Count() < 1)
                {
                    try
                    {
                        await ProductsORM.Insert(product);
                        ProductList.Add(new MainInventory { Product = product });
                    }
                    catch (SqliteException ex)
                    {
                        MessageBox.Show(ex.Message, "Database Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
    private async void OnExportClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog();
        dialog.FileName = "InventoryProducts";
        dialog.DefaultExt = ".json"; // Default file extension
        dialog.Filter = "Json files (*.json)|*.json"; // Filter files by extension
        var result = dialog.ShowDialog();
        if (result == true)
        {
            string filename = dialog.FileName;

            string stringJson = JsonConvert.SerializeObject(ProductList);
            await File.WriteAllTextAsync(filename, stringJson);
        }
    }

    private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if(e.OriginalSource is FrameworkElement FE && FE.DataContext is Product SelectedProduct)
        {
            _navigationService.NavigateTo(typeof(InventoryPage), SelectedProduct);
        }
    }

    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        ProductList.Clear();
        foreach (var item in await ProductsORM.SelectAll())
            ProductList.Add(new MainInventory
            {
                System = await SystemORM.SelectProduct(item),
                Product = item,
                Given = GivenORM.SelectProduct()

            });
    }

    void INavigationAware.OnNavigatedFrom()
    {
    }
}
