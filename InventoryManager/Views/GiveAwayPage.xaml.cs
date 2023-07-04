using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using InventoryManager.Models;
using NuGet;

namespace InventoryManager.Views;

public partial class GiveAwayPage : Page, INotifyPropertyChanged, INavigationAware
{

    public GiveAwayPage(IDBSetup dBSetup, ILanguageSelectorService languageSelector)
    {
        DataContext = this;
        _dBSetup = dBSetup;
        giveawayORM = _dBSetup.GetTable<GivenAwayORM>();
        productsORM = _dBSetup.GetTable<ProductsORM>();
        languageSelector.InitializeLanguage();
        FlowDirection = languageSelector.Flow;
        InitializeComponent();
    }

    public decimal TotalPV => SelectedGiveAways.Sum(o => o.Product.PV * o.GivenAway.Amount); 

    public ObservableCollection<GivenAway> Events { get; } = new ObservableCollection<GivenAway>();
    public ObservableCollection<GiftDisplay> SelectedGiveAways { get; } = new ObservableCollection<GiftDisplay>();
    public List<Product> Products { get; private set; } = new List<Product>();
    public List<GivenAway> AllGiveAways { get; private set; } = new List<GivenAway>();

    private readonly GivenAwayORM giveawayORM;
    private readonly ProductsORM productsORM;
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
        Events.Clear();
        Products.Clear();
        await Task.Run(async () =>
        {
            Products = new List<Product>(await productsORM.SelectAll());
            AllGiveAways = new List<GivenAway>(await giveawayORM.SelectAll());
            foreach (var item in await giveawayORM.SelectAllEvents())
            {
                Dispatcher.Invoke(() =>
                    Events.Add(item));
            }
        });
        OnPropertyChanged(nameof(TotalPV));
    }

    void INavigationAware.OnNavigatedFrom()
    {
        
    }

    void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView listView && listView.SelectedItem is GivenAway s)
        {
            SelectedGiveAways.Clear();
            await Task.Run(() =>
            {
                foreach (var i in AllGiveAways.Where(o=>o.Date == s.Date && o.Event == s.Event))
                {
                    var giftDisplay = new GiftDisplay
                    {
                        GivenAway = i,
                        Product = Products.Find(o => o.ID == i.ProductID),
                    };
                    Dispatcher.Invoke(() =>
                        SelectedGiveAways.Add(giftDisplay));
                }
            });
            OnPropertyChanged(nameof(TotalPV));
        }
    }
    async Task Remove(GiftDisplay p)
    {
        await giveawayORM.Delete(p.GivenAway);
        SelectedGiveAways.Remove(p);
        AllGiveAways.Remove(p.GivenAway);
        if (SelectedGiveAways.Count < 1)
            Events.RemoveAll(o => o.Event == p.GivenAway.Event);
        OnPropertyChanged(nameof(TotalPV));
    }

    async void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Delete:
                if ((e.OriginalSource as FrameworkElement).DataContext is GiftDisplay p)
                {
                    await Remove(p);
                }
                break;
        }
    }

}