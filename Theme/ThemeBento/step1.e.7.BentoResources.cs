using System.Collections.ObjectModel;
using System.Windows;
using ProGlassAutomation.Theming;

namespace ProGlassAutomation.ThemeBento;

/// <summary>Bento owns its visual composition; the shared engine knows no templates.</summary>
public static class BentoResources
{
    private static IReadOnlyDictionary<object, Style>? components;

    public static ResourceDictionary Load(Appearance appearance)
    {
        Application.Current.Dispatcher.VerifyAccess();
        if (!Enum.IsDefined(appearance)) throw new ArgumentOutOfRangeException(nameof(appearance));
        components ??= LoadComponents();
        var package = new ResourceDictionary { Source = Resource($"ThemeBento.{appearance}.xaml") };
        // Reuse sealed style identities, never mutable dictionaries or cached brushes.
        // Open selections/editing survive mode changes; each candidate has its own keys.
        var styles = new ResourceDictionary();
        foreach (var (key, style) in components) styles.Add(key, style);
        package.MergedDictionaries.Add(styles);
        return package;
    }

    private static IReadOnlyDictionary<object, Style> LoadComponents()
    {
        var values = new Dictionary<object, Style>();
        Collect(new ResourceDictionary { Source = Resource("step1.e.Components/step1.e.1.Components.xaml") }, values);
        return new ReadOnlyDictionary<object, Style>(values);
    }

    private static void Collect(ResourceDictionary dictionary, Dictionary<object, Style> values)
    {
        foreach (object key in dictionary.Keys)
        {
            if (dictionary[key] is not Style style || values.ContainsKey(key))
                throw new InvalidOperationException($"Bento has a non-style or duplicate component: {key}");
            style.Seal();
            values.Add(key, style);
        }
        foreach (ResourceDictionary child in dictionary.MergedDictionaries) Collect(child, values);
    }

    private static Uri Resource(string file) => new(
        $"pack://application:,,,/ProGlassAutomation;component/Theme/ThemeBento/{file}", UriKind.Absolute);
}
