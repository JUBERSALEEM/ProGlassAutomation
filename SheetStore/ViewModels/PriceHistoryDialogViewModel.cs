using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.SheetStore.ViewModels;

public partial class PriceHistoryDialogViewModel : ObservableObject
{
    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<Sheet> AllSheets { get; } = new();
    public ObservableCollection<Sheet> FilteredSheets { get; } = new();
    [ObservableProperty] private Sheet? _selectedSheet;

    public PriceHistoryDialogViewModel(IEnumerable<Sheet> sheets)
    {
        foreach (var s in sheets.OrderBy(s => s.Category))
            AllSheets.Add(s);
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        FilteredSheets.Clear();
        var query = AllSheets.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            var s = _searchText.Trim();
            query = query.Where(x => x.Category.Contains(s, StringComparison.OrdinalIgnoreCase)
                                  || x.Color.Contains(s, StringComparison.OrdinalIgnoreCase));
        }
        foreach (var sheet in query)
            FilteredSheets.Add(sheet);
    }
}