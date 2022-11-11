using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using Newtonsoft.Json;

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
    }
    
    public ObservableCollection<Product> ProductList { get; } = new ObservableCollection<Product>();
    private readonly INavigationService _navigationService;
    private readonly IDBSetup _dBSetup;
    public event PropertyChangedEventHandler PropertyChanged;
    private ProductsORM ProductsORM { get; }

    async void AddProduct(Product value)
    {
        if (ProductList.Contains(value))
            return;
        try
        {
            await ProductsORM.Insert(value);
            ProductList.Add(value);
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
                if (!ProductList.Contains(product))
                {
                    try
                    {
                        await ProductsORM.Insert(product);
                        ProductList.Add(product);
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
            ProductList.Add(item);
    }

    void INavigationAware.OnNavigatedFrom()
    {
    }
}
