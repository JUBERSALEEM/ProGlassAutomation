namespace ProGlassAutomation.SheetStore.Services;

internal static class SqlSchema
{
    public const string InitialSchema = @"
        CREATE TABLE IF NOT EXISTS Suppliers (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL UNIQUE,
            Contact TEXT,
            Phone TEXT,
            Email TEXT,
            IsActive INTEGER NOT NULL DEFAULT 1,
            CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
        );

        CREATE TABLE IF NOT EXISTS Sheets (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Category TEXT NOT NULL,
            Thickness TEXT NOT NULL,
            Color TEXT NOT NULL,
            Width REAL NOT NULL,
            Height REAL NOT NULL,
            SquareMeter REAL NOT NULL,
            Description TEXT,
            DefaultPurchasePrice REAL NOT NULL DEFAULT 0,
            DefaultSellPrice REAL NOT NULL DEFAULT 0,
            IsActive INTEGER NOT NULL DEFAULT 1,
            CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
            UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
        );

        CREATE TABLE IF NOT EXISTS SheetPurchases (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SheetId INTEGER NOT NULL,
            Quantity INTEGER NOT NULL CHECK (Quantity > 0),
            UnitPrice REAL NOT NULL CHECK (UnitPrice >= 0),
            SupplierName TEXT NOT NULL,
            PurchasedOn TEXT NOT NULL,
            CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
            FOREIGN KEY (SheetId) REFERENCES Sheets(Id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS SheetUsages (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SheetId INTEGER NOT NULL,
            Quantity INTEGER NOT NULL CHECK (Quantity > 0),
            Reason TEXT,
            UsedOn TEXT NOT NULL,
            CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
            FOREIGN KEY (SheetId) REFERENCES Sheets(Id) ON DELETE CASCADE
        );

        CREATE VIEW IF NOT EXISTS vw_SheetInventory AS
        SELECT
            s.Id,
            s.Category, s.Thickness, s.Color, s.Width, s.Height, s.SquareMeter,
            s.Description, s.DefaultPurchasePrice, s.DefaultSellPrice,
            s.IsActive, s.CreatedAt, s.UpdatedAt,
            COALESCE(SUM(sp.Quantity), 0) AS TotalStock,
            COALESCE((SELECT SUM(Quantity) FROM SheetUsages WHERE SheetId = s.Id), 0) AS UsedSheets,
            COALESCE(SUM(sp.Quantity), 0) -
                COALESCE((SELECT SUM(Quantity) FROM SheetUsages WHERE SheetId = s.Id), 0) AS BalanceSheets,
            (SELECT MAX(PurchasedOn) FROM SheetPurchases WHERE SheetId = s.Id) AS LatestPurchaseDate
        FROM Sheets s
        LEFT JOIN SheetPurchases sp ON s.Id = sp.SheetId
        WHERE s.IsActive = 1
        GROUP BY s.Id;
    ";

    public const string Indexes = @"
        CREATE INDEX IF NOT EXISTS idx_sheets_category ON Sheets(Category);
        CREATE INDEX IF NOT EXISTS idx_sheets_thickness ON Sheets(Thickness);
        CREATE INDEX IF NOT EXISTS idx_sheets_color ON Sheets(Color);
        CREATE INDEX IF NOT EXISTS idx_sheets_active ON Sheets(IsActive);
        CREATE INDEX IF NOT EXISTS idx_purchases_sheet ON SheetPurchases(SheetId);
        CREATE INDEX IF NOT EXISTS idx_purchases_date ON SheetPurchases(PurchasedOn DESC);
        CREATE INDEX IF NOT EXISTS idx_usages_sheet ON SheetUsages(SheetId);
        CREATE INDEX IF NOT EXISTS idx_sheets_combo ON Sheets(Category, Thickness, Color, IsActive);
    ";
}