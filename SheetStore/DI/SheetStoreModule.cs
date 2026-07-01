using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProGlassAutomation.SheetStore.Navigation;
using ProGlassAutomation.SheetStore.Services;
using ProGlassAutomation.SheetStore.ViewModels;
using ProGlassAutomation.SheetStore.Views;

namespace ProGlassAutomation.SheetStore.DI;

public static class SheetStoreModule
{
    public static IServiceCollection AddSheetStoreModule(
        this IServiceCollection services,
        string databasePath)
    {
        // ─── Infrastructure (Singletons) ───
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<ISheetStoreCache, SheetStoreCache>();
        services.AddSingleton<ISheetStoreExport, SheetStoreExport>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<ISheetStoreRepository>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<SqliteRepository>>();
            return new SqliteRepository(databasePath, logger);
        });

        services.AddSingleton<ISheetStoreService, SheetStoreService>();

        // ─── ViewModels (Transient) ───
        services.AddTransient<SheetStoreViewModel>();
        services.AddTransient<SheetDialogViewModel>();
        services.AddTransient<BuySheetDialogViewModel>();
        services.AddTransient<UseSheetsDialogViewModel>();
        services.AddTransient<PurchasePriceDialogViewModel>();
        services.AddTransient<PriceHistoryDialogViewModel>();
        services.AddTransient<AddSupplierDialogViewModel>();

        // ─── Views (Transient) - ONLY parameterless constructors ───
        services.AddTransient<SheetStoreView>();
        services.AddTransient<AddSupplierDialog>();
        // Dialogs with required parameters are NOT registered in DI
        // They are created with 'new' in the ViewModel (e.g., 'new SheetDialog()')

        return services;
    }
}