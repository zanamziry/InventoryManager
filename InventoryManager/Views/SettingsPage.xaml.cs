using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Models;
using InventoryManager.Properties;
using InventoryManager.Properties.ar_iq;
using InventoryManager.Services;
using Microsoft.Extensions.Options;
using SQLClient;
using Squirrel;

namespace InventoryManager.Views;

public partial class SettingsPage : Page, INotifyPropertyChanged, INavigationAware
{
    Regex v = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
    private readonly AppConfig _appConfig;
    private readonly ISystemService _systemService;
    private readonly IApplicationInfoService _applicationInfoService;
    private readonly ISystemDataGather _dataGather;
    private AppTheme _theme;
    private IDBSetup _dBSetup;
    private ILanguageSelectorService _languageSelector;
    private Language _selectedLang;
    private string _serverAddress;
    private string _versionDescription;
    private string _password;
    private string _username;

    public ObservableCollection<Language> Languages { get; set; } = new ObservableCollection<Language>();
    public event PropertyChangedEventHandler PropertyChanged;
    public AppTheme Theme
    {
        get { return _theme; }
        set { Set(ref _theme, value); }
    }
    public string VersionDescription
    {
        get { return _versionDescription; }
        set { Set(ref _versionDescription, value); }
    }
    public string ServerAddress
    {
        get { return _serverAddress; }
        set 
        {
            if (string.IsNullOrEmpty(value) || !v.IsMatch(value))
                value = _dataGather.DEFAULT;
            Set(ref _serverAddress, value);
            _dataGather.BASE_URL = value;
        }
    }
    public string Username
    {
        get { return _username; }
        set 
        {
            Set(ref _username, value);
            _dataGather.Username = value;
        }
    }
    public string Password
    {
        get { return _password; }
        set 
        { 
            Set(ref _password ,value);
            _dataGather.Password = value;
        }
    }
    public Language SelectedLang
    {
        get { return _selectedLang; }
        set 
        { 
            Set(ref _selectedLang, value); 
            _languageSelector.SetLanguagePreferences(value);
        }
    }
    public SettingsPage(IOptions<AppConfig> appConfig, ISystemService systemService, IApplicationInfoService applicationInfoService, ISystemDataGather dataGather, IDBSetup dBSetup, ILanguageSelectorService languageSelector)
    {
        _appConfig = appConfig.Value;
        _systemService = systemService;
        _applicationInfoService = applicationInfoService;
        _dataGather = dataGather;
        _dBSetup = dBSetup;
        _languageSelector = languageSelector;
        languageSelector.InitializeLanguage();
        FlowDirection = languageSelector.Flow;
        InitializeComponent();
    }
    public void OnNavigatedTo(object parameter)
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
        ServerAddress = _dataGather.BASE_URL;
        Username = _dataGather.Username;
        Password = _dataGather.Password;
        passField.Password = Password;
        SelectedLang = _languageSelector.PreferedLang;
        Languages = new ObservableCollection<Language>(_languageSelector.Languages);
        OnPropertyChanged(nameof(Languages));
    }
    public void OnNavigatedFrom()
    {
    }

    private void OnPrivacyStatementClick(object sender, RoutedEventArgs e)
        => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);
    private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return;
        }

        storage = value;
        OnPropertyChanged(propertyName);
    }
    private void OnCheckUpdateClicked(object sender, RoutedEventArgs e)
    {
        UpdatingService.Update();
    }
    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    private void OnDeleteAllClicked(object sender, RoutedEventArgs e)
    {
        _dBSetup.DropTables();
        _dBSetup.CreateTables(new DataAccess(_dBSetup.ConnectionString));
    }
    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if(e.Source is PasswordBox pb)
        {
            Password = pb.Password;
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var result = await _dataGather.TestLogin(Username, Password);
        if (result)
            MessageBox.Show(Properties.Resources.SettingsPageLoginTestSuccess, Properties.Resources.SettingsPageLoginTestTitle, MessageBoxButton.OK);
        else
            MessageBox.Show(Properties.Resources.SettingsPageLoginTestFailed, Properties.Resources.SettingsPageLoginTestTitle, MessageBoxButton.OK,MessageBoxImage.Warning);
    }

    private void OnWhatsAppClicked(object sender, RoutedEventArgs e)
    {
        _systemService.OpenInWebBrowser(Properties.Resources.ContactMeWhatsAppUrl);
    }

    private void OnWebsiteClicked(object sender, RoutedEventArgs e)
    {
        _systemService.OpenInWebBrowser(Properties.Resources.ContactMeWebsite);
    }

    private void OnInstagramClicked(object sender, RoutedEventArgs e)
    {
        _systemService.OpenInWebBrowser(Properties.Resources.ContactMeInstagramUrl);
    }
}
