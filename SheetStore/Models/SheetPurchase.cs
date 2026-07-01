using CommunityToolkit.Mvvm.ComponentModel;

namespace ProGlassAutomation.SheetStore.Models;

public partial class SheetPurchase : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private int _sheetId;
    [ObservableProperty] private int _quantity;
    [ObservableProperty] private decimal _unitPrice;
    [ObservableProperty] private string _supplier = string.Empty;
    [ObservableProperty] private DateTime _purchasedOn = DateTime.Now;
    [ObservableProperty] private DateTime _createdAt = DateTime.Now;

    public decimal TotalCost => _quantity * _unitPrice;
}