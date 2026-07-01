using System.Collections.Generic;
using System.Windows;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.ViewModels;

namespace ProGlassAutomation.SheetStore.Views;

public partial class PriceHistoryDialog : Window
{
    public PriceHistoryDialogViewModel ViewModel { get; }

    public PriceHistoryDialog() : this(new List<Sheet>()) { }

    public PriceHistoryDialog(IEnumerable<Sheet> sheets)
    {
        InitializeComponent();
        ViewModel = new PriceHistoryDialogViewModel(sheets);
        DataContext = ViewModel;
    }
}