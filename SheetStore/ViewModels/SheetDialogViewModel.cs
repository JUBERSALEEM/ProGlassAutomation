using CommunityToolkit.Mvvm.ComponentModel;
using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.SheetStore.ViewModels;

public partial class SheetDialogViewModel : ObservableObject
{
    [ObservableProperty] private string _category = string.Empty;
    [ObservableProperty] private string _thickness = string.Empty;
    [ObservableProperty] private string _color = string.Empty;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;
    [ObservableProperty] private string _supplier = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private decimal _purchasePrice;
    [ObservableProperty] private decimal _sellPrice;
    [ObservableProperty] private int _initialStock;

    public string Title { get; }
    public Sheet? NewSheet { get; private set; }
    public bool IsEdit { get; }

    public List<string> Categories => Sheet.Categories;
    public string[] Thicknesses => Sheet.Thicknesses;
    public List<ColorItem> Colors => Sheet.ColorItems;
    public List<string> Suppliers => Sheet.Suppliers;

    public SheetDialogViewModel(Sheet? existing = null)
    {
        IsEdit = existing is not null;
        Title = IsEdit ? "Edit Sheet" : "Add New Sheet";

        if (existing is not null)
        {
            _category = existing.Category;
            _thickness = existing.Thickness;
            _color = existing.Color;
            _width = existing.Width;
            _height = existing.Height;
            _supplier = existing.Supplier;
            _description = existing.Description;
            _purchasePrice = existing.PurchasePrice;
            _sellPrice = existing.SellPrice;
            _initialStock = existing.TotalStock;
        }
    }

    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(_category)) return false;
        if (string.IsNullOrWhiteSpace(_thickness)) return false;
        if (string.IsNullOrWhiteSpace(_color)) return false;
        if (_width <= 0 || _height <= 0) return false;
        return true;
    }

    public Sheet BuildSheet()
    {
        NewSheet = new Sheet
        {
            Category = _category,
            Thickness = _thickness,
            Color = _color,
            Width = _width,
            Height = _height,
            SquareMeter = _width * _height,
            Supplier = _supplier,
            Description = _description,
            PurchasePrice = _purchasePrice,
            SellPrice = _sellPrice,
            TotalStock = _initialStock
        };
        return NewSheet;
    }
}