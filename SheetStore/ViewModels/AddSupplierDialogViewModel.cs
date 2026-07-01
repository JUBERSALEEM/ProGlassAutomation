using CommunityToolkit.Mvvm.ComponentModel;

namespace ProGlassAutomation.SheetStore.ViewModels;

public partial class AddSupplierDialogViewModel : ObservableObject
{
    [ObservableProperty] private string _supplierName = string.Empty;
    [ObservableProperty] private string _contact = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
}