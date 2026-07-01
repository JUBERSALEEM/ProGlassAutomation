using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ProGlassAutomation.SheetStore.Common;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.Models.DTOs;
using ProGlassAutomation.SheetStore.Navigation;
using ProGlassAutomation.SheetStore.Services;
using ProGlassAutomation.SheetStore.Views;

namespace ProGlassAutomation.SheetStore.ViewModels;

public partial class SheetStoreViewModel : ObservableObject, INavigationAware, IDisposable
{
    private readonly ISheetStoreService _service;
    private readonly ISheetStoreExport _export;
    private readonly IMediator _mediator;
    private readonly INavigationService _navigation;
    private readonly ILogger<SheetStoreViewModel> _logger;
    private readonly List<Sheet> _allSheets = new();
    private bool _disposed;

    public SheetStoreViewModel(
        ISheetStoreService service,
        ISheetStoreExport export,
        IMediator mediator,
        INavigationService navigation,
        ILogger<SheetStoreViewModel> logger)
    {
        _service = service;
        _export = export;
        _mediator = mediator;
        _navigation = navigation;
        _logger = logger;

        _mediator.Subscribe<SheetsChangedMessage>(OnSheetsChanged);
        SortOptions = new ObservableCollection<SheetSortOptionDisplay>(SheetSortOptionDisplay.All);
        _selectedSortOption = SortOptions.First();
        _ = LoadDataAsync();
    }

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = "Ready";
    [ObservableProperty] private Sheet? _selectedSheet;
    [ObservableProperty] private string _selectedCategory = "ALL";
    [ObservableProperty] private string _selectedThickness = "ALL";
    [ObservableProperty] private string _selectedColor = "ALL";
    [ObservableProperty] private string _selectedSupplier = "ALL";
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private SheetSortOptionDisplay _selectedSortOption;
    [ObservableProperty] private SheetStats _globalStats = SheetStats.Empty;
    [ObservableProperty] private SheetStats _filteredStats = SheetStats.Empty;
    [ObservableProperty] private SheetStats _selectedStats = SheetStats.Empty;
    [ObservableProperty] private string _selectedLastPurchase = "-";
    [ObservableProperty] private string _selectedLastUpdate = "-";

    public ObservableCollection<Sheet> FilteredSheets { get; } = new();
    public ObservableCollection<string> Categories { get; } = new() { "ALL" };
    public ObservableCollection<string> Thicknesses { get; } = new() { "ALL" };
    public ObservableCollection<string> Colors { get; } = new() { "ALL" };
    public ObservableCollection<string> Suppliers { get; } = new() { "ALL" };
    public ObservableCollection<SheetSortOptionDisplay> SortOptions { get; }

    public bool HasSelection => _selectedSheet is not null;
    public int FilteredCount => FilteredSheets.Count;

    [RelayCommand(CanExecute = nameof(CanAddSheet))]
    private async Task AddSheetAsync()
    {
        try
        {
            var dialog = new SheetDialog();
            if (dialog.ShowDialog() == true && dialog.ViewModel.NewSheet is { } sheet)
            {
                await _service.AddSheetAsync(sheet);
                _statusMessage = $"Added: {sheet.Category} {sheet.Thickness} {sheet.Color}";
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddSheet failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedSheet))]
    private async Task EditSheetAsync(Sheet? sheet)
    {
        sheet ??= _selectedSheet;
        if (sheet is null) return;

        try
        {
            var dialog = new SheetDialog(sheet);
            if (dialog.ShowDialog() == true && dialog.ViewModel.NewSheet is { } updated)
            {
                updated.Id = sheet.Id;
                await _service.UpdateSheetAsync(updated);
                _statusMessage = $"Updated: {updated.Category} {updated.Thickness}";
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditSheet failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedSheet))]
    private async Task DeleteSheetAsync(Sheet? sheet)
    {
        sheet ??= _selectedSheet;
        if (sheet is null) return;

        var confirm = MessageBox.Show(
            $"Delete '{sheet.Category} - {sheet.Thickness} {sheet.Color}'?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            if (_selectedSheet?.Id == sheet.Id)
            {
                _selectedSheet = null;
                OnPropertyChanged(nameof(SelectedSheet));
            }
            await _service.DeleteSheetAsync(sheet.Id);
            _statusMessage = $"Deleted: {sheet.Category} {sheet.Thickness}";
            OnPropertyChanged(nameof(StatusMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteSheet failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedSheet))]
    private async Task BuySheetAsync(Sheet? sheet)
    {
        sheet ??= _selectedSheet;
        if (sheet is null) return;

        try
        {
            var dialog = new BuySheetDialog(sheet);
            if (dialog.ShowDialog() == true && dialog.ViewModel.Result is { } result)
            {
                await _service.AddPurchaseAsync(new SheetPurchase
                {
                    SheetId = sheet.Id,
                    Quantity = result.Quantity,
                    UnitPrice = result.UnitPrice,
                    Supplier = result.Supplier,
                    PurchasedOn = result.PurchasedOn,
                    CreatedAt = DateTime.Now
                });
                _statusMessage = $"Purchased {result.Quantity} sheets";
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BuySheet failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedSheet))]
    private async Task UseSheetsAsync(Sheet? sheet)
    {
        sheet ??= _selectedSheet;
        if (sheet is null) return;

        if (sheet.BalanceSheets <= 0)
        {
            MessageBox.Show("No sheets available to use", "Insufficient Stock");
            return;
        }

        try
        {
            var dialog = new UseSheetsDialog(sheet.BalanceSheets);
            if (dialog.ShowDialog() == true && dialog.ViewModel.Result is { } result)
            {
                await _service.AddUsageAsync(new SheetUsage
                {
                    SheetId = sheet.Id,
                    Quantity = result.Quantity,
                    Reason = result.Reason,
                    UsedOn = result.UsedOn,
                    CreatedAt = DateTime.Now
                });
                _statusMessage = $"Used {result.Quantity} sheets";
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UseSheets failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV Files|*.csv",
                FileName = $"SheetInventory_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dialog.ShowDialog() != true) return;

            _isLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            AddSheetCommand.NotifyCanExecuteChanged();
            _statusMessage = "Exporting...";
            OnPropertyChanged(nameof(StatusMessage));

            await _export.ExportToCsvAsync(FilteredSheets, dialog.FileName);
            _statusMessage = $"Exported {FilteredSheets.Count} rows";
            OnPropertyChanged(nameof(StatusMessage));
            MessageBox.Show("Export completed!", "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
        }
        finally
        {
            _isLoading = false;
            OnPropertyChanged(nameof(IsLoading));
            AddSheetCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        try
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "CSV Files|*.csv" };
            if (dialog.ShowDialog() != true) return;

            _isLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            AddSheetCommand.NotifyCanExecuteChanged();
            _statusMessage = "Importing...";
            OnPropertyChanged(nameof(StatusMessage));

            var result = await _export.ImportFromCsvAsync(dialog.FileName);

            if (result.ImportedCount > 0 && result.Sheets is not null)
                await _service.BulkImportAsync(result.Sheets);

            _statusMessage = $"Imported {result.ImportedCount} rows";
            OnPropertyChanged(nameof(StatusMessage));
            MessageBox.Show(
                $"Imported: {result.ImportedCount}\nErrors: {result.Errors.Count}",
                "Import Result");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
        }
        finally
        {
            _isLoading = false;
            OnPropertyChanged(nameof(IsLoading));
            AddSheetCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void ClearFilter()
    {
        _selectedCategory = "ALL"; OnPropertyChanged(nameof(SelectedCategory));
        _selectedThickness = "ALL"; OnPropertyChanged(nameof(SelectedThickness));
        _selectedColor = "ALL"; OnPropertyChanged(nameof(SelectedColor));
        _selectedSupplier = "ALL"; OnPropertyChanged(nameof(SelectedSupplier));
        _searchText = string.Empty; OnPropertyChanged(nameof(SearchText));
        _selectedSheet = null; OnPropertyChanged(nameof(SelectedSheet));
    }

    [RelayCommand]
    private void ViewHistory()
    {
        var dialog = new PriceHistoryDialog(_allSheets);
        dialog.ShowDialog();
    }

    [RelayCommand]
    private void UpdatePurchasePrice()
    {
        var dialog = new PurchasePriceDialog(_allSheets);
        dialog.ShowDialog();
    }

    [RelayCommand]
    private async Task AddSupplierAsync()
    {
        try
        {
            var dialog = new AddSupplierDialog();
            if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.ViewModel.SupplierName))
                return;

            var name = dialog.ViewModel.SupplierName.Trim();
            var supplier = new Supplier { Name = name };
            if (await _service.AddSupplierAsync(supplier) > 0)
            {
                await LoadFilterListsAsync();
                _statusMessage = $"Added supplier: {name}";
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddSupplier failed");
        }
    }

    [RelayCommand]
    private void Refresh() => _ = LoadDataAsync();

    // ═══ CanExecute methods ═══
    private bool CanAddSheet() => !_isLoading;
    private bool HasSelectedSheet(Sheet? sheet) => sheet is not null || _selectedSheet is not null;

    partial void OnSelectedCategoryChanged(string value) => ApplyFilters();
    partial void OnSelectedThicknessChanged(string value) => ApplyFilters();
    partial void OnSelectedColorChanged(string value) => ApplyFilters();
    partial void OnSelectedSupplierChanged(string value) => ApplyFilters();
    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedSortOptionChanged(SheetSortOptionDisplay value) => ApplyFilters();
    partial void OnSelectedSheetChanged(Sheet? value)
    {
        _selectedStats = value is null ? _filteredStats : SheetStats.From(new[] { value });
        OnPropertyChanged(nameof(SelectedStats));
        _selectedLastPurchase = value?.LatestPurchaseDate?.ToString("dd/MM/yyyy") ?? "-";
        OnPropertyChanged(nameof(SelectedLastPurchase));
        _selectedLastUpdate = value?.DisplayDateTime ?? "-";
        OnPropertyChanged(nameof(SelectedLastUpdate));
        EditSheetCommand.NotifyCanExecuteChanged();
        DeleteSheetCommand.NotifyCanExecuteChanged();
        BuySheetCommand.NotifyCanExecuteChanged();
        UseSheetsCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(HasSelection));
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _isLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            AddSheetCommand.NotifyCanExecuteChanged();  // ✅ FIX: disable button while loading
            _statusMessage = "Loading sheets...";
            OnPropertyChanged(nameof(StatusMessage));

            var sheets = await _service.GetAllActiveAsync();
            _allSheets.Clear();
            _allSheets.AddRange(sheets);

            await LoadFilterListsAsync();
            ApplyFilters();
            _globalStats = SheetStats.From(_allSheets);
            OnPropertyChanged(nameof(GlobalStats));

            _statusMessage = $"Loaded {sheets.Count} sheets";
            OnPropertyChanged(nameof(StatusMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadData failed");
            _statusMessage = $"Error: {ex.Message}";
            OnPropertyChanged(nameof(StatusMessage));
            MessageBox.Show($"Failed to load: {ex.Message}", "Error");
        }
        finally
        {
            _isLoading = false;
            OnPropertyChanged(nameof(IsLoading));
            AddSheetCommand.NotifyCanExecuteChanged();  // ✅ FIX: re-enable button after loading
        }
    }

    private async Task LoadFilterListsAsync()
    {
        try
        {
            var suppliers = await _service.GetSuppliersAsync();

            Categories.Clear();
            Categories.Add("ALL");
            foreach (var c in _allSheets.Select(s => s.Category).Distinct().OrderBy(x => x))
                Categories.Add(c);

            Thicknesses.Clear();
            Thicknesses.Add("ALL");
            foreach (var t in _allSheets.Select(s => s.Thickness).Distinct().OrderBy(x => x))
                Thicknesses.Add(t);

            Colors.Clear();
            Colors.Add("ALL");
            foreach (var c in _allSheets.Select(s => s.Color).Distinct().OrderBy(x => x))
                Colors.Add(c);

            Suppliers.Clear();
            Suppliers.Add("ALL");
            foreach (var s in suppliers.Select(sup => sup.Name).OrderBy(x => x))
                Suppliers.Add(s);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadFilterLists failed");
        }
    }

    private void ApplyFilters()
    {
        if (_allSheets.Count == 0)
        {
            FilteredSheets.Clear();
            _filteredStats = SheetStats.Empty;
            OnPropertyChanged(nameof(FilteredStats));
            OnPropertyChanged(nameof(FilteredCount));
            return;
        }

        IEnumerable<Sheet> query = _allSheets;

        if (_selectedCategory != "ALL")
            query = query.Where(s => s.Category == _selectedCategory);
        if (_selectedThickness != "ALL")
            query = query.Where(s => s.Thickness == _selectedThickness);
        if (_selectedColor != "ALL")
            query = query.Where(s => s.Color == _selectedColor);
        if (_selectedSupplier != "ALL")
            query = query.Where(s => s.Supplier == _selectedSupplier);
        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            var search = _searchText.Trim();
            query = query.Where(s =>
                s.Category.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                s.Color.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                s.Thickness.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                s.Supplier.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                s.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        query = _selectedSortOption?.Option switch
        {
            SheetSortOption.StockHighLow => query.OrderByDescending(s => s.TotalStock),
            SheetSortOption.BalanceHighLow => query.OrderByDescending(s => s.BalanceSheets),
            SheetSortOption.ValueHighLow => query.OrderByDescending(s => s.TotalSellValue),
            SheetSortOption.LastUpdateNewestFirst => query.OrderByDescending(s => s.CreatedDate),
            SheetSortOption.ColorCategoryThickness => query.OrderBy(s => s.Color).ThenBy(s => s.Category).ThenBy(s => s.Thickness),
            _ => query.OrderBy(s => s.Category).ThenBy(s => s.Color).ThenBy(s => s.Thickness)
        };

        var sorted = query.ToList();
        for (int i = 0; i < sorted.Count; i++)
            sorted[i].SrNo = i + 1;

        FilteredSheets.Clear();
        foreach (var sheet in sorted)
            FilteredSheets.Add(sheet);

        _filteredStats = SheetStats.From(FilteredSheets);
        OnPropertyChanged(nameof(FilteredStats));
        OnPropertyChanged(nameof(FilteredCount));
    }

    private void OnSheetsChanged(SheetsChangedMessage message)
    {
        Application.Current?.Dispatcher.Invoke(async () => await LoadDataAsync());
    }

    public void OnNavigatedTo(object? parameter) => _ = LoadDataAsync();
    public void OnNavigatedFrom() { }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _mediator.Unsubscribe<SheetsChangedMessage>(OnSheetsChanged);
    }
}

public record SheetSortOptionDisplay(string Display, SheetSortOption Option)
{
    public override string ToString() => Display;

    public static List<SheetSortOptionDisplay> All { get; } = new()
    {
        new("Category → Color → Thickness", SheetSortOption.CategoryColorThickness),
        new("Color → Category → Thickness", SheetSortOption.ColorCategoryThickness),
        new("Thickness → Color → Category", SheetSortOption.ThicknessColorCategory),
        new("Stock High → Low", SheetSortOption.StockHighLow),
        new("Balance High → Low", SheetSortOption.BalanceHighLow),
        new("Value High → Low", SheetSortOption.ValueHighLow),
        new("Last Update (Newest)", SheetSortOption.LastUpdateNewestFirst)
    };
}