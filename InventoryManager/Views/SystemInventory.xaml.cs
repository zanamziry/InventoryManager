﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Models;
using InventoryManager.Core.Services;
using Newtonsoft.Json;

namespace InventoryManager.Views;

public partial class SystemInventory : Page, INotifyPropertyChanged, INavigationAware
{
    public SystemInventory(IDBSetup dBSetup, ISystemDataGather dataGather, ILanguageSelectorService languageSelector)
    {
        DataContext = this;
        _dBSetup = dBSetup;
        _dataGather = dataGather;
        SystemORM = _dBSetup.GetTable<SystemProductsORM>();
        languageSelector.InitializeLanguage();
        FlowDirection = languageSelector.Flow;
        InitializeComponent();
    }
    readonly string AgentSettingsKey = "AgentID";
    readonly string LastUpdatedSettingsKey = "LastUpdate";
    private ServiceCenter _selectedAgent;
    private DateTime? lastUpdated;
    private bool _isLoading;

    public bool IsLoading
    {
        get { return _isLoading; }
        set { Set(ref _isLoading ,value); }
    }
    public bool IsSelected => SelectedAgent != null;
    public ServiceCenter SelectedAgent
    {
        get { return _selectedAgent; }
        set 
        { 
            Set(ref _selectedAgent, value);
            SaveSetting(_selectedAgent.id, AgentSettingsKey);
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    public DateTime? LastUpdated
    {
        get
        {
            if (lastUpdated == null)
            {
                if (App.Current.Properties.Contains(LastUpdatedSettingsKey))
                {
                    if(long.TryParse(App.Current.Properties[LastUpdatedSettingsKey].ToString(), out long dateticks))
                        lastUpdated = new DateTime(dateticks);
                    else
                        lastUpdated = null;
                }
            }
            return lastUpdated;
        }
        set
        {
            if (value == null)
                return;
            Set(ref lastUpdated ,value);
            App.Current.Properties[LastUpdatedSettingsKey] = value.Value.Ticks.ToString();
        }
    }



    private readonly ISystemDataGather _dataGather;
    private readonly IDBSetup _dBSetup;
    private readonly SystemProductsORM SystemORM;

    public ObservableCollection<SystemProduct> SystemProducts { get; } = new ObservableCollection<SystemProduct>();
    public ObservableCollection<ServiceCenter> Agents { get; } = new ObservableCollection<ServiceCenter>();
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

    private void OnGetDataClicked(object sender, RoutedEventArgs e)
    {
        DateTime d;
        if (SelectedDate.SelectedDate != null)
            d = SelectedDate.SelectedDate.Value;
        else d = DateTime.Now;
        Task.Run(async () =>
        {
            IsLoading = true;
            try
            {
                var s = await _dataGather.GetInventoryAsync(SelectedAgent.id, d);
                if (s != null && s.list != null && s.list.Any())
                {
                    await SystemORM.DeleteAll();
                    Dispatcher.Invoke(() =>
                            SystemProducts.Clear());
                    foreach (var i in s.list)
                    {
                        await SystemORM.Insert(i);
                        Dispatcher.Invoke(() =>
                                SystemProducts.Add(i));
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
                IsLoading = false;
            }
        });
    }
    async Task getCenters()
    {
        string savedID = GetSavedSetting(AgentSettingsKey);
        IEnumerable<ServiceCenter> listOfAgents = await _dataGather.GetServiceCentersAsync();
        if (!listOfAgents.Any())
            return;
        Dispatcher.Invoke(() =>
            Agents.Clear());
        foreach (var agent in listOfAgents)
        {
            Dispatcher.Invoke(() =>
                Agents.Add(agent));
            if (agent.id == savedID)
            {
                SelectedAgent = agent;
            }
        }
    }
    void INavigationAware.OnNavigatedTo(object parameter)
    {
        SystemProducts.Clear();
        Task.Run(async () =>
        {
            foreach (var item in await SystemORM.SelectAll())
                Dispatcher.Invoke(() =>
                    SystemProducts.Add(item));
        });
        Task.Run(async () =>
        await getCenters());
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
