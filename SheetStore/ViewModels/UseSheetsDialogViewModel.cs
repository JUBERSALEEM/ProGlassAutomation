using CommunityToolkit.Mvvm.ComponentModel;

namespace ProGlassAutomation.SheetStore.ViewModels;

public partial class UseSheetsDialogViewModel : ObservableObject
{
    [ObservableProperty] private int _quantity = 1;
    [ObservableProperty] private string _reason = string.Empty;
    [ObservableProperty] private DateTime _usedOn = DateTime.Now;

    public int MaxQuantity { get; }
    public UseSheetsResult? Result { get; private set; }

    public UseSheetsDialogViewModel(int maxQuantity)
    {
        MaxQuantity = maxQuantity;
    }

    partial void OnQuantityChanged(int value)
    {
        if (value > MaxQuantity) _quantity = MaxQuantity;
        if (value < 1) _quantity = 1;
    }

    public bool BuildResult()
    {
        if (_quantity <= 0 || _quantity > MaxQuantity) return false;
        Result = new UseSheetsResult(_quantity, _reason, _usedOn);
        return true;
    }
}

public record UseSheetsResult(int Quantity, string Reason, DateTime UsedOn);