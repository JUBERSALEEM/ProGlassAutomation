using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace ProGlassAutomation.ThemeStudio;

/// <summary>Local illustrative content only: no persistence, ERP service or database.</summary>
public sealed record ReferenceSample(string Title, string Owner, string Updated, string Status);

public sealed partial class ThemeStudioViewModel
{
    private string referenceFilter = "";
    private ReferenceSample? selectedReference;
    public IReadOnlyList<ReferenceSample> ReferenceItems { get; } =
    [
        new("Page foundations", "Design operations", "Today", "Ready"),
        new("Input & validation", "Interaction review", "Today", "Pending"),
        new("Surface hierarchy", "Visual review", "Yesterday", "Review"),
        new("Keyboard journey", "Accessibility review", "Yesterday", "Not verified")
    ];
    public ICollectionView ReferenceView { get; private set; } = null!;
    public string ReferenceFilter
    {
        get => referenceFilter;
        set { referenceFilter = value; ReferenceView.Refresh(); Changed(nameof(ReferenceFilter)); Changed(nameof(ReferenceSummary)); }
    }
    public ReferenceSample? SelectedReference
    {
        get => selectedReference;
        set { selectedReference = value; Changed(nameof(SelectedReference)); CommandManager.InvalidateRequerySuggested(); }
    }
    public string ReferenceSummary => ReferenceView.IsEmpty
        ? "No reference items match. Clear the search to show all patterns."
        : $"{ReferenceView.Cast<ReferenceSample>().Count()} reference patterns · Sample data only";
    public ICommand ReviewReference { get; private set; } = null!;
    public ICommand ClearPreview { get; private set; } = null!;
    public ICommand ShowHelp { get; private set; } = null!;

    private void InitializeReferenceSamples()
    {
        ReferenceView = CollectionViewSource.GetDefaultView(ReferenceItems);
        ReferenceView.Filter = item => item is ReferenceSample row &&
            $"{row.Title} {row.Owner} {row.Status}".Contains(referenceFilter, StringComparison.OrdinalIgnoreCase);
        SelectedReference = ReferenceItems[0];
        ReviewReference = new DelegateCommand(() => Feedback = $"Review selected: {SelectedReference!.Title} (preview only).", () => SelectedReference is not null);
        ClearPreview = new DelegateCommand(() => { SampleName = ""; Feedback = "Preview name cleared. No saved data was changed. Reset restores the sample."; });
        ShowHelp = new DelegateCommand(() => Feedback = "Reference guide: Tab moves focus, arrows change selections, Alt+Down opens the theme selector. Changes are local previews.");
    }
}
