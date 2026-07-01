using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ProGlassAutomation.SheetStore.ViewModels;

namespace ProGlassAutomation.SheetStore.Views;

public partial class SheetStoreView : UserControl
{
    public SheetStoreView() : this(GetService<SheetStoreViewModel>())
    {
    }

    public SheetStoreView(SheetStoreViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private static T GetService<T>() where T : class =>
        App.ServiceProvider.GetRequiredService<T>();
}