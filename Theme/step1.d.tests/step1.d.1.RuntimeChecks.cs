using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Threading;
using ProGlassAutomation;
using ProGlassAutomation.Theming;
using ProGlassAutomation.ThemeStudio;

namespace ProGlassAutomation.ThemeTests;

/// <summary>Dependency-free STA runtime checks. Run on Windows; nonzero exit means failure.</summary>
internal static class RuntimeChecks
{
    [STAThread]
    public static int Main(string[] args)
    {
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        var failures = new BindingFailures();
        PresentationTraceSources.DataBindingSource.Listeners.Add(failures);
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning;
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        Window? window = null;
        Window? dialog = null;
        try
        {
            if (args.Length > 1 || (args.Length == 1 && args[0] != "--contracts-only"))
                throw new ArgumentException("Usage: [--contracts-only]");
            var themes = new ThemeService();
            app.Resources["UnrelatedResource"] = "preserve me";
            themes.Initialize(app.Resources);
            ContractChecks.Run();
            if (args.Contains("--contracts-only"))
            {
                Check(failures.Messages.Count == 0, "contract binding diagnostics");
                Console.WriteLine("PASS: compiled token/style/target contracts, package isolation and negative resource cases.");
                return 0;
            }
            ContractChecks.ExpectFailure(() => themes.Initialize(app.Resources), "double initialization");
            window = new MainWindow(themes);
            app.MainWindow = window;
            window.Show();
            Pump();
            Check(window.IsVisible, "startup main window is visible");
            var progress = Descendants(window).OfType<ProgressBar>().Single();
            Check(BindingOperations.GetBinding(progress, RangeBase.ValueProperty)?.Mode == BindingMode.OneWay,
                "display-only progress binding is explicitly one-way");
            var headerThemePicker = Descendants(window).OfType<ComboBox>().Single();
            Check(headerThemePicker.IsVisible, "live theme selector is visible in startup header");
            var model = (ThemeStudioViewModel)window.DataContext;
            dialog = new PreviewWindow { Owner = window, DataContext = model };
            dialog.Show();
            Pump();

            var button = Descendants(window).OfType<Button>().First(b => Equals(b.Content, "_Apply preview"));
            var input = Descendants(window).OfType<TextBox>().First(b => b.Name == "PreviewName");
            var combo = Descendants(window).OfType<ComboBox>().Single();
            combo.IsDropDownOpen = true;
            Pump();
            var popup = (Popup)combo.Template.FindName("PART_Popup", combo);
            Check(popup.IsOpen, "real selection popup opens");
            var popupBorder = (Border)popup.Child;
            input.SetCurrentValue(TextBox.TextProperty, "Unsaved editing state");
            input.GetBindingExpression(TextBox.TextProperty)!.UpdateSource();
            input.CaretIndex = 7;
            var existingButtonTemplate = button.Template;
            var existingWindowBrush = window.Background;
            model.IsDark = true;
            Pump();
            Check(themes.ActiveAppearance == Appearance.Dark && model.IsDark && !model.IsLight, "bound appearance state");
            Check(!ReferenceEquals(existingWindowBrush, window.Background), "existing window resources replaced");
            Check(Equals(window.Background, themes.GetToken("ApplicationBackground")), "main window background");
            Check(Equals(dialog.Background, themes.GetToken("ApplicationBackground")), "already-open dialog background");
            Check(Equals(button.Background, themes.GetToken("AccentPrimary")), "existing primary button");
            Check(Equals(input.Background, themes.GetToken("SurfacePrimary")), "existing input");
            Check(Equals(popupBorder.Background, themes.GetToken("SurfaceElevated")), "already-open popup resources");
            Check(combo.IsDropDownOpen && popup.IsOpen, "open selection survives appearance replacement");
            Check(ReferenceEquals(existingButtonTemplate, button.Template), "Bento template identity stays stable across modes");
            Check(input.Text == "Unsaved editing state" && input.CaretIndex == 7, "text and caret survive switching");
            Check(BindingOperations.IsDataBound(input, TextBox.TextProperty), "input binding survives editing and switching");
            combo.IsDropDownOpen = false;
            Check(Descendants(window).OfType<RadioButton>().Any(r => r.IsChecked == true && r.Content.ToString()!.Contains("Dark")), "appearance selector reflects actual mode");
            Check(model.Tokens.Count == ThemeContract.Required.Count, "live token completeness");
            Check(model.Tokens.Single(t => t.Name == "SurfacePrimary").Preview == themes.GetToken("SurfacePrimary"), "live token reference");
            Check(Equals(app.Resources["UnrelatedResource"], "preserve me"), "unrelated resources preserved");

            model.SampleName = "x";
            Pump();
            Check(Validation.GetHasError(input), "actual binding validation state");
            Check(!model.ApplyPreview.CanExecute(null), "invalid preview command disabled");
            model.SampleName = "Runtime review";
            model.ApplyPreview.Execute(null);
            Pump();
            Check(!Validation.GetHasError(input) && model.Feedback.Contains("Runtime review"), "valid preview action");
            model.AdvanceProgress.Execute(null);
            Pump();
            Check(model.Progress == 75 && progress.Value == 75, "progress command updates the displayed value");
            model.ResetPreview.Execute(null);
            Check(model.Progress == 50 && model.SampleName == "Bento review", "reset action");
            model.TokenFilter = "SurfacePrimary";
            Check(model.TokenView.Cast<TokenRow>().Count() == 1, "live token search");
            model.TokenFilter = "";
            model.ReferenceFilter = "no matching pattern";
            Check(model.ReferenceView.IsEmpty && model.ReferenceSummary.StartsWith("No reference"), "search empty state");
            model.ReferenceFilter = "";
            Check(model.ReferenceView.Cast<ReferenceSample>().Count() == 4, "reference search reset");
            model.SelectedReference = model.ReferenceItems[1];
            model.ReviewReference.Execute(null);
            Check(model.Feedback.Contains("Input & validation"), "selected sample review action");
            model.ClearPreview.Execute(null);
            Check(model.SampleName == "" && !model.ApplyPreview.CanExecute(null), "clear preview is local and validates");
            model.ResetPreview.Execute(null);

            int packageCount = app.Resources.MergedDictionaries.Count;
            window.WindowState = WindowState.Normal;
            foreach (double width in new[] { 720d, 1024d, 1440d, 1920d, 2560d })
            {
                window.Width = width;
                window.Height = 900;
                foreach (var mode in new[] { Appearance.Light, Appearance.Dark, Appearance.Light })
                {
                    themes.Apply(ThemeService.Bento, mode);
                    Pump();
                    Check(Equals(window.Background, themes.GetToken("ApplicationBackground")), $"switch at {width}");
                    Check(app.Resources.MergedDictionaries.Count == packageCount, "switching does not accumulate resource packages");
                    Check(Equals(dialog.Background, window.Background), "dialog stays synchronized");
                    var studio = Descendants(window).OfType<ThemeStudioView>().Single();
                    var modules = (Grid)studio.FindName("Modules");
                    Check((modules.ColumnDefinitions[1].ActualWidth == 0) == (modules.ActualWidth < 980), "responsive column mode");
                    Check((modules.ColumnDefinitions[2].ActualWidth > 0) == (modules.ActualWidth >= 1500), "wide three-column mode");
                    var cards = Descendants(modules).OfType<UserControl>().ToArray();
                    Check(cards.Length == 8, "all eight cards survive reflow");
                    Check(input.Text == model.SampleName && BindingOperations.IsDataBound(input, TextBox.TextProperty), "input binding survives column redistribution");
                    Check(model.SelectedReference == model.ReferenceItems[1], "selected sample survives column redistribution");
                    var bounds = new List<Rect>();
                    foreach (FrameworkElement child in cards)
                    {
                        Check(child.ActualWidth > 0 && child.ActualHeight > 0, "module remains laid out");
                        Rect box = child.TransformToAncestor(modules).TransformBounds(new Rect(child.RenderSize));
                        Check(box.Left >= -1 && box.Right <= modules.ActualWidth + 1, "module fits horizontal viewport");
                        foreach (Rect previous in bounds)
                        {
                            Rect overlap = Rect.Intersect(previous, box);
                            Check(overlap.IsEmpty || overlap.Width <= 1 || overlap.Height <= 1, "modules do not overlap");
                        }
                        bounds.Add(box);
                    }
                }
            }
            ContractChecks.ExpectFailure(() => themes.Apply("Claymorphism", Appearance.Dark), "uninstalled theme");
            ContractChecks.ExpectFailure(() => themes.Apply(ThemeService.Bento, (Appearance)100), "invalid appearance");
            Check(themes.ActiveAppearance == Appearance.Light, "invalid selection preserves active state");
            Pump();
            Check(failures.Messages.Count == 0, "binding diagnostics: " + string.Join("\n", failures.Messages));
            Console.WriteLine("PASS: contract, negative cases, commands, live tokens, existing controls/dialog, repeated switching, responsive layout and binding diagnostics.");
            Console.WriteLine("Manual review still required: visual fidelity, hover/pressed/keyboard navigation, popup/menu behavior, physical DPI, high contrast and performance.");
            return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
        finally { dialog?.Close(); window?.Close(); app.Shutdown(); }
    }

    private static void Check(bool passed, string name)
    {
        if (!passed) throw new InvalidOperationException("FAILED: " + name);
    }

    private static void Pump() => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);

    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            yield return child;
            foreach (var nested in Descendants(child)) yield return nested;
        }
    }

    private sealed class BindingFailures : TraceListener
    {
        public List<string> Messages { get; } = [];
        public override void Write(string? message) { if (!string.IsNullOrWhiteSpace(message)) Messages.Add(message); }
        public override void WriteLine(string? message) => Write(message);
    }
}
