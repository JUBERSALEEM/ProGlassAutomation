using System.Windows;
using ProGlassAutomation.SheetStore.ViewModels;

namespace ProGlassAutomation.SheetStore.Views;

public partial class UseSheetsDialog : Window
{
    public UseSheetsDialogViewModel ViewModel { get; }

    public UseSheetsDialog() : this(1) { }

    public UseSheetsDialog(int maxQuantity)
    {
        InitializeComponent();
        ViewModel = new UseSheetsDialogViewModel(maxQuantity);
        DataContext = ViewModel;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.BuildResult())
        {
            MessageBox.Show($"Quantity must be between 1 and {ViewModel.MaxQuantity}", "Validation");
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