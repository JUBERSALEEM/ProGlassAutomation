using CommunityToolkit.Mvvm.ComponentModel;

namespace ProGlassAutomation.SheetStore.Models;

public partial class SheetUsage : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private int _sheetId;
    [ObservableProperty] private int _quantity;
    [ObservableProperty] private string _reason = string.Empty;
    [ObservableProperty] private DateTime _usedOn = DateTime.Now;
    [ObservableProperty] private DateTime _createdAt = DateTime.Now;
}