namespace ProGlassAutomation.SheetStore.Models.DTOs;

public record SheetFilter(
    string? Category = null,
    string? Thickness = null,
    string? Color = null,
    string? Supplier = null,
    string? SearchText = null,
    SheetSortOption SortOption = SheetSortOption.CategoryColorThickness,
    int Page = 1,
    int PageSize = 100
);

public enum SheetSortOption
{
    CategoryColorThickness,
    ColorCategoryThickness,
    ThicknessColorCategory,
    CategoryThicknessColor,
    StockHighLow,
    BalanceHighLow,
    ValueHighLow,
    LastUpdateNewestFirst
}