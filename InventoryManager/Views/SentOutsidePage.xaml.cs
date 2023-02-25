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
using static System.Net.Mime.MediaTypeNames;

namespace InventoryManager.Views;

public partial class SentOutsidePage : Page, INotifyPropertyChanged, INavigationAware
{

    public SentOutsidePage(IDBSetup dBSetup)
    {
        DataContext = this;
        _dBSetup = dBSetup;
        outsideORM = _dBSetup.GetTable<SentOutsideORM>();
        productsORM = _dBSetup.GetTable<ProductsORM>();
        localORM = _dBSetup.GetTable<LocalInventoryORM>();
        InitializeComponent();
    }
    public ObservableCollection<string> Locations { get; } = new ObservableCollection<string>();
    public ObservableCollection<SentOutDisplay> Source { get; } = new ObservableCollection<SentOutDisplay>();
    private readonly SentOutsideORM outsideORM;
    private readonly ProductsORM productsORM;
    private readonly LocalInventoryORM localORM;
    private readonly IDBSetup _dBSetup;

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
    }


    void INavigationAware.OnNavigatedFrom()
    {
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
                OutDisplay.Inventory =  await localORM.GetByID(i.InventoryID);
                OutDisplay.Product =  await productsORM.GetByID(OutDisplay.Inventory.ProductID);
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
            if (e.Row.DataContext is SentOutDisplay l && e.EditingElement is TextBox tb && e.Column.Header is string header)
            {
                switch (header)
                {
                    case nameof(SentOutDisplay.Outside.AmountSold):
                        {
                            int.TryParse(tb.Text, out int a);
                            l.Outside.AmountSold = a;
                            break;
                        }
                    case nameof(SentOutDisplay.Outside.AmountSent):
                        {
                            int.TryParse(tb.Text, out int b);
                            l.Outside.AmountSent = b;
                            break;
                        }
                }
                try
                {
                    await outsideORM.Update(l.Outside);
                    l.OnPropertyChanged();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Error");
                }
            }
        }
    }
}