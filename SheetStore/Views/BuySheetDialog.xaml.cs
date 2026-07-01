using System.Windows;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.ViewModels;

namespace ProGlassAutomation.SheetStore.Views;

public partial class BuySheetDialog : Window
{
    public BuySheetDialogViewModel ViewModel { get; }

    public BuySheetDialog() : this(null!) { }

    public BuySheetDialog(Sheet sheet)
    {
        InitializeComponent();
        ViewModel = new BuySheetDialogViewModel(sheet);
        DataContext = ViewModel;
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.BuildResult())
        {
            MessageBox.Show("Quantity must be greater than 0", "Validation");
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