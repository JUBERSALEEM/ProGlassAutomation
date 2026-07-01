using System.Windows;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.ViewModels;

namespace ProGlassAutomation.SheetStore.Views;

public partial class SheetDialog : Window
{
    public SheetDialogViewModel ViewModel { get; }

    // Parameterless constructor for `new SheetDialog()` calls
    public SheetDialog() : this(null) { }

    public SheetDialog(Sheet? existing)
    {
        InitializeComponent();
        ViewModel = new SheetDialogViewModel(existing);
        DataContext = ViewModel;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.Validate())
        {
            MessageBox.Show("Please fill all required fields (Category, Thickness, Color, Width, Height)",
                "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        ViewModel.BuildSheet();
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}