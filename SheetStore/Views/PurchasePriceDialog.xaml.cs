using System.Collections.Generic;
using System.Windows;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.ViewModels;

namespace ProGlassAutomation.SheetStore.Views;

public partial class PurchasePriceDialog : Window
{
    public PurchasePriceDialogViewModel ViewModel { get; }

    public PurchasePriceDialog() : this(new List<Sheet>()) { }

    public PurchasePriceDialog(IEnumerable<Sheet> sheets)
    {
        InitializeComponent();
        ViewModel = new PurchasePriceDialogViewModel(sheets);
        DataContext = ViewModel;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}