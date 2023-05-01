using System.Collections.ObjectModel;
using System.ComponentModel;
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
    private Agent _selectedAgent;
    private DateTime lastUpdated;
    private bool _isLoading;

    public bool IsLoading
    {
        get { return _isLoading; }
        set { Set(ref _isLoading ,value); }
    }
    public bool IsSelected => SelectedAgent != null;
    public Agent SelectedAgent
    {
        get { return _selectedAgent; }
        set 
        { 
            Set(ref _selectedAgent, value);
            SaveSetting(_selectedAgent.ID, AgentSettingsKey);
            OnPropertyChanged(nameof(IsSelected));
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
    public ObservableCollection<Agent> Agents { get; } = new ObservableCollection<Agent>();
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

    private async void OnGetDataClicked(object sender, RoutedEventArgs e)
    {
        IsLoading = true;
        DateTime d;
        if (SelectedDate.SelectedDate != null)
            d = SelectedDate.SelectedDate.Value;
        else d = DateTime.Now;
        try
        {
            var s = JsonConvert.DeserializeObject<SystemAPI>(await _dataGather.GetInventoryAsync(SelectedAgent.ID, d));
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
            IsLoading = false;
        }
    }
    async void getAgents()
    {
        string savedID = GetSavedSetting(AgentSettingsKey);
        IEnumerable<Agent> listOfAgents = Enumerable.Empty<Agent>();
        string json = await _dataGather.GetAgentsAsync();
        if (json != null)
        {
            try
            {
                listOfAgents = JsonConvert.DeserializeObject<IEnumerable<Agent>>(json);
            }
            catch
            {
                MessageBox.Show("No Agents Found!");
                return;
            }
        }
        Agents.Clear();
        foreach (var agent in listOfAgents)
        {
            Agents.Add(agent);
            if (agent.ID == savedID)
            {
                SelectedAgent = agent;
            }
        }
    }
    async void INavigationAware.OnNavigatedTo(object parameter)
    {
        IsLoading = true;
        getAgents();
        DateTime.TryParse(GetSavedSetting(LastUpdatedSettingsKey), out DateTime d);
        LastUpdated = d;
        SystemProducts.Clear();
        foreach (var item in await SystemORM.SelectAll())
            SystemProducts.Add(item);
        IsLoading = false;

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
