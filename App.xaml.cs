using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProGlassAutomation.SheetStore.DI;
using ProGlassAutomation.SheetStore.Services;

namespace ProGlassAutomation;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        try
        {
            var repo = ServiceProvider.GetRequiredService<ISheetStoreRepository>();
            if (repo is SqliteRepository sqliteRepo)
                await sqliteRepo.InitializeAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database init failed: {ex.Message}", "Startup Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ProGlassAutomation");
        Directory.CreateDirectory(appDataPath);
        var dbPath = Path.Combine(appDataPath, "sheetstore.db");

        services.AddSheetStoreModule(dbPath);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (ServiceProvider is IDisposable disposable)
            disposable.Dispose();
        base.OnExit(e);
    }
}