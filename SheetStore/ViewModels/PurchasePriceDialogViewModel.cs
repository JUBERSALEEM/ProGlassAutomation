using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.SheetStore.ViewModels;

public partial class PurchasePriceDialogViewModel : ObservableObject
{
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private Sheet? _selectedSheet;
    [ObservableProperty] private decimal _newPurchasePrice;
    [ObservableProperty] private decimal _newSellPrice;

    public ObservableCollection<Sheet> Sheets { get; } = new();
    public ObservableCollection<Sheet> FilteredSheets { get; } = new();

    public PurchasePriceDialogViewModel(IEnumerable<Sheet> sheets)
    {
        foreach (var s in sheets.OrderBy(s => s.Category))
            Sheets.Add(s);
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnSelectedSheetChanged(Sheet? value)
    {
        if (value is not null)
        {
            _newPurchasePrice = value.PurchasePrice;
            _newSellPrice = value.SellPrice;
        }
    }

    private void ApplyFilter()
    {
        FilteredSheets.Clear();
        var query = Sheets.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            var s = _searchText.Trim();
            query = query.Where(x => x.Category.Contains(s, StringComparison.OrdinalIgnoreCase)
                                  || x.Color.Contains(s, StringComparison.OrdinalIgnoreCase)
                                  || x.Thickness.Contains(s, StringComparison.OrdinalIgnoreCase));
        }
        foreach (var sheet in query)
            FilteredSheets.Add(sheet);
    }

    [RelayCommand]
    private void Apply()
    {
        if (_selectedSheet is null) return;
        _selectedSheet.PurchasePrice = _newPurchasePrice;
        _selectedSheet.SellPrice = _newSellPrice;
    }
}