using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using ProGlassAutomation.SheetStore.Models;
using ProGlassAutomation.SheetStore.Models.DTOs;

namespace ProGlassAutomation.SheetStore.Services;

public sealed class SqliteRepository : ISheetStoreRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqliteRepository> _logger;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public SqliteRepository(string databasePath, ILogger<SqliteRepository> logger)
    {
        _connectionString = $"Data Source={databasePath};Cache=Shared;Pooling=True";
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;

            await using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            // ✅ Apply embedded schema (no .sql files needed)
            _logger.LogInformation("Applying initial schema");
            await conn.ExecuteAsync(SqlSchema.InitialSchema);

            // ✅ Apply indexes
            _logger.LogInformation("Applying indexes");
            await conn.ExecuteAsync(SqlSchema.Indexes);

            _initialized = true;
            _logger.LogInformation("SheetStore database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task<SqliteConnection> OpenAsync()
    {
        await InitializeAsync();
        var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public async Task<IReadOnlyList<Sheet>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM vw_SheetInventory ORDER BY Category, Color, Thickness";
        await using var conn = await OpenAsync();
        var rows = await conn.QueryAsync<SheetRow>(sql);
        return rows.Select(MapRowToSheet).ToList();
    }

    public async Task<Sheet?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM vw_SheetInventory WHERE Id = @Id";
        await using var conn = await OpenAsync();
        var row = await conn.QueryFirstOrDefaultAsync<SheetRow>(sql, new { Id = id });
        return row is null ? null : MapRowToSheet(row);
    }

    public async Task<int> AddAsync(Sheet sheet, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO Sheets (Category, Thickness, Color, Width, Height, SquareMeter,
                              Description, DefaultPurchasePrice, DefaultSellPrice, IsActive)
            VALUES (@Category, @Thickness, @Color, @Width, @Height, @SquareMeter,
                    @Description, @DefaultPurchasePrice, @DefaultSellPrice, 1);
            SELECT last_insert_rowid();";

        await using var conn = await OpenAsync();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            sheet.Category,
            sheet.Thickness,
            sheet.Color,
            sheet.Width,
            sheet.Height,
            sheet.SquareMeter,
            sheet.Description,
            DefaultPurchasePrice = sheet.PurchasePrice,
            DefaultSellPrice = sheet.SellPrice
        });
    }

    public async Task UpdateAsync(Sheet sheet, CancellationToken ct = default)
    {
        const string sql = @"
            UPDATE Sheets SET
                Category = @Category, Thickness = @Thickness, Color = @Color,
                Width = @Width, Height = @Height, SquareMeter = @SquareMeter,
                Description = @Description,
                DefaultPurchasePrice = @DefaultPurchasePrice,
                DefaultSellPrice = @DefaultSellPrice,
                UpdatedAt = datetime('now')
            WHERE Id = @Id";

        await using var conn = await OpenAsync();
        await conn.ExecuteAsync(sql, new
        {
            sheet.Id,
            sheet.Category,
            sheet.Thickness,
            sheet.Color,
            sheet.Width,
            sheet.Height,
            sheet.SquareMeter,
            sheet.Description,
            DefaultPurchasePrice = sheet.PurchasePrice,
            DefaultSellPrice = sheet.SellPrice
        });
    }

    public async Task SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        const string sql = "UPDATE Sheets SET IsActive = 0, UpdatedAt = datetime('now') WHERE Id = @Id";
        await using var conn = await OpenAsync();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task AddPurchaseAsync(SheetPurchase purchase, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO SheetPurchases (SheetId, Quantity, UnitPrice, SupplierName, PurchasedOn, CreatedAt)
            VALUES (@SheetId, @Quantity, @UnitPrice, @Supplier, @PurchasedOn, datetime('now'))";

        await using var conn = await OpenAsync();
        await conn.ExecuteAsync(sql, new
        {
            purchase.SheetId,
            purchase.Quantity,
            purchase.UnitPrice,
            Supplier = purchase.Supplier,
            PurchasedOn = purchase.PurchasedOn.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    public async Task AddUsageAsync(SheetUsage usage, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO SheetUsages (SheetId, Quantity, Reason, UsedOn, CreatedAt)
            VALUES (@SheetId, @Quantity, @Reason, @UsedOn, datetime('now'))";

        await using var conn = await OpenAsync();
        await conn.ExecuteAsync(sql, new
        {
            usage.SheetId,
            usage.Quantity,
            usage.Reason,
            UsedOn = usage.UsedOn.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    public async Task<IReadOnlyList<Supplier>> GetSuppliersAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM Suppliers WHERE IsActive = 1 ORDER BY Name";
        await using var conn = await OpenAsync();
        var result = await conn.QueryAsync<Supplier>(sql);
        return result.ToList();
    }

    public async Task<int> AddSupplierAsync(Supplier supplier, CancellationToken ct = default)
    {
        // ✅ FIXED: was "last_insert_rowrowid()" - typo
        const string sql = @"
            INSERT INTO Suppliers (Name, Contact, Phone, Email, IsActive)
            VALUES (@Name, @Contact, @Phone, @Email, 1);
            SELECT last_insert_rowid();";

        await using var conn = await OpenAsync();
        return await conn.ExecuteScalarAsync<int>(sql, supplier);
    }

    public async Task<int> BulkInsertAsync(IEnumerable<Sheet> sheets, CancellationToken ct = default)
    {
        const string sql = @"
            INSERT INTO Sheets (Category, Thickness, Color, Width, Height, SquareMeter,
                              Description, DefaultPurchasePrice, DefaultSellPrice, IsActive)
            VALUES (@Category, @Thickness, @Color, @Width, @Height, @SquareMeter,
                    @Description, @DefaultPurchasePrice, @DefaultSellPrice, 1)";

        await using var conn = await OpenAsync();
        using var transaction = await conn.BeginTransactionAsync(ct);
        int count = 0;
        foreach (var sheet in sheets)
        {
            await conn.ExecuteAsync(sql, new
            {
                sheet.Category,
                sheet.Thickness,
                sheet.Color,
                sheet.Width,
                sheet.Height,
                sheet.SquareMeter,
                sheet.Description,
                DefaultPurchasePrice = sheet.PurchasePrice,
                DefaultSellPrice = sheet.SellPrice
            }, transaction);
            count++;
        }
        await transaction.CommitAsync(ct);
        return count;
    }

    public async Task<SheetStats> GetGlobalStatsAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT
                COUNT(*) AS TotalSheets,
                COALESCE(SUM(TotalStock), 0) AS TotalStock,
                COALESCE(SUM(UsedSheets), 0) AS TotalUsed,
                COALESCE(SUM(BalanceSheets), 0) AS BalanceSheets
            FROM vw_SheetInventory";

        await using var conn = await OpenAsync();
        var row = await conn.QueryFirstAsync(sql);
        return new SheetStats(
            (int)row.TotalSheets,
            (int)row.TotalStock,
            (int)row.TotalUsed,
            (int)row.BalanceSheets,
            0, 0, 0, 0, 0, 0);
    }

    private static Sheet MapRowToSheet(SheetRow row) => new()
    {
        Id = row.Id,
        Category = row.Category,
        Thickness = row.Thickness,
        Color = row.Color,
        Width = row.Width,
        Height = row.Height,
        SquareMeter = row.SquareMeter,
        Description = row.Description ?? string.Empty,
        PurchasePrice = (decimal)row.DefaultPurchasePrice,
        SellPrice = (decimal)row.DefaultSellPrice,
        TotalStock = (int)row.TotalStock,
        UsedSheets = (int)row.UsedSheets,
        CreatedDate = DateTime.Parse(row.CreatedAt),
        LatestPurchaseDate = row.LatestPurchaseDate != null
            ? DateTime.Parse(row.LatestPurchaseDate) : null,
        IsActive = row.IsActive == 1
    };

    private sealed class SheetRow
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Thickness { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double SquareMeter { get; set; }
        public string? Description { get; set; }
        public double DefaultPurchasePrice { get; set; }
        public double DefaultSellPrice { get; set; }
        public int IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string? LatestPurchaseDate { get; set; }
        public int TotalStock { get; set; }
        public int UsedSheets { get; set; }
        public int BalanceSheets { get; set; }
    }
}