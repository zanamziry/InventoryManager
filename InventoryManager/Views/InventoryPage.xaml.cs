using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using InventoryManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace InventoryManager.Views;

public partial class InventoryPage : Page, INotifyPropertyChanged
{
    public InventoryPage(IDBSetup dBSetup)
    {
        InitializeComponent();
        DataContext = this;
        _dBSetup = dBSetup;
        InventoryORM = _dBSetup.GetTable<LocalInventoryORM>();
        updateList();
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<InventoryListItem> InventorySource { get; } = new ObservableCollection<InventoryListItem>();

    private readonly IDBSetup _dBSetup;
    public LocalInventoryORM InventoryORM { get; set; }
    async void updateList()
    {
        InventorySource.Clear();
        foreach (var item in await InventoryORM.SelectAll())
            InventorySource.Add(item);
    }
    async void AddProduct(Product value)
    {
        if (InventorySource.Contains(value))
            return;
        try
        {
            await InventoryORM.Insert(value);
            InventorySource.Add(value);
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
                if ((e.OriginalSource as FrameworkElement).DataContext is InventoryListItem p)
                {
                    await InventoryORM.Delete(p);
                    InventorySource.Remove(p);
                }
                break;
        }
    }
    private async void OnRemoveButtonClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        if(GridOfProducts.SelectedItem is InventoryListItem p)
        {
            await InventoryORM.Delete(p);
            InventorySource.Remove(p);
        }
    }
}
