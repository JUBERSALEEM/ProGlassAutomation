using System.Windows;
using System.Windows.Controls;

namespace ProGlassAutomation.SheetStore.Navigation;

public interface INavigationService
{
    void SetContentControl(ContentControl control);
    void NavigateTo<TView>() where TView : UserControl;
    void NavigateTo<TView>(object parameter) where TView : UserControl;
    bool CanGoBack { get; }
}

public interface INavigationAware
{
    void OnNavigatedTo(object? parameter);
    void OnNavigatedFrom();
}