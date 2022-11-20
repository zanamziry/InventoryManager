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
using InventoryManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace InventoryManager.Views;

public partial class SystemInventory : Page, INotifyPropertyChanged, INavigationAware
{
    public SystemInventory(IDBSetup dBSetup, ISystemDataGather dataGather)
    {
        InitializeComponent();
        DataContext = this;
        _dBSetup = dBSetup;
        _dataGather = dataGather;
        SystemORM = _dBSetup.GetTable<SystemProductsORM>();
    }

    readonly string SettingsKey = "AgentID";
    private string _agentId;
    public string AgentID
    {
        get { return _agentId; }
        set 
        { 
            Set(ref _agentId, value);
            SaveAgentID(_agentId);
        }
    }

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
        Loading(true);
        DateTime d;
        if (SelectedDate.SelectedDate != null)
            d = SelectedDate.SelectedDate.Value;
        else d = DateTime.Now;
        try
        {
            await SystemORM.DeleteAll();
            SystemProducts.Clear();
            var s = JsonConvert.DeserializeObject<SystemAPI>(await _dataGather.GetInventoryAsync(AgentID, d));
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
        Loading(false);
    }
    void Loading(bool val)
    {
        if(val == true)
        {
            loadingBackground.Visibility = Visibility.Visible;
            loadingCircle.IsLoading = true;
        }
        else
        {
            loadingBackground.Visibility = Visibility.Collapsed;
            loadingCircle.IsLoading = false;
        }
    }
    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        Loading(true);
        AgentID = GetOldID();
        SystemProducts.Clear();
        foreach (var item in await SystemORM.SelectAll())
            SystemProducts.Add(item);
        Loading(false);
    }
    private string GetOldID()
    {
        if (App.Current.Properties.Contains(SettingsKey))
        {
            string Agent = App.Current.Properties[SettingsKey].ToString();
            return Agent;
        }
        return "";
    }
    private void SaveAgentID(string val)
    {
        if(!string.IsNullOrWhiteSpace(val))
            App.Current.Properties[SettingsKey] = val;
    }
    void INavigationAware.OnNavigatedFrom()
    {
    }

}
