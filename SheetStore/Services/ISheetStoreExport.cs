using CsvHelper;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.Models.DTOs;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;

namespace ProGlassAutomation.SheetStore.Services;

public interface ISheetStoreExport
{
    Task ExportToCsvAsync(IEnumerable<Sheet> sheets, string filePath, CancellationToken ct = default);
    Task<ImportResult> ImportFromCsvAsync(string filePath, CancellationToken ct = default);
}

public sealed class SheetStoreExport : ISheetStoreExport
{
    public async Task ExportToCsvAsync(IEnumerable<Sheet> sheets, string filePath, CancellationToken ct = default)
    {
        await using var writer = new StreamWriter(filePath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteField("Category"); csv.WriteField("Thickness"); csv.WriteField("Color");
        csv.WriteField("Width"); csv.WriteField("Height"); csv.WriteField("Supplier");
        csv.WriteField("PurchasePrice"); csv.WriteField("SellPrice");
        csv.WriteField("TotalStock"); csv.WriteField("UsedSheets");
        await csv.NextRecordAsync();

        foreach (var sheet in sheets)
        {
            csv.WriteField(sheet.Category); csv.WriteField(sheet.Thickness); csv.WriteField(sheet.Color);
            csv.WriteField(sheet.Width); csv.WriteField(sheet.Height); csv.WriteField(sheet.Supplier);
            csv.WriteField(sheet.PurchasePrice); csv.WriteField(sheet.SellPrice);
            csv.WriteField(sheet.TotalStock); csv.WriteField(sheet.UsedSheets);
            await csv.NextRecordAsync();
        }
        await csv.FlushAsync();
    }

    public async Task<ImportResult> ImportFromCsvAsync(string filePath, CancellationToken ct = default)
    {
        var sheets = new List<Sheet>();
        var errors = new List<string>();

        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                try
                {
                    var sheet = new Sheet
                    {
                        Category = csv.GetField("Category") ?? string.Empty,
                        Thickness = csv.GetField("Thickness") ?? string.Empty,
                        Color = csv.GetField("Color") ?? string.Empty,
                        Width = double.Parse(csv.GetField("Width") ?? "0"),
                        Height = double.Parse(csv.GetField("Height") ?? "0"),
                        Supplier = csv.GetField("Supplier") ?? string.Empty,
                        PurchasePrice = decimal.Parse(csv.GetField("PurchasePrice") ?? "0"),
                        SellPrice = decimal.Parse(csv.GetField("SellPrice") ?? "0"),
                        TotalStock = int.Parse(csv.GetField("TotalStock") ?? "0"),
                        UsedSheets = int.Parse(csv.GetField("UsedSheets") ?? "0")
                    };
                    sheet.SquareMeter = sheet.Width * sheet.Height;
                    sheets.Add(sheet);
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {csv.Context.Parser.Row}: {ex.Message}");
                }
            }
            return new ImportResult(sheets.Count, errors, sheets);
        }
        catch (Exception ex)
        {
            return new ImportResult(0, new List<string> { ex.Message });
        }
    }
}