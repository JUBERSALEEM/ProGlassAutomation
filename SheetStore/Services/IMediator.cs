namespace ProGlassAutomation.SheetStore.Services;

public interface IMediator
{
    void Publish<T>(T message) where T : class;
    void Subscribe<T>(Action<T> handler) where T : class;
    void Unsubscribe<T>(Action<T> handler) where T : class;
}

public sealed class Mediator : IMediator
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();
    private readonly object _lock = new();

    public void Publish<T>(T message) where T : class
    {
        List<Delegate>? handlers;
        lock (_lock)
        {
            if (!_handlers.TryGetValue(typeof(T), out handlers)) return;
            handlers = handlers.ToList();
        }
        foreach (var handler in handlers)
            ((Action<T>)handler).Invoke(message);
    }

    public void Subscribe<T>(Action<T> handler) where T : class
    {
        lock (_lock)
        {
            if (!_handlers.TryGetValue(typeof(T), out var list))
            {
                list = new List<Delegate>();
                _handlers[typeof(T)] = list;
            }
            list.Add(handler);
        }
    }

    public void Unsubscribe<T>(Action<T> handler) where T : class
    {
        lock (_lock)
        {
            if (_handlers.TryGetValue(typeof(T), out var list))
                list.Remove(handler);
        }
    }
}