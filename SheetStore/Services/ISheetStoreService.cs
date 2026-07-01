using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.Models.DTOs;

namespace ProGlassAutomation.SheetStore.Services;

public interface ISheetStoreService
{
    event EventHandler? DataChanged;
    Task<IReadOnlyList<Sheet>> GetAllActiveAsync(CancellationToken ct = default);
    Task<SheetStats> GetGlobalStatsAsync(CancellationToken ct = default);
    Task AddSheetAsync(Sheet sheet, CancellationToken ct = default);
    Task UpdateSheetAsync(Sheet sheet, CancellationToken ct = default);
    Task DeleteSheetAsync(int id, CancellationToken ct = default);
    Task AddPurchaseAsync(SheetPurchase purchase, CancellationToken ct = default);
    Task AddUsageAsync(SheetUsage usage, CancellationToken ct = default);
    Task<IReadOnlyList<Supplier>> GetSuppliersAsync(CancellationToken ct = default);
    Task<int> AddSupplierAsync(Supplier supplier, CancellationToken ct = default);
    Task<ImportResult> BulkImportAsync(IEnumerable<Sheet> sheets, CancellationToken ct = default);
    void InvalidateCache();
}