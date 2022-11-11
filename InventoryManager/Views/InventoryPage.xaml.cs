using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using Microsoft.Data.Sqlite;

namespace InventoryManager.Views;

public partial class InventoryPage : Page, INotifyPropertyChanged, INavigationAware
{
    public InventoryPage(IDBSetup dBSetup)
    {
        InitializeComponent();
        DataContext = this;
        _dBSetup = dBSetup;
        InventoryORM = _dBSetup.GetTable<LocalInventoryORM>();
    }
    
    private readonly LocalInventoryORM InventoryORM;
    private readonly IDBSetup _dBSetup;
    public event PropertyChangedEventHandler PropertyChanged;
    public Product SelectedProduct { get; set; }
    public int System { get; set; } = 0;
    public int Outside { get; set; } = 0;
    public int GivenAway { get; set; } = 0;
    public int Result => Real + Outside + GivenAway - System;
    public int Real
    {
        get
        {
            int result = 0;
            foreach (var i in InventoryList)
            {
                result += i.Open + i.Inventory;
            }
            return result;
        }
    }
    public ObservableCollection<LocalInventory> InventoryList { get; set; } = new ObservableCollection<LocalInventory>();

    async void AddInventory(LocalInventory value)
    {
        if (InventoryList.Contains(value))
            return;
        try
        {
            await InventoryORM.Insert(value);
            InventoryList.Add(value);
        }
        catch (SqliteException ex)
        {
            MessageBox.Show(ex.Message,"Database Error!",MessageBoxButton.OK,MessageBoxImage.Error);
        }
    }

    bool CanAdd() =>
        Regex.IsMatch(InventoryAmount.Text, "^[0-9]") && Regex.IsMatch(OpenAmount.Text, "^[0-9]");

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
        if (CanAdd())
        {
            AddInventory(new LocalInventory {ProductID = SelectedProduct.ID, Inventory = int.Parse(InventoryAmount.Text), Open = int.Parse(OpenAmount.Text), ExpireDate = DateTime.Parse(ProductExpire.Text)});
        }
    }
    private async void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Delete:
                if ((e.OriginalSource as FrameworkElement).DataContext is LocalInventory p)
                {
                    await InventoryORM.Delete(p);
                    InventoryList.Remove(p);
                }
                break;
        }
    }
    private async void OnRemoveButtonClicked(object sender, RoutedEventArgs e)
    {
        if(GridOfInventory.SelectedItem is LocalInventory p)
        {
            await InventoryORM.Delete(p);
            InventoryList.Remove(p);
        }
    }

    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        if (parameter is Product p)
            SelectedProduct = p;
        InventoryList.Clear();
        foreach (var item in await InventoryORM.SelectAll())
            InventoryList.Add(item);
    }

    void INavigationAware.OnNavigatedFrom()
    {
        
    }
}
