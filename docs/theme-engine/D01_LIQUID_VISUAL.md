# D01 — Liquid Glass visual acceptance (V1)

**Date:** 2026-10-22  
**HEAD inspected:** `4b4ef3d` on `arena/f1e0ac70-proglassautomation`  
**Host:** Linux Debian 12 sandbox. `dotnet` absent. No display. No Wine. **0% WPF pixels.**

This is the **V1 / defect D01** gate (Liquid Light+Dark Home and Theme Studio). It is **not** Stage F. It is **not** a second engine.

Naming: MASTER_PLAN Stage D row “D01 Confirm 47 styles” is a **different** checklist item. That row is source-confirmed below. Visual acceptance remains this file.

## Scope

Inspect Liquid Glass Light and Dark from source. Attempt Windows render if available. Fix only D01-necessary defects. Do not start E or F.

**Result of this run:** source inspection completed. **No Liquid XAML/C# edits.** Visual gate **BLOCKED**.

## 4b4ef3d confirmation (Stage B/C still present)

| Item | Present |
| --- | --- |
| `ThemeCatalog.Sequence` (9 identities) | Yes |
| `ThemeCatalog.Installed` count 5 | Yes |
| `ThemeEditSession` | Yes |
| Colors Studio `MarginInline` + `{Binding .}` | Yes |
| Store v2 path `bento-customization.json` | Yes |
| ThemeService Overlay + slot-replace | Unchanged |
| `Theme/ThemeGlassmorphism` | Absent |

Working tree at start of this run: clean except untracked `image-search/` (not product).

## Host capability

| Check | Available |
| --- | --- |
| Read XAML/C# | Yes |
| Python isolation / foundation / verify_sources | Yes |
| `dotnet build` / STA ThemeTests | **No** |
| Launch WPF, switch Liquid Light/Dark | **No** |
| Screenshot Home / Studio | **No** |
| Chart pixel inspection | **No** |
| Resize, DPI, keyboard, performance counters | **No** |

## Source architecture (both modes)

Liquid owns `Theme/ThemeLiquidGlass/`. Loader: `LiquidResources.Load` → palette + extras + 7 component dictionaries + `LiquidWallpaper.Install`.

| Layer | Light | Dark | Distinct from Fluent? |
| --- | --- | --- | --- |
| Window | `ThemedWindow` → `LiquidCanvasBrush` (coded Tahoe sky after Install) | Night sky/moon | Fluent uses opaque `ApplicationBackground` |
| Wallpaper | `PaintDay` in-bounds sun bloom | `PaintNight` in-bounds moon | No PNG; no Fluent acrylic |
| Cards / KPI / ChartCard | `LiquidGlassSurface` Rank=Card, unnamed lift on caster | Same template, darker extras | Fluent uses `FluentSurface` tint/luminosity |
| Controls / chips | Rank Control/Chip; wells `LiquidWellBrush` | Same | |
| Inputs | `ShowFace=False` + `LiquidWellBrush` | Opaque well `#1A2C40` | Not glass-over-text field |
| DataGrid | Opaque `SurfacePrimary` rows | Opaque dark rows | Charts/grids stay solid |
| Shell sidebar / nav / banner | Opaque `SurfacePrimary` unless **IsLiquid && IsDark** | Transparent over canvas | Intentional Dark-only chrome |
| Workspace `TabControl` | **Transparent whenever `IsLiquid`** (Light and Dark) | Same | Lets wallpaper show under Home/Studio |
| Extras studio | Frost / Specular / Tint when `CanCustomizeLiquid` | Same | Fluent has **no** extras engine |

Rejected leftovers **not** found in the Liquid package: `BlurEffect`, named `DropShadowEffect`, BitmapCache, GPU Skia, CSS, desktop capture, lining shader, orb catalog, Theme 6 folders.

`ContentPresenter` sits inside Face, not on the shadow caster. Charts: `LiveCharts.RenderingSettings.UseGPU = false`. Compact chrome when plot height &lt; 160 or `ShowAxis=false` (`703797e`).

47 required styles: isolation **PASS** both modes (Stage D table D01, source only).

## Liquid Light — source findings (not rendered)

- Palette: sky `ApplicationBackground #62B4E8`, body `TextPrimary #071422`, cards via ~16% white veil (`#29FFFFFF` at Standard frost).
- Token pairs (verify_sources + desktop WCAG math): TextPrimary/SurfacePrimary **17.62:1**, TextPrimary/ApplicationBackground **8.14:1**.
- Estimated **simple** alpha-over-sky (not WPF): body on veil over `#62B4E8` ≈ **9.36:1**. **Not a screenshot.** Blooms, sheen, stacked glass, and Clear frost (thinner) are unmeasured.
- Hero uses `LiquidTintHero` (`#0A3658` family) so `HeroForeground` is light-on-dark glass, not a copy of Dark extras.
- Light sidebar stays opaque (readme). TabControl is still transparent, so Home mosaic sits on wallpaper — this is the Light visual that prior shots rejected; **cannot accept from source**.

**Light visual status:** **BLOCKED** — no WPF Light Home/Studio shot on this host.

## Liquid Glass Dark — source findings (not rendered)

- Palette: `ApplicationBackground #0A3A58`, `TextPrimary #F2F7FC`, card face `#1CFFFFFF` over night canvas.
- Token pairs: TextPrimary/SurfacePrimary **13.18:1**, TextPrimary/ApplicationBackground **11.08:1**.
- Estimated veil over `#10305C` ≈ **8.76:1**. Unrendered.
- Dark shell chrome (sidebar, nav, banner) goes transparent; wallpaper is the page. Cards remain veil + hairline + sheen.

**Dark visual status:** **BLOCKED** — no WPF Dark Home/Studio shot on this host.

## Home / Theme Studio / charts (source only)

| Surface | Source | Visual |
| --- | --- | --- |
| Home mosaic | Shared `BusinessReferenceView` 4-col BentoGrid, Liquid `BentoRowHeight` 120 | Unverified. D02/D03 chart chrome FIXED-UNVERIFIED (`703797e`) |
| Theme Studio | Same shell; Liquid extras gated; Texts/Colors global | Unverified |
| Charts | CPU Skia, compact nested chrome, Mix side legend (`23fd30d`) | Pixels **not** inspected. Charts never frost |
| Dialogs / focus / hover / pressed / disabled | Styles exist (focus ring, concave press, DisabledText) | Unverified |
| ReduceTransparency | Template swaps to `LiquidOpaqueBrush` | **Never enabled** (`LiquidCustomization.Apply` forces `false`). Fallback unexercised |

## Accessibility and performance

| Topic | Finding |
| --- | --- |
| Token 4.5:1 / focus 3:1 | Source PASS (1050 pairs across five themes × 2) |
| Effective fill on wallpaper | Estimate only; **not** V1 evidence |
| Reduce-transparency | Present, always off |
| System High Contrast | Not bound (known gap) |
| Blur / capture cost | No BlurEffect. Per-card unnamed DropShadow (Quality) **unmeasured** |
| Theme switch stale resources | Requires STA Overlay/Apply — **NOT RUN** |
| Dense ERP / resize 720–2560 | **NOT RUN** |

Do not treat the veil arithmetic as certification.

## Tests this run

| Command | Ran | Result | Limitation |
| --- | --- | --- | --- |
| `python3 Theme/step1.d.tests/step1.d.5.verify_isolation.py` | Yes | **PASS** — 5 packages, 47 styles both modes | Not pixels |
| `python3 Theme/step1.d.tests/step1.d.6.verify_foundation.py` | Yes | **PASS** — 64 Compile / 78 Page | Not pixels |
| `python3 Theme/step1.d.tests/step1.d.3.verify_sources.py` | Yes | **PASS** — tokens, contrast, no Studio hex | Not pixels |
| `python3 Theme/step1.d.tests/step1.d.4.CheckerRegression.py` | **NOT RUN** | Last PASS on `4b4ef3d`; no source mutation this run | — |
| `dotnet build` / ThemeTests STA | **NOT RUN** | SDK absent | Windows |
| Launch app, Liquid Light/Dark screenshots | **NOT RUN** | No WPF host | Windows |
| Chart labels/legends on screen | **NOT RUN** | | Windows |

No screenshots produced. None fabricated.

## Defects found / fixes applied

No new Liquid styling defect was **proven** in source strongly enough to restyle without repeating prior rejects (L7 opaque, frost plates, lining, wash, orbs, Light-break).

**No product code changed in this run.**

Still open from the register: D01 visual, D02/D03 chart pixels, D05 no Windows, D09 glass-over-wallpaper measurement.

## Final D01 status

**BLOCKED** — pending actual WPF rendering of Liquid Light and Dark on Home and Theme Studio, including charts.

Rationale: V1 requires user-visible Windows shots. Source PASS is not runtime PASS. A successful mental compile is not acceptance.

## Next authorized phase

Remain on **D01 / V1** until a Windows host (or owner-supplied shots of this tree) inspects Light+Dark Home and Studio.

Do **not** start Stage E (Studio completeness) or Stage F (Glassmorphism) from this run.

After V1 is accepted, MASTER_PLAN critical path continues: **E → F (blocked until V1) → G → H → I**.
