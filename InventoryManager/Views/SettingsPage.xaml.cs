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

using Microsoft.Extensions.Options;

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
            _dataGather.SaveSettings(value);
        }
    }

    public ObservableCollection<Language> Languages { get; set; } = new ObservableCollection<Language>();

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
        InitializeComponent();
        DataContext = this;
    }

    public void OnNavigatedTo(object parameter)
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
        ServerAddress = _dataGather.base_url;
        SelectedLang = _languageSelector.PreferedLang;
        Languages = new ObservableCollection<Language>(_languageSelector.Languages);
        OnPropertyChanged(nameof(Languages));
    }

    public void OnNavigatedFrom()
    {
    }

    private void OnPrivacyStatementClick(object sender, RoutedEventArgs e)
        => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

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

    private void OnDeleteAllClicked(object sender, RoutedEventArgs e)
    {
        _dBSetup.DropTables();
        _dBSetup.CreateTables();
    }
}
