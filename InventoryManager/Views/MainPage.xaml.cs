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
    public MainPage(IDBSetup dBSetup,INavigationService navigationService)
    {
        InitializeComponent();
        DataContext = this;
        _navigationService = navigationService;
        _dBSetup = dBSetup;
        ProductsORM = _dBSetup.GetTable<ProductsORM>();
        LocalORM = _dBSetup.GetTable<LocalInventoryORM>();
        SystemORM = _dBSetup.GetTable<SystemProductsORM>();
        GivenORM = _dBSetup.GetTable<GivenAwayORM>();
        OutsideORM = _dBSetup.GetTable<SentOutsideORM>();
    }
    
    private readonly INavigationService _navigationService;
    private readonly IDBSetup _dBSetup;
    private readonly SystemProductsORM SystemORM;
    private readonly ProductsORM ProductsORM;
    private readonly GivenAwayORM GivenORM;
    private readonly LocalInventoryORM LocalORM;
    private readonly SentOutsideORM OutsideORM;

    public ObservableCollection<MainInventory> Source { get; } = new ObservableCollection<MainInventory>();
    public event PropertyChangedEventHandler PropertyChanged;

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

    private async void OnAddButtonClicked(object sender, RoutedEventArgs e)
    {
        if (canAdd())
        {
            if (decimal.TryParse(ProductPrice.Text, out decimal price))
            {
                var p = new Product { ID = ProductCode.Text, Name = ProductName.Text, Price = price };
                try
                {
                    await ProductsORM.Insert(p);
                    AddToView(p);
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
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
                    var ns = Source.Where(o => o.Product == p);
                    if (ns.Count() < 1)
                        return;
                    Source.Remove(ns.First());
                }
                break;
        }
    }
    private async void OnRemoveButtonClicked(object sender, RoutedEventArgs e)
    {
        if(GridOfProducts.SelectedItem is Product p)
        {
            await ProductsORM.Delete(p);
            var ns = Source.Where(o => o.Product == p);
            if (ns.Count() < 1)
                return;
            Source.Remove(ns.First());
        }
    }
    private async void OnRemoveAllButtonClicked(object sender, RoutedEventArgs e)
    {
        await ProductsORM.DeleteAll();
        Source.Clear();
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
                if (Source.Where(o => o.Product == product).Count() == 0)
                {
                    try
                    {
                        await ProductsORM.Insert(product);
                        AddToView(product);
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
            List<Product> products = new List<Product>();
            string filename = dialog.FileName;
            foreach (var item in Source)
            {
                products.Add(item.Product);
            }
            string stringJson = JsonConvert.SerializeObject(products);
            await File.WriteAllTextAsync(filename, stringJson);
        }
    }

    private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if(e.OriginalSource is FrameworkElement FE && FE.DataContext is MainInventory SelectedProduct)
        {
            _navigationService.NavigateTo(typeof(InventoryPage), SelectedProduct);
        }
    }
    async void AddToView(Product p)
    {
        var localDB = await LocalORM.SelectProduct(p);
        var sys = await SystemORM.SelectProduct(p);
        var totalGiven = 0;
        var totalOut = 0;
        foreach (var item in localDB)
        {
            totalGiven += await GivenORM.SelectTotalAmount(item);
            totalOut += await OutsideORM.SelectTotalAmountSent(item);
        }
        Source.Add(new MainInventory
        {
            Product = p,
            System = sys,
            TotalGivenAway = totalGiven,
            TotalOutside = totalOut,
        });

    }
    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        Source.Clear();
        foreach (var p in await ProductsORM.SelectAll())
            AddToView(p);
    }

    void INavigationAware.OnNavigatedFrom()
    {
    }
}
