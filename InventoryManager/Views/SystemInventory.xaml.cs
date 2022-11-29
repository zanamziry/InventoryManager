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

    readonly string AgentSettingsKey = "AgentID";
    readonly string LastUpdatedSettingsKey = "LastUpdate";
    private string agentID;
    private DateTime lastUpdated;
    public string AgentID
    {
        get { return agentID; }
        set 
        { 
            Set(ref agentID, value);
            SaveSetting(agentID, AgentSettingsKey);
        }
    }

    public DateTime LastUpdated
    {
        get { return lastUpdated; }
        set 
        { 
            Set(ref lastUpdated ,value);
            SaveSetting(value.ToString(), LastUpdatedSettingsKey);
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
            var s = JsonConvert.DeserializeObject<SystemAPI>(await _dataGather.GetInventoryAsync(AgentID, d));
            if (s != null && s.list != null && s.list.Any())
            {
                await SystemORM.DeleteAll();
                SystemProducts.Clear();
                foreach (var i in s.list)
                {
                    await SystemORM.Insert(i);
                    SystemProducts.Add(i);
                }
                LastUpdated = d;
            }
            else
            {
                throw new Exception("Couldn't Load The Data Correctly, Please Try Again...");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error Updating", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Loading(false);
        }
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
        AgentID = GetSavedSetting(AgentSettingsKey);
        DateTime.TryParse(GetSavedSetting(LastUpdatedSettingsKey), out DateTime d);
        LastUpdated = d;
        SystemProducts.Clear();
        foreach (var item in await SystemORM.SelectAll())
            SystemProducts.Add(item);
        Loading(false);
    }
    private string GetSavedSetting(string key)
    {
        if (App.Current.Properties.Contains(key))
        {
            string savedString = App.Current.Properties[key].ToString();
            return savedString;
        }
        return "";
    }
    private void SaveSetting(string val,string key)
    {
        if(!string.IsNullOrWhiteSpace(val))
            App.Current.Properties[key] = val;
    }
    void INavigationAware.OnNavigatedFrom()
    {
    }

}
