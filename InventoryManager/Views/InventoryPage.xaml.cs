using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlzEx.Standard;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using InventoryManager.Helpers;
using InventoryManager.Models;
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
    private IList<MainInventory> Inventories = new List<MainInventory>();
    private readonly LocalInventoryORM InventoryORM;
    private readonly IDBSetup _dBSetup;
    private ICommand gotoNext;
    private ICommand gotoPrevious;

    public ICommand GotoNext => gotoNext ??= new RelayCommand(Next,CanNext);
    public ICommand GotoPrevious => gotoPrevious ??= new RelayCommand(Previous, CanPrevious);

    bool CanNext()
    {
        if (Inventories.Count() > 1 && Inventories.Last() != SelectedProduct)
            return true;
        return false;
    }
    
    async void Next()
    {
        int s = Inventories.IndexOf(SelectedProduct);
        if (s == -1)
            return;
        s += 1;
        SelectedProduct = Inventories[s];
        SelectedProduct.Locals.Clear();
        foreach (var item in await InventoryORM.SelectProduct(SelectedProduct.Product))
            SelectedProduct.Locals.Add(item);
    }
    bool CanPrevious()
    {
        if (Inventories.Count() > 1 && Inventories.First() != SelectedProduct)
            return true;
        return false;
    }

    async void Previous()
    {
        int s = Inventories.IndexOf(SelectedProduct);
        if (s == -1)
            return;
        s -= 1;
        SelectedProduct = Inventories[s];
        SelectedProduct.Locals.Clear();
        foreach (var item in await InventoryORM.SelectProduct(SelectedProduct.Product))
            SelectedProduct.Locals.Add(item);
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private MainInventory _selectedProduct;
    public MainInventory SelectedProduct
    {
        get { return _selectedProduct; }
        set { Set(ref _selectedProduct ,value); }
    }

    async void AddInventory(LocalInventory value)
    {
        if (SelectedProduct.Locals.Contains(value))
            return;
        try
        {
            await InventoryORM.Insert(value);
            SelectedProduct.Locals.Add(value);
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
            AddInventory(new LocalInventory {ProductID = SelectedProduct.Product.ID, Inventory = int.Parse(InventoryAmount.Text), Open = int.Parse(OpenAmount.Text), ExpireDate = r});
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
        SelectedProduct.Locals.Remove(p);
    }

    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        if ((parameter as dynamic).SelectedProduct is MainInventory p && (parameter as dynamic).Source is ObservableCollection<MainInventory> s)
        {
            Inventories = s.ToList();
            SelectedProduct = p;
            SelectedProduct.Locals.Clear();
            SelectedProduct.Locals.CollectionChanged += Locals_CollectionChanged;
            foreach (var item in await InventoryORM.SelectProduct(SelectedProduct.Product))
                SelectedProduct.Locals.Add(item);
        }
    }

    private void Locals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        SelectedProduct.OnPropertyChanged();
    }

    void INavigationAware.OnNavigatedFrom()
    {
        SelectedProduct.Locals.CollectionChanged -= Locals_CollectionChanged;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {

    }
}