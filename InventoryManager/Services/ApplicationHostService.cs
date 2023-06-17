﻿using InventoryManager.Contracts.Activation;
using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Services;
using InventoryManager.Views;

using Microsoft.Extensions.Hosting;
using Squirrel;
using System.Windows;

namespace InventoryManager.Services;

public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISystemDataGather _systemDataGather;
    private readonly ILanguageSelectorService _languageSelector;
    private readonly IDBSetup _dBSetup;
    private readonly INavigationService _navigationService;
    private readonly IPersistAndRestoreService _persistAndRestoreService;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private IShellWindow _shellWindow;
    private bool _isInitialized;

    public ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IPersistAndRestoreService persistAndRestoreService, IDBSetup dBSetup, ISystemDataGather systemDataGather, ILanguageSelectorService languageSelector)
    {
        _serviceProvider = serviceProvider;
        _activationHandlers = activationHandlers;
        _navigationService = navigationService;
        _persistAndRestoreService = persistAndRestoreService;
        _dBSetup = dBSetup;
        _systemDataGather = systemDataGather;
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

    private void InitializeAsync()
    {
        if (!_isInitialized)
        {
            _persistAndRestoreService.RestoreData();
            _dBSetup.InitializeDatabase();
            _systemDataGather.LoadSettings();
            _languageSelector.InitializeLanguage();
            Updates();
        }
    }
    private async void Updates()
    {
        var updateManager = await UpdateManager.GitHubUpdateManager("https://github.com/zanamziry/InventoryManager");
        var updateInfo = await updateManager.CheckForUpdate();
        if(updateInfo.ReleasesToApply.Count > 0)
        {
            if(MessageBoxResult.Yes == MessageBox.Show($"New Update Is Available Do You Want To Update To {updateInfo.ReleasesToApply.First().Version}", "Update Availbable!",MessageBoxButton.YesNo,MessageBoxImage.Question))
            {
                await updateManager.UpdateApp();
                MessageBox.Show($"Update Complete", "Restart Required For the Update To Take Effect");
                UpdateManager.RestartApp();
            }
        }
    }
    private async Task StartupAsync()
    {
        if (!_isInitialized)
        {
            _languageSelector.InitializeLanguage();

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
