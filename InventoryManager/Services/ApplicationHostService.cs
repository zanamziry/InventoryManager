using InventoryManager.Contracts.Activation;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Services;
using InventoryManager.Views;

using Microsoft.Extensions.Hosting;
using Squirrel;
using System.Diagnostics;
using System.Windows;

namespace InventoryManager.Services;

public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILanguageSelectorService _languageSelector;
    private readonly IDBSetup _dBSetup;
    private readonly INavigationService _navigationService;
    private readonly IPersistAndRestoreService _persistAndRestoreService;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private IShellWindow _shellWindow;
    private bool _isInitialized;

    public ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IPersistAndRestoreService persistAndRestoreService, IDBSetup dBSetup, ILanguageSelectorService languageSelector)
    {
        _serviceProvider = serviceProvider;
        _activationHandlers = activationHandlers;
        _navigationService = navigationService;
        _persistAndRestoreService = persistAndRestoreService;
        _dBSetup = dBSetup;
        _languageSelector = languageSelector;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize services that you need before app activation
        InitializeAsync();

        await HandleActivationAsync();
        
        // Tasks after activation
        await StartupAsync();
        _isInitialized = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _persistAndRestoreService.PersistData();
        await Task.CompletedTask;
    }

    private async void InitializeAsync()
    {
        if (!_isInitialized)
        {
            _persistAndRestoreService.RestoreData();
            _dBSetup.InitializeDatabase();
            _languageSelector.InitializeLanguage();
        }
    }

    private async Task StartupAsync()
    {
        if (!_isInitialized)
        {
            _languageSelector.InitializeLanguage();
            UpdatingService.Update(_shellWindow);
            await _shellWindow.ShowMessage("test", "ok");
            await Task.CompletedTask;
        }
    }

    private async Task HandleActivationAsync()
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle());

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync();
        }

        await Task.CompletedTask;

        if (App.Current.Windows.OfType<IShellWindow>().Count() == 0)
        {
            // Default activation that navigates to the apps default page
            _shellWindow = _serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
            _navigationService.Initialize(_shellWindow.GetNavigationFrame());
            _shellWindow.ShowWindow();
            _navigationService.NavigateTo(typeof(MainPage));
            await Task.CompletedTask;
        }
    }
}
