using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProGlassAutomation.SheetStore.Models;

public partial class Sheet : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _srNo;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _thickness = string.Empty;

    [ObservableProperty]
    private string _color = string.Empty;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;

    [ObservableProperty]
    private string _supplier = string.Empty;

    [ObservableProperty]
    private string _supplierName = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private double _squareMeter;

    [ObservableProperty]
    private decimal _purchasePrice;

    [ObservableProperty]
    private decimal _sellPrice;

    [ObservableProperty]
    private int _totalStock;

    [ObservableProperty]
    private int _usedSheets;

    [ObservableProperty]
    private DateTime _createdDate = DateTime.Now;

    [ObservableProperty]
    private DateTime? _latestPurchaseDate;

    [ObservableProperty]
    private bool _isActive = true;

    // ─── Computed Properties (use backing fields directly) ───
    public int BalanceSheets => _totalStock - _usedSheets;
    public double TotalStockSQM => _totalStock * _squareMeter;
    public double UsedSQM => _usedSheets * _squareMeter;
    public double BalanceSQM => BalanceSheets * _squareMeter;
    public decimal TotalPurchaseValue => _totalStock * _purchasePrice;
    public decimal TotalSellValue => _totalStock * _sellPrice;
    public decimal TotalProfitValue => TotalSellValue - TotalPurchaseValue;
    public string DisplayDateTime => _createdDate.ToString("dd/MM/yyyy HH:mm");
    public string ColorHex => ColorHelper.GetHex(_color);
    public Brush ColorBrushValue => ColorHelper.GetBrush(_color);

    // ─── Static Master Data ───
    public static readonly List<string> Categories = new()
    {
        "Mirror", "Tinted", "Clear", "Frosted", "Tempered", "Laminated"
    };

    public static readonly string[] Thicknesses =
    {
        "3mm", "4mm", "5mm", "6mm", "8mm", "10mm", "12mm"
    };

    public static readonly List<ColorItem> ColorItems = new()
    {
        new("Bronze", "#8B4513"),
        new("Clear", "#E0F7FA"),
        new("Dark Grey", "#374151"),
        new("Euro Grey", "#6B7280"),
        new("Frosted", "#D1D5DB"),
        new("Green", "#059669"),
        new("Mirror", "#C0C0C0"),
        new("Blue", "#1E40AF")
    };

    public static List<string> Suppliers { get; } = new()
    {
        "Gulf Glass", "Saint Gobain", "AGC", "Pilkington", "Guardian"
    };

    public static bool SupplierExists(string name) =>
        Suppliers.Any(s => s.Equals(name, StringComparison.OrdinalIgnoreCase));

    public static void AddSupplier(string name)
    {
        if (!SupplierExists(name)) Suppliers.Add(name);
    }
}

public record ColorItem(string Name, string Hex);

public static class ColorHelper
{
    public static string GetHex(string colorName) =>
        Sheet.ColorItems.FirstOrDefault(c => c.Name == colorName)?.Hex ?? "#94A3B8";

    public static Brush GetBrush(string colorName)
    {
        var hex = GetHex(colorName);
        try
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
        catch
        {
            return Brushes.Gray;
        }
    }
}