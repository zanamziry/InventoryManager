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

namespace InventoryManager.Views;

public partial class GiveAwayPage : Page, INotifyPropertyChanged, INavigationAware
{

    public GiveAwayPage(IDBSetup dBSetup, ILanguageSelectorService languageSelector)
    {
        DataContext = this;
        _dBSetup = dBSetup;
        giveawayORM = _dBSetup.GetTable<GivenAwayORM>();
        productsORM = _dBSetup.GetTable<ProductsORM>();
        FlowDirection = languageSelector.Flow;
        InitializeComponent();
    }
    public FlowDirection Direction => CultureInfo.CurrentCulture == CultureInfo.GetCultureInfo("ar") ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
    public ObservableCollection<GivenAway> Events { get; } = new ObservableCollection<GivenAway>();
    public ObservableCollection<GiftDisplay> SelectedGiveAways { get; } = new ObservableCollection<GiftDisplay>();
    public List<Product> Products { get; private set; } = new List<Product>();

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
        foreach (var item in await giveawayORM.SelectAllEvents())
        {
            Events.Add(item);
        }
        Products.Clear();
        foreach(var i in await productsORM.SelectAll())
        {
            Products.Add(i);
        }
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
            foreach (var i in await giveawayORM.SelectByEvent(s))
            {
                var giftDisplay = new GiftDisplay
                {
                    GivenAway = i,
                    Product = Products.Find(o => o.ID == i.ProductID),
                };
                SelectedGiveAways.Add(giftDisplay);
            }
        }
    }
    async void Remove(GiftDisplay p)
    {
        await giveawayORM.Delete(p.GivenAway);
        SelectedGiveAways.Remove(p);
    }

    void OnKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Delete:
                if ((e.OriginalSource as FrameworkElement).DataContext is GiftDisplay p)
                {
                    Remove(p);
                }
                break;
        }
    }

}