# Existing theme coverage

Statuses below are **source coverage**, not visual approval.

| # | Theme | Catalog | Light | Dark | Distinct material | Customization | Studio preview | Home mosaic | A11y token contrast | Visual user accept |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | Bento Grid / Modular | Yes | Yes | Yes | Cards/borders, BentoGrid | Yes (accent, density, shape, type, presets) | Yes | Yes | Source-tested | Historically accepted tree `d1c4268`; later Liquid work did not regress Bento **in source** |
| 2 | Claymorphism | Yes | Yes | Yes | `ClaySurface` plump | Yes (volume, plumpness, sheen, …) | Yes | Yes | Source-tested | User still sending Clay Home shots with chart collapse (pre-`703797e`) |
| 3 | Fluent / Acrylic | Yes | Yes | Yes | `FluentSurface` tint/stroke | Geometry/font/color plates only — **no Fluent extras engine** | Yes | Yes | Source-tested | Not recently re-accepted |
| 4 | Neumorphism | Yes | Yes | Yes | `NeumorphSurface` | Depth/blur/intensity owned | Yes | Yes | Source-tested | Not recently re-accepted |
| 5 | Liquid Glass | Yes | Yes | Yes | `LiquidGlassSurface` + coded wallpaper | Frost/specular/tint studio | Yes | Yes | Source-tested | **OPEN** — multiple visual rejects through 4-shot after `6b22ffa`; `703797e` unconfirmed |
| 6 | Glassmorphism | **No** | — | — | — | — | — | — | — | Not started (gated) |
| 7 | Spatial / Immersive | **No** | — | — | — | — | — | — | — | Not started |
| 8 | Swiss International | **No** | — | — | — | — | — | — | — | Not started |
| 9 | Holographic / Iridescent | **No** | — | — | — | — | — | — | — | Not started |

## Shared component contract (all installed themes)

Required styles include ThemedWindow, Caption/Section/Page headings, Card/plates, buttons (primary/destructive/subtle), inputs, selections, popups, workspace (Hero, Metric, badges, TabControl/TabItem), ChartCard/KPI/DataGrid parts. Isolation requires each package to supply them.

## Honest gaps even where “Yes”

- No GPU backdrop blur anywhere.
- Charts/DataGrid remain opaque Skia / solid rows.
- Persistence file name still `bento-customization.json` (multi-theme payload).
- Feature registry (`docs/06`) is stale vs catalog.
- CompactWidth 1100 reflows Studio to 2 columns under ~1100 DIP — same for all themes.
- Nested BentoGrid (Home wraps AnalyticsOverview) still exists; inner/outer row counts currently match.
