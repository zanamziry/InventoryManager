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
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace InventoryManager.Views;

public partial class InventoryPage : Page, INotifyPropertyChanged, INavigationAware
{

    public InventoryPage(IDBSetup dBSetup, ILanguageSelectorService languageSelector)
    {
        _dBSetup = dBSetup;
        InventoryORM = _dBSetup.GetTable<LocalInventoryORM>();
        outsideORM = _dBSetup.GetTable<SentOutsideORM>();
        givenAwayORM = _dBSetup.GetTable<GivenAwayORM>();
        _languageSelector = languageSelector;
        _languageSelector.InitializeLanguage();
        FlowDirection = _languageSelector.Flow;
        InitializeComponent();
    }

    private IList<MainInventory> Inventories = new List<MainInventory>();
    private ILanguageSelectorService _languageSelector;
    private readonly LocalInventoryORM InventoryORM;
    private readonly SentOutsideORM outsideORM;
    private readonly GivenAwayORM givenAwayORM;
    private readonly IDBSetup _dBSetup;
    private RelayCommand gotoNext;
    private RelayCommand gotoPrevious;
    private MainInventory _selectedProduct;
    private bool isButtonEnabled;
    public string[] Agents { get; } =
    {
        "Duhok",
        "Arbil",
        "Sulaimania",
        "Kirkuk",
        "Mosul",
        "Al-Adhamiya",
        "Mammon",
        "Dora",
        "Maysan",
        "Najaf",
        "Basra",
        "Salah Al-Din",
        "Karbala",
        "Diyala",
        "Babl",
        "Fallujah",
        "Diqar",
        "Al-Ramadi",
        };

    public LocalInventory SelectedInv
    {
        get
        {
            if (GridOfInventory.SelectedItem is LocalInventory l)
                return l;
            return null;
        }
    }
    public bool IsButtonEnabled
    {
        get { return isButtonEnabled; }
        set { Set(ref isButtonEnabled, value); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public RelayCommand GotoNext => gotoNext ??= new RelayCommand(Next, CanNext);
    public RelayCommand GotoPrevious => gotoPrevious ??= new RelayCommand(Previous, CanPrevious);

    public MainInventory SelectedProduct
    {
        get { return _selectedProduct; }
        set { Set(ref _selectedProduct, value); }
    }
    void UpdateUI()
    {
        GotoNext.OnCanExecuteChanged();
        GotoPrevious.OnCanExecuteChanged();
        SelectedProduct.OnPropertyChanged();
        IsButtonEnabled = SelectedProduct.Locals.Count > 0 && SelectedInv != null;
    }
    bool CanNext()
    {
        if (Inventories.Count > 1 && Inventories.Last() != SelectedProduct)
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
        await Task.Run(async () =>
        {
            foreach (var item in await InventoryORM.SelectProduct(SelectedProduct.Product))
                await Dispatcher.BeginInvoke(() =>
                SelectedProduct.Locals.Add(item));
        });
        UpdateUI();
    }
    bool CanPrevious()
    {
        if (Inventories.Count > 1 && Inventories.First() != SelectedProduct)
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
        await Task.Run(async () =>
        {
            foreach (var item in await InventoryORM.SelectProduct(SelectedProduct.Product))
                await Dispatcher.BeginInvoke(() =>
                    SelectedProduct.Locals.Add(item));
        });
        UpdateUI();
    }
    async Task AddInventory(LocalInventory value)
    {
        if (SelectedProduct.Locals.Contains(value))
            return;
        try
        {
            SelectedProduct.Locals.Add(await InventoryORM.Insert(value));
            UpdateUI();
        }
        catch (SqliteException ex)
        {
            MessageBox.Show(ex.Message, "Database Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    bool CanAdd() =>
        Regex.IsMatch(InventoryAmount.Text, "^[0-9]") && Regex.IsMatch(OpenAmount.Text, "^[0-9]");

    void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }


    async Task Remove(LocalInventory p)
    {
        await InventoryORM.Delete(p);
        SelectedProduct.Locals.Remove(p);
        UpdateUI();
    }

    void INavigationAware.OnNavigatedTo(object parameter)
    {
        if ((parameter as dynamic).SelectedProduct is MainInventory p && (parameter as dynamic).Source is ObservableCollection<MainInventory> s)
        {
            SelectedProduct = p;
            Inventories = s.ToList();
            UpdateUI();
        }
    }


    void INavigationAware.OnNavigatedFrom()
    {
    }

    async void OnCellEdited(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Cancel && e.Cancel == false)
        {
            await Remove(e.Row.DataContext as LocalInventory);
        }
        if (e.EditAction == DataGridEditAction.Commit && e.Cancel == false)
        {
            if (e.Row.DataContext is LocalInventory l)
            {
                if (e.EditingElement is TextBox tb)
                    switch (e.Column.SortMemberPath)
                    {
                        case nameof(LocalInventory.Inventory):
                            {
                                if (int.TryParse(tb.Text, out int a))
                                    l.Inventory = a;
                                else l.Inventory = 0;
                                break;
                            }
                        case nameof(LocalInventory.Open):
                            {
                                if (int.TryParse(tb.Text, out int b))
                                    l.Open = b;
                                else l.Open = 0;
                                break;
                            }
                        case nameof(LocalInventory.Note):
                            {
                                l.Note = tb.Text;
                                break;
                            }
                    }
                try
                {
                    await InventoryORM.Update(l);
                    UpdateUI();
                    int i = SelectedProduct.Locals.IndexOf(l);
                    SelectedProduct.Locals[i].OnRealChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database Error");
                }
            }
        }
    }
    void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    async void OnRemoveButtonClicked(object sender, RoutedEventArgs e)
    {
        if (GridOfInventory.SelectedItem is LocalInventory p)
        {
            await Remove(p);
        }
    }

    async void OnAddButtonClicked(object sender, RoutedEventArgs e)
    {
        if (CanAdd())
        {
            var newLocal = new LocalInventory { ProductID = SelectedProduct.Product.ID, Inventory = int.Parse(InventoryAmount.Text), Open = int.Parse(OpenAmount.Text), ExpireDate = ProductExpire.SelectedDate };
            await AddInventory(newLocal);
            ToggleAdd.IsChecked = false;
        }
    }

    void OnCancelAddClicked(object sender, RoutedEventArgs e)
    {
        OpenAmount.Text = "0";
        ToggleAdd.IsChecked = false;
    }

    async void OnGiveAwayClicked(object sender, RoutedEventArgs e)
    {
        if (!GiveAwayDate.SelectedDate.HasValue)
            GiveAwayDate.SelectedDate = DateTime.Now;
        var giveaway = new GivenAway
        {
            Amount = int.Parse(AmountToGive.Text),
            ProductID = SelectedProduct.Product.ID,
            Event = GiveAwayName.Text,
            Date = (DateTime)GiveAwayDate.SelectedDate,
        };
        await givenAwayORM.Insert(giveaway);
        SelectedProduct.GivenAways.Add(giveaway);
        AmountToGive.Text = "1";
        GiveAwayName.Text = "";
        ToggleGift.IsChecked = false;
        UpdateUI();
    }

    void OnCancelGiveawayClicked(object sender, RoutedEventArgs e)
    {
        AmountToGive.Text = "1";
        GiveAwayName.Text = "";
        ToggleGift.IsChecked = false;
    }

    async void OnSendClicked(object sender, RoutedEventArgs e)
    {
        var outside = new SentOutside
        {
            AmountSent = int.Parse(AmountToSend.Text),
            AmountSold = 0,
            ProductID = SelectedProduct.Product.ID,
            Location = PlaceToSend.Text,
            Old = IsOld.IsChecked.Value
        };
        await outsideORM.Insert(outside);
        SelectedProduct.SentOutsides.Add(outside);
        AmountToSend.Text = "0";
        ToggleSend.IsChecked = false;
        UpdateUI();
    }
    void OnCancelSendClicked(object sender, RoutedEventArgs e)
    {
        AmountToSend.Text = "0";
        ToggleSend.IsChecked = false;
    }

    void AmountToSend_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out int r);
    }

    void OpenAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out int r);
    }

    void InventoryAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out int r);
    }

    void AmountToGive_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out int r);
    }

    private void GridOfInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateUI();
    }

    private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if(sender is DatePicker dateSelected && dateSelected.DataContext is LocalInventory localInventory)
        {
            if (dateSelected.SelectedDate == localInventory.ExpireDate)
                return;
            int i = SelectedProduct.Locals.IndexOf(localInventory);
            localInventory.ExpireDate = dateSelected.SelectedDate;
            SelectedProduct.Locals[i].ExpireDate = localInventory.ExpireDate;
            try
            {
                await InventoryORM.Update(localInventory);
                SelectedProduct.Locals[i].OnRealChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error");
            }
        }
    }
}