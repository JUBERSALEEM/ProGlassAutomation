using System.Collections.Concurrent;
using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.SheetStore.Services;

public interface ISheetStoreCache
{
    IReadOnlyList<Sheet> GetAll();
    void SetAll(IReadOnlyList<Sheet> sheets);
    void Invalidate();
    void Update(Sheet sheet);
    void Remove(int id);
}

public sealed class SheetStoreCache : ISheetStoreCache
{
    private readonly ConcurrentDictionary<int, Sheet> _cache = new();

    public IReadOnlyList<Sheet> GetAll() =>
        _cache.Values.OrderBy(s => s.Category).ToList();

    public void SetAll(IReadOnlyList<Sheet> sheets)
    {
        _cache.Clear();
        foreach (var sheet in sheets)
            _cache[sheet.Id] = sheet;
    }

    public void Invalidate() => _cache.Clear();
    public void Update(Sheet sheet) => _cache[sheet.Id] = sheet;
    public void Remove(int id) => _cache.TryRemove(id, out _);
}