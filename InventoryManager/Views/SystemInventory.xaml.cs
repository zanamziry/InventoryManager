using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace InventoryManager.Views;

public partial class SystemInventory : Page, INotifyPropertyChanged, INavigationAware
{
    public SystemInventory(IDBSetup dBSetup,INavigationService navigationService, ISystemDataGather dataGather)
    {
        InitializeComponent();
        DataContext = this;
        _navigationService = navigationService;
        _dBSetup = dBSetup;
        _dataGather = dataGather;
        SystemORM = _dBSetup.GetTable<SystemProductsORM>();
    }
    
    private readonly INavigationService _navigationService;
    private readonly ISystemDataGather _dataGather;
    private readonly IDBSetup _dBSetup;
    private readonly SystemProductsORM SystemORM;

    public ObservableCollection<SystemProduct> SystemProducts { get; } = new ObservableCollection<SystemProduct>();
    public event PropertyChangedEventHandler PropertyChanged;

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

    private async void OnGetDataClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        DateTime d;
        if (SelectedDate.SelectedDate != null)
            d = SelectedDate.SelectedDate.Value;
        else d = DateTime.Now;
        try
        {
            await SystemORM.DeleteAll();
            SystemProducts.Clear();
            var s = JsonConvert.DeserializeObject<SystemAPI>(await _dataGather.getDataAsync(AgentID.Text, d));
            foreach (var i in s.list)
            {
                await SystemORM.Insert(i);
                SystemProducts.Add(i);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error Updating", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }

    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        SystemProducts.Clear();
        foreach (var item in await SystemORM.SelectAll())
            SystemProducts.Add(item);
    }

    void INavigationAware.OnNavigatedFrom()
    {
    }

}
