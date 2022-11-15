using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace InventoryManager.Views;

public partial class InventoryPage : Page, INotifyPropertyChanged, INavigationAware
{
    /*
     TODO: Things to add to InventoryPage
        - Next and Previous Product
     */

    public InventoryPage(IDBSetup dBSetup)
    {
        InitializeComponent();
        DataContext = this;
        _dBSetup = dBSetup;
        
        InventoryORM = _dBSetup.GetTable<LocalInventoryORM>();
    }
    
    private readonly LocalInventoryORM InventoryORM;
    private readonly GivenAwayORM GivenAwayORM;
    private readonly SentOutsideORM SentOutsideORM;
    private readonly IDBSetup _dBSetup;
    private int sysvalue;
    private int real;
    private int outside;
    private int giveaway;

    public ObservableCollection<LocalInventory> InventoryList { get; } = new ObservableCollection<LocalInventory>();
    public event PropertyChangedEventHandler PropertyChanged;
    public Product SelectedProduct { get; set; }
    
    public int SysValue
    {
        get { return sysvalue; }
        set { Set(ref sysvalue , value); }
    }

    public int Outside
    {
        get { return outside; }
        set { Set(ref outside, value); }
    }

    public int GiveAway
    {
        get { return giveaway; }
        set { Set(ref giveaway, value); }
    }

    public int Real
    {
        get { return real; }
        set 
        {
            int result = 0;
            foreach (var i in InventoryList)
            {
                result += i.Open + i.Inventory;
            }
            Set(ref real, result); 
        }
    }

    async void updateValues()
    {
        SysValue = 0;
        GiveAway = 0;
        Outside = 0;
        foreach (LocalInventory i in InventoryList)
        {
            GiveAway += await GivenAwayORM.SelectTotalAmount(i);
            Outside += await SentOutsideORM.SelectTotalAmount(i);
        }
        Real = 0;
    }

    public int Result => Real + Outside + GiveAway - SysValue;

    async void AddInventory(LocalInventory value)
    {
        if (InventoryList.Contains(value))
            return;
        try
        {
            await InventoryORM.Insert(value);
            InventoryList.Add(value);
            updateValues();
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
            DateTime.TryParse(ProductExpire.Text, out DateTime r);
            AddInventory(new LocalInventory {ProductID = SelectedProduct.ID, Inventory = int.Parse(InventoryAmount.Text), Open = int.Parse(OpenAmount.Text), ExpireDate = r});
            updateValues();
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Delete:
                if ((e.OriginalSource as FrameworkElement).DataContext is LocalInventory p)
                {
                    Remove(p);
                }
                break;
        }
    }

    private void OnRemoveButtonClicked(object sender, RoutedEventArgs e)
    {
        if(GridOfInventory.SelectedItem is LocalInventory p)
        {
            Remove(p);
        }
    }

    async void Remove(LocalInventory p)
    {
        await InventoryORM.Delete(p);
        InventoryList.Remove(p);
        updateValues();
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
