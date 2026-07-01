using CommunityToolkit.Mvvm.ComponentModel;

namespace ProGlassAutomation.SheetStore.Models;

public partial class Supplier : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _contact = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private DateTime _createdAt = DateTime.Now;
    [ObservableProperty] private bool _isActive = true;
}