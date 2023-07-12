using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using InventoryManager.Contracts.Services;
using InventoryManager.Contracts.Views;
using InventoryManager.Core.Contracts.Services;
using InventoryManager.Core.Services;
using InventoryManager.Models;
using InventoryManager.Services;
using InventoryManager.Views;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryManager;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : Application
{
    private IHost _host;

    public T GetService<T>()
        where T : class
        => _host.Services.GetService(typeof(T)) as T;

    public App()
    {
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
        _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation);
                })
                .ConfigureServices(ConfigureServices)
                .Build();
        _host.StartAsync();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // TODO: Register your services, viewmodels and pages here

        // App Host
        services.AddHostedService<ApplicationHostService>();

        // Activation Handlers

        // Core Services
        services.AddSingleton<IFileService, FileService>();

        // Services
        services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDBSetup, DBSetup>();
        services.AddSingleton<ISystemDataGather, SystemDataGather>();
        services.AddSingleton<ILanguageSelectorService, LanguageSelectorService>();

        // Views
        services.AddSingleton<IShellWindow, ShellWindow>();

        services.AddTransient<MainPage>();
        services.AddTransient<InventoryPage>();
        services.AddTransient<SystemInventory>();
        services.AddTransient<SentOutsidePage>();
        services.AddTransient<GiveAwayPage>();
        services.AddTransient<SettingsPage>();


        // Configuration
        services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
    }
    protected async override void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        _host = null;
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var l = new LoggerFactory().CreateLogger("UNHANDLED");
        if (e.Exception is SqliteException || e.Exception is StackOverflowException || e.Exception is NullReferenceException)
        {
            l.LogCritical(e.Exception, "CriticalUnhandled");
            e.Handled = false;
            MessageBox.Show(e.Exception.Message, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show(e.Exception.Message, "Unhandled Error",MessageBoxButton.OK,MessageBoxImage.Warning);
            l.LogError(e.Exception, "ErrorUnhandled");
            e.Handled = true;
        }
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
