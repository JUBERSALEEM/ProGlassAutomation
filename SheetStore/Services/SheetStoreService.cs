using Microsoft.Extensions.Logging;
using ProGlassAutomation.SheetStore.Common;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.Models.DTOs;

namespace ProGlassAutomation.SheetStore.Services;

public sealed class SheetStoreService : ISheetStoreService, IDisposable
{
    private readonly ISheetStoreRepository _repository;
    private readonly ISheetStoreCache _cache;
    private readonly IMediator _mediator;
    private readonly ILogger<SheetStoreService> _logger;
    private readonly SemaphoreSlim _operationLock = new(1, 1);

    public event EventHandler? DataChanged;

    public SheetStoreService(
        ISheetStoreRepository repository,
        ISheetStoreCache cache,
        IMediator mediator,
        ILogger<SheetStoreService> logger)
    {
        _repository = repository;
        _cache = cache;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Sheet>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var cached = _cache.GetAll();
        if (cached.Count > 0) return cached;

        await _operationLock.WaitAsync(ct);
        try
        {
            var sheets = await _repository.GetAllAsync(ct);
            _cache.SetAll(sheets);
            return sheets;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task<SheetStats> GetGlobalStatsAsync(CancellationToken ct = default)
    {
        var sheets = await GetAllActiveAsync(ct);
        return SheetStats.From(sheets);
    }

    public async Task AddSheetAsync(Sheet sheet, CancellationToken ct = default)
    {
        await _operationLock.WaitAsync(ct);
        try
        {
            var id = await _repository.AddAsync(sheet, ct);
            sheet.Id = id;
            _cache.Update(sheet);
            NotifyChanged();
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task UpdateSheetAsync(Sheet sheet, CancellationToken ct = default)
    {
        await _operationLock.WaitAsync(ct);
        try
        {
            await _repository.UpdateAsync(sheet, ct);
            _cache.Update(sheet);
            NotifyChanged();
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task DeleteSheetAsync(int id, CancellationToken ct = default)
    {
        await _operationLock.WaitAsync(ct);
        try
        {
            await _repository.SoftDeleteAsync(id, ct);
            _cache.Remove(id);
            NotifyChanged();
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task AddPurchaseAsync(SheetPurchase purchase, CancellationToken ct = default)
    {
        await _operationLock.WaitAsync(ct);
        try
        {
            await _repository.AddPurchaseAsync(purchase, ct);
            InvalidateCache();
            NotifyChanged();
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task AddUsageAsync(SheetUsage usage, CancellationToken ct = default)
    {
        await _operationLock.WaitAsync(ct);
        try
        {
            await _repository.AddUsageAsync(usage, ct);
            InvalidateCache();
            NotifyChanged();
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task<IReadOnlyList<Supplier>> GetSuppliersAsync(CancellationToken ct = default) =>
        await _repository.GetSuppliersAsync(ct);

    public async Task<int> AddSupplierAsync(Supplier supplier, CancellationToken ct = default)
    {
        await _operationLock.WaitAsync(ct);
        try
        {
            var id = await _repository.AddSupplierAsync(supplier, ct);
            supplier.Id = id;
            _mediator.Publish(new SuppliersChangedMessage());
            return id;
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public async Task<ImportResult> BulkImportAsync(IEnumerable<Sheet> sheets, CancellationToken ct = default)
    {
        await _operationLock.WaitAsync(ct);
        try
        {
            var count = await _repository.BulkInsertAsync(sheets, ct);
            InvalidateCache();
            NotifyChanged();
            return new ImportResult(count, new List<string>());
        }
        finally
        {
            _operationLock.Release();
        }
    }

    public void InvalidateCache() => _cache.Invalidate();

    private void NotifyChanged()
    {
        DataChanged?.Invoke(this, EventArgs.Empty);
        _mediator.Publish(new SheetsChangedMessage());
    }

    public void Dispose() => _operationLock.Dispose();
}