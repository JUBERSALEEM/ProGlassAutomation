using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.SheetStore.Models.DTOs;

public record ImportResult(
    int ImportedCount,
    IReadOnlyList<string> Errors,
    IReadOnlyList<Sheet>? Sheets = null)
{
    public bool HasErrors => Errors.Count > 0;
}