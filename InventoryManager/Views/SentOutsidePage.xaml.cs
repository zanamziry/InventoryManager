using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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

namespace InventoryManager.Views;

public partial class SentOutsidePage : Page, INotifyPropertyChanged, INavigationAware
{

    public SentOutsidePage(IDBSetup dBSetup)
    {
        DataContext = this;
        _dBSetup = dBSetup;
        outsideORM = _dBSetup.GetTable<SentOutsideORM>();
        productsORM = _dBSetup.GetTable<ProductsORM>();
        InitializeComponent();
    }
    public ObservableCollection<string> Locations { get; } = new ObservableCollection<string>();
    public ObservableCollection<SentOutDisplay> Source { get; } = new ObservableCollection<SentOutDisplay>();
    private readonly SentOutsideORM outsideORM;
    private readonly ProductsORM productsORM;
    private readonly IDBSetup _dBSetup;

    public decimal TotalPrice
    {
        get
        {
            decimal res = 0;
            foreach (var item in Source)
            {
                if (item.Product == null || item.Outside == null)
                    continue;
                if (item.Outside.Old == true)
                    res += item.Product.Old_Price * item.Outside.AmountSold;
                else res += item.Product.Price * item.Outside.AmountSold;
            }
            return res;
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }
    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        Locations.Clear();
        foreach (var item in await outsideORM.SelectAllLocations())
        {
            Locations.Add(item);
        }
        Source.CollectionChanged += Source_CollectionChanged;
    }

    private void Source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TotalPrice));
    }

    void INavigationAware.OnNavigatedFrom()
    {
        Source.CollectionChanged -= Source_CollectionChanged;
    }

    void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView listView && listView.SelectedItem is string s)
        {
            Source.Clear();
            foreach (var i in await outsideORM.SelectByLocation(s))
            {
                SentOutDisplay OutDisplay = new SentOutDisplay();
                OutDisplay.Product =  await productsORM.GetByID(i.ProductID);
                OutDisplay.Outside =  i;
                Source.Add(OutDisplay);
            }
        }
    }
    async void Remove(SentOutDisplay p)
    {
        await outsideORM.Delete(p.Outside);
        Source.Remove(p);
    }

    void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Delete:
                if ((e.OriginalSource as FrameworkElement).DataContext is SentOutDisplay p)
                {
                    Remove(p);
                }
                break;
        }
    }
    async void OnCellEdited(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Cancel && e.Cancel == false)
        {
            Remove(e.Row.DataContext as SentOutDisplay);
        }
        if (e.EditAction == DataGridEditAction.Commit && e.Cancel == false)
        {
            if (e.Row.DataContext is SentOutDisplay l && e.Column.Header is string header)
            {
                switch (header)
                {
                    // Just in Case There are other columns other than this to edit
                    case nameof(SentOutDisplay.Outside.AmountSold):
                        {
                            if (e.EditingElement is TextBox tb)
                            {
                                int.TryParse(tb.Text, out int a);
                                l.Outside.AmountSold = a;
                            }
                            break;
                        }
                    case nameof(SentOutDisplay.Outside.Old):
                        {
                            if (e.EditingElement is CheckBox cb)
                            {
                                l.Outside.Old = cb.IsChecked.Value;
                            }
                            break;
                        }
                }
                try
                {
                    await outsideORM.Update(l.Outside);
                    l.OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Error");
                }
            }
        }
    }
}