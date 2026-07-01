using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.Models.DTOs;

namespace ProGlassAutomation.SheetStore.Services;

public interface ISheetStoreRepository
{
    Task InitializeAsync();
    Task<IReadOnlyList<Sheet>> GetAllAsync(CancellationToken ct = default);
    Task<Sheet?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> AddAsync(Sheet sheet, CancellationToken ct = default);
    Task UpdateAsync(Sheet sheet, CancellationToken ct = default);
    Task SoftDeleteAsync(int id, CancellationToken ct = default);
    Task AddPurchaseAsync(SheetPurchase purchase, CancellationToken ct = default);
    Task AddUsageAsync(SheetUsage usage, CancellationToken ct = default);
    Task<IReadOnlyList<Supplier>> GetSuppliersAsync(CancellationToken ct = default);
    Task<int> AddSupplierAsync(Supplier supplier, CancellationToken ct = default);
    Task<int> BulkInsertAsync(IEnumerable<Sheet> sheets, CancellationToken ct = default);
    Task<SheetStats> GetGlobalStatsAsync(CancellationToken ct = default);
}