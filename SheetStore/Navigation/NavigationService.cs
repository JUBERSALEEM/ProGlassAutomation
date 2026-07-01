using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ProGlassAutomation.SheetStore.Navigation;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<string> _history = new();
    private ContentControl? _contentControl;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool CanGoBack => _history.Count > 1;

    public void SetContentControl(ContentControl control) => _contentControl = control;

    public void NavigateTo<TView>() where TView : UserControl => NavigateTo<TView>(null);

    public void NavigateTo<TView>(object parameter) where TView : UserControl
    {
        if (_contentControl is null)
            throw new InvalidOperationException("ContentControl not set. Call SetContentControl first.");

        if (_contentControl.Content is UserControl current &&
            current.DataContext is INavigationAware oldAware)
            oldAware.OnNavigatedFrom();

        var view = _serviceProvider.GetRequiredService<TView>();
        if (view.DataContext is INavigationAware newAware)
            newAware.OnNavigatedTo(parameter);

        _contentControl.Content = view;
        _history.Push(typeof(TView).Name);
    }
}