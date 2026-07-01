using ProGlassAutomation.SheetStore.Models;

namespace ProGlassAutomation.SheetStore.Models.DTOs;

public record SheetStats(
    int TotalSheets,
    int TotalStock,
    int TotalUsed,
    int BalanceSheets,
    decimal TotalPurchaseValue,
    decimal TotalSellValue,
    decimal TotalProfitValue,
    double TotalSQM,
    double TotalUsedSQM,
    double TotalBalanceSQM)
{
    public static SheetStats Empty { get; } = new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

    public static SheetStats From(IEnumerable<Sheet> sheets)
    {
        var list = sheets.ToList();

        var totalStock = list.Sum(s => s.TotalStock);
        var totalUsed = list.Sum(s => s.UsedSheets);
        var purchase = list.Sum(s => s.TotalPurchaseValue);
        var sell = list.Sum(s => s.TotalSellValue);

        return new SheetStats(
            list.Count,
            totalStock,
            totalUsed,
            totalStock - totalUsed,
            purchase,
            sell,
            sell - purchase,
            list.Sum(s => s.TotalStockSQM),
            list.Sum(s => s.UsedSQM),
            list.Sum(s => s.BalanceSQM));
    }
}