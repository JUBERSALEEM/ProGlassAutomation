using CommunityToolkit.Mvvm.ComponentModel;
using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.SheetStore.ViewModels;

public partial class BuySheetDialogViewModel : ObservableObject
{
    [ObservableProperty] private int _quantity = 1;
    [ObservableProperty] private decimal _unitPrice;
    [ObservableProperty] private string _supplier = string.Empty;
    [ObservableProperty] private DateTime _purchasedOn = DateTime.Now;

    public string SheetInfo { get; }
    public decimal TotalCost => _quantity * _unitPrice;
    public BuySheetResult? Result { get; private set; }

    public BuySheetDialogViewModel(Sheet sheet)
    {
        SheetInfo = $"{sheet.Category} {sheet.Thickness} {sheet.Color} ({sheet.Width}×{sheet.Height})";
        _unitPrice = sheet.PurchasePrice;
        _supplier = sheet.Supplier;
    }

    partial void OnQuantityChanged(int value) => OnPropertyChanged(nameof(TotalCost));
    partial void OnUnitPriceChanged(decimal value) => OnPropertyChanged(nameof(TotalCost));

    public bool BuildResult()
    {
        if (_quantity <= 0) return false;
        Result = new BuySheetResult(_quantity, _unitPrice, _supplier, _purchasedOn);
        return true;
    }
}

public record BuySheetResult(int Quantity, decimal UnitPrice, string Supplier, DateTime PurchasedOn);