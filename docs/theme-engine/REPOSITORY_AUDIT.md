# Repository audit

**Branch:** `arena/f1e0ac70-proglassautomation`  
**HEAD at audit:** `703797e` (Stop LiveCharts axis chrome from flattening nested plots.)  
**Date:** 2026-10-22  
**Working tree:** clean except untracked `image-search/` (not product).

This audit traces **actual paths**. Items are not “missing” merely because a name search failed.

## 1. Solution and TFM

| Item | Fact |
| --- | --- |
| Solution | `ProGlassAutomation.slnx` (SDK 8 does not consume `.slnx` in CI; projects are restored by path) |
| App project | `ProGlassAutomation.csproj` — `net8.0-windows7.0`, `UseWPF`, `EnableWindowsTargeting` |
| Test project | `Theme/step1.d.tests/step1.d.ThemeTests.csproj` |
| Package | **Only** `LiveChartsCore.SkiaSharpView.WPF` **2.0.4** |
| Default items | Disabled; explicit globs for Theme + Studio |
| Manifest | `app.manifest` (DPI) |
| Version | 3.0 / InformationalVersion 3.0 |

There is **no DI container**. `App` constructs `ThemeService` and `MainWindow`.

## 2. Entry and shell

- `App.xaml` — empty resources (engine fills at startup).
- `App.xaml.cs` — `Themes.Initialize` → `CustomizationStore.Load` → `Themes.Apply` with `SharedGeometry.Compose` → `MainWindow.Show`. Corrupt JSON is swallowed (store already returns defaults).
- `MainWindow.xaml` — sidebar + top panel + `TabControl` workspaces (Home, Theme Studio, Texts, Colors). Liquid TabControl background is transparent when `IsLiquid`. Shell glass gated `IsLiquid && IsDark` only.
- Startup workspace: Home (`selectedWorkspace = 1`).

Shell rule (standing): **sidebar + top panel + content**. Theme engine is not the window. Theme lives under **Settings** in the sidebar.

## 3. Theme engine (non-visual)

Directory: `Theme/step1.a.ThemeEngine/`

| File | Role |
| --- | --- |
| `step1.a.1.ThemeContract.cs` | Typed token dictionary (~90 keys) + 47 required styles/targets |
| `step1.a.2.ThemeService.cs` | Initialize / Apply (atomic slot replace) / Overlay / Load |
| `step1.a.3.ResourceValidation.cs` | Type, finite numbers, required styles, no duplicate merged keys |
| `step1.a.6.ThemeCatalog.cs` | **Five** installed definitions only. Comment: *Register only complete, authorized themes. No placeholder entries.* |
| `step1.a.7.ThemeNotificationException.cs` | Subscriber failure after successful apply |

**Installed names (exact):**

1. `Bento Grid / Modular`
2. `Claymorphism`
3. `Fluent / Acrylic`
4. `Neumorphism`
5. `Liquid Glass`

Not installed: Glassmorphism, Spatial / Immersive, Swiss International, Holographic / Iridescent.

## 4. Visual packages (theme-owned)

Each of the five has `*.Light.xaml` + `*.Dark.xaml` (contract tokens), optional `*.Extras.xaml` (theme-owned material), `step1.e.7.*Resources.cs` loader, and `step1.e.Components/` (Foundation, Buttons, Inputs, Selections, Popups, Workspace, Analytics).

| Theme | Geometry | Extra engine | Notes |
| --- | --- | --- | --- |
| Bento | SharedTokens | `BentoCustomization` density/accent/layout presets | Layout presets exist; **Layout Studio overlay was hard-reset off** (`d1c4268`). Do not restore. |
| Clay | ThemeOwned plumpness | `ClaySurface`, extras | Shadows not on named effects |
| Fluent | SharedTokens | `FluentSurface` tint/stroke/luminosity only | **No Fluent extras/lighting engine** (standing) |
| Neumorph | SharedTokens rounding; owned depth | `NeumorphSurface` | |
| Liquid | SharedTokens + extras frost | `LiquidGlassSurface`, `LiquidWallpaper` coded scene | Visual **not accepted**. No PNG wallpaper, no scene catalog |

Isolation test: no XAML in the engine folder; no cross-theme `Source=`; 47 styles both modes.

## 5. Customization and persistence

`Theme/step1.g.Customization/`:

- `CustomizationStore` — `%LOCALAPPDATA%\ProGlassAutomation\bento-customization.json`, max 64 KB, version **2**, atomic-ish load with defaults on error.
- States: Bento, Clay, Neumorph, Liquid, SharedGeometry, Font, Color plates, presets.
- **No Fluent extras state** (correct).
- `FontService`, `ColorPlateService` — global Texts/Colors Studio.
- `PresetCatalog` — Bento built-ins.

Studio VM: `step1.b.17.Customization.cs` + `step1.b.3.ThemeStudioViewModel.cs`. Save/revert/reset exist as commands. **No ExcelSticker-style `ThemeEditSession` type.**

## 6. Theme Studio screens

`step1.ThemeStudio/step1.b.ThemeStudioScreen/` — Theme Studio mosaic, Home (`BusinessReferenceView`), ChartGallery, AnalyticsOverview, CustomizationPanel, Texts, Colors, ChartLab, Tokens (OPTIONAL / RESOURCE DIAGNOSTICS), etc.

Home/Studio layout is **shared XAML**; themes only restyle. Collapse bugs were ClipToBounds + axis chrome, not per-theme slot maps.

## 7. Analytics

`Theme/step1.f.Analytics/` — `BentoChart` / `BentoKpi` / immutable `ChartDataset`. No theme lookup, no sample data in the renderer. CPU Skia.

## 8. Tests

| Script / assembly | What it is | What it is not |
| --- | --- | --- |
| `step1.d.3.verify_sources.py` | Tokens, contrast math, XML, no view-local Margin literals in Studio | Currently **fails baseline** on pre-existing `ColorsStudioView.xaml` `Margin="0,0,2,0"` |
| `step1.d.5.verify_isolation.py` | Package isolation | Visual |
| `step1.d.6.verify_foundation.py` | Globs, no legacy, Compact+FillAvailable | Visual |
| `step1.d.4.CheckerRegression.py` | Mutations of verify_sources | Blocked by baseline fail |
| `step1.d.ThemeTests` STA | Contracts + runtime smoke | **Cannot run here** (no Windows, no dotnet) |
| GitHub Actions | `cols/ci/windows-wpf.yml` **inactive**; `.github/workflows` not owned by this App | 403 if published without workflows permission |

## 9. Docs already in tree

- `docs/00–06` governance. **`docs/04.Project-Roadmap.md` lists an obsolete eight-theme order** (Spatial as 3, no Fluent). **Superseded** by the 2026-10-22 nine-theme mission. Do not treat 04 as the live sequence.
- `docs/06.Feature-Registry.md` still says Clay unimplemented — **stale**. Clay through Liquid exist in source.
- `step1.ThemeStudio/step1.c.docs/` — Phase 01 theme-by-theme notes (c.12 Clay … c.18 Liquid).
- This folder (`docs/theme-engine/`) is the **authoritative** engine/Studio/Glassmorphism roadmap from the 2026-10-22 mission.

## 10. What is not in this repository

- ERP calculations, invoices, stock, print, licensing — **none**. Theme work cannot regress them because they are absent.
- Glassmorphism / Spatial / Swiss / Holographic packages.
- A second settings database.

## 11. Standing product constraints that survived the audit

Stay Phase 01 / step1 (no new feature number). Native WPF. No PR without two confirmations. Do not start theme 6 until Liquid Light+Dark are **user-accepted**. Do not mix themes. Do not relocate `windows-wpf.yml`. Never PNG in product source. Layout overlay stays dead.
