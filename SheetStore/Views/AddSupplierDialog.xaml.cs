using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ProGlassAutomation.SheetStore.ViewModels;

namespace ProGlassAutomation.SheetStore.Views;

public partial class AddSupplierDialog : Window
{
    public AddSupplierDialogViewModel ViewModel { get; }

    public AddSupplierDialog() : this(GetService<AddSupplierDialogViewModel>())
    {
    }

    public AddSupplierDialog(AddSupplierDialogViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
    }

    private static T GetService<T>() where T : class =>
        App.ServiceProvider.GetRequiredService<T>();

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.SupplierName))
        {
            MessageBox.Show("Supplier name is required", "Validation");
            return;
        }
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}