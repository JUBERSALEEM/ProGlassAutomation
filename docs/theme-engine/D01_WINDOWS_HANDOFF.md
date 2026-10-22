# D01 — Windows visual verification handoff

**Status of D01 / V1: BLOCKED.** Do not mark VERIFIED from Linux, from `dotnet build` alone, or from STA smoke without human shots.

**Tree to inspect:** branch `arena/f1e0ac70-proglassautomation`. Record `git rev-parse HEAD` on the Windows machine and write that hash on every screenshot. Do **not** include untracked `image-search/`.

This annex is the operator procedure for [D01_LIQUID_VISUAL.md](D01_LIQUID_VISUAL.md). It does not start Stage E or F. It does not authorize product code changes.

## 1. Prerequisites (from the repo)

| Item | Source in tree | Value |
| --- | --- | --- |
| OS | `README.md`, `cols/ci/windows-wpf.yml` | Windows 10/11 (CI image `windows-2022`) |
| SDK | `ProGlassAutomation.csproj`, CI `dotnet-version: '8.0.x'` | .NET **8** SDK. No `global.json` in the repo (CI may write a temporary pin). |
| TFM | csproj | `net8.0-windows7.0` — API baseline, **not** a Windows 7 support claim |
| UI stack | csproj `<UseWPF>true` | Native WPF. Browser preview is not evidence. |
| App project | `ProGlassAutomation.csproj` | `OutputType` WinExe, Version 3.0.0 |
| Tests project | `Theme/step1.d.tests/step1.d.ThemeTests.csproj` | Exe, project-reference to the app |
| Chart package | csproj PackageReference | `LiveChartsCore.SkiaSharpView.WPF` **2.0.4** only |
| Manifest | `app.manifest` | Per-monitor DPI (`PerMonitorV2`) |
| Persistence | `CustomizationStore.FilePath` | `%LOCALAPPDATA%\ProGlassAutomation\bento-customization.json` |
| CI workflow | `cols/ci/windows-wpf.yml` | **Inactive** (not under `.github/workflows/`). Do not relocate it. |

Do not build `.slnx` on SDK 8 (CI comment: SDK 8 does not consume `.slnx`; use the csproj paths).

Suggested known-start: rename or move the persistence file so the first launch is not a leftover save. The Theme Studio VM still restores that file if it exists.

## 2. Build and launch (exact commands)

From the repository root, PowerShell, matching [README.md](../../README.md) and [cols/ci/windows-wpf.yml](../../cols/ci/windows-wpf.yml):

```powershell
git status
git rev-parse HEAD
git branch --show-current

python Theme/step1.d.tests/step1.d.3.verify_sources.py
python Theme/step1.d.tests/step1.d.5.verify_isolation.py
python Theme/step1.d.tests/step1.d.6.verify_foundation.py

dotnet restore ProGlassAutomation.csproj
dotnet restore Theme/step1.d.tests/step1.d.ThemeTests.csproj
dotnet build ProGlassAutomation.csproj --configuration Release --no-restore
dotnet build Theme/step1.d.tests/step1.d.ThemeTests.csproj --configuration Release --no-restore
dotnet run --project Theme/step1.d.tests/step1.d.ThemeTests.csproj --configuration Release --no-build --no-restore -- --contracts-only
dotnet run --project Theme/step1.d.tests/step1.d.ThemeTests.csproj --configuration Release --no-build --no-restore
dotnet run --project ProGlassAutomation.csproj --configuration Release
```

Checker regression (optional on Windows if time allows; last Linux PASS was `4b4ef3d`):

```powershell
python Theme/step1.d.tests/step1.d.4.CheckerRegression.py
```

SDK-style output (do not invent other drop folders):

- App: `bin/Release/net8.0-windows7.0/ProGlassAutomation.exe`
- Tests: `Theme/step1.d.tests/bin/Release/net8.0-windows7.0/`

**Classification:** portable Python = source. `dotnet build` = compile. `--contracts-only` = compiled tokens. STA `ThemeTests` without the flag = runtime smoke **including a MainWindow**, **not** V1 visual accept. Only the launched app plus screenshots can move D01.

### STA note (do not “fix” in this handoff)

`RuntimeChecks` asserts `window.Background == ApplicationBackground` after each theme. Liquid `ThemedWindow` sets `Background` to `LiquidCanvasBrush` (wallpaper). If STA fails that assertion on Liquid, record it as a **test vs theme** mismatch. Do not restyle Liquid to satisfy the assert without a separate approval.

## 3. How to reach Liquid Light / Dark

Startup workspace is **Home** (`SelectedWorkspace = 1`).

1. Sidebar **Theme Studio** (or keep Home visible after the theme is live).
2. **Theme** combo (`ThemePicker`): choose **`Liquid Glass`** (catalog name; five installed themes only).
3. **Light** / **Dark** radios (`IsLight` / `IsDark`), or the top-banner **Dark** toggle (`ToggleAppearance`).
4. Confirm the Liquid extras card is visible (`IsLiquid`): Frost / Specular / Tint. Fluent must not show a material extras engine.
5. Sidebar **Home** for mosaic/charts. **Theme Studio** again for controls.
6. Closed sidebar: only the hamburger; reopen to navigate.

Do not register or select Glassmorphism. Themes 6–9 are not in `Installed`.

## 4. Manual verification checklist

Tick on Windows only. Record Pass / Fail / N/A and a shot ID.

### 4.1 Liquid Glass Light — Home

- [ ] Window is a normal app chrome (`AllowsTransparency="False"`). Sky/canvas visible through the workspace (`TabControl` is transparent when Liquid).
- [ ] Sidebar / nav stay **opaque** on Light (transparent chrome is Dark-only in `MainWindow`).
- [ ] Cards read as glass (veil, hairline, sheen), not flat Bento and not Fluent acrylic.
- [ ] Hero / KPI / activity / table text is readable on the effective fill (not token math).
- [ ] No overlap, clipped labels, or empty chart holes in the Home mosaic.
- [ ] Not a prior reject: L7 opaque slabs, frost plates, lining, wash, orbs.

### 4.2 Liquid Glass Light — Theme Studio

- [ ] Theme picker, Light/Dark, rounding, Import/Export, Texts/Colors buttons.
- [ ] Liquid extras: Frost, Specular, Tint, presets. Changing Frost updates live cards (preview; Save is optional for V1 look).
- [ ] Buttons: default / primary / hover / pressed (concave) / disabled / keyboard focus ring.
- [ ] Inputs, combo, list: caret, selection, no glass-over-glyph wells (wells use `LiquidWellBrush`).
- [ ] Tokens panel is diagnostics, not a second editor.

### 4.3 Liquid Glass Dark — Home

- [ ] Night canvas / moon wallpaper. Sidebar, nav, banner **transparent** over canvas.
- [ ] Light text on dark glass remains readable (KPI values, captions, table).
- [ ] Same **slots** as Light (ADR-08). Density may differ; layout map must not.

### 4.4 Liquid Glass Dark — Theme Studio

- [ ] Same controls as Light; extras still gated to Liquid.
- [ ] Contrast of captions and `ThemeError` / `StoreWarning` on glass.

### 4.5 Charts (both modes, Home)

Prior defects: **D02** nested cartesian flattened by axis chrome (`703797e`); **D03** KPI sparks empty (`52d1345` + `703797e`).

- [ ] KPI sparks visible in the 72–96 column (Revenue, Orders, Conversion, On-time). Not a hollow pad.
- [ ] Monthly performance: four nested plots (Revenue area, Retention lines, Capacity bars, Conversion spark) show **series**, not axis chrome filling the cell.
- [ ] Order mix: donut with legend **beside** the pie (`23fd30d`), not a corner pie.
- [ ] Channels / Revenue vs target: series visible, not hollow.
- [ ] Axis/legend/label **text** is the chart token color, not missing or same-as-plot.
- [ ] Charts stay opaque Skia (not frosted). No black plots (D12 GPU/BitmapCache — do not enable GPU).

Studio **Chart Lab** (`ChartGallery`) should keep full axes; compact chrome is for nested/short plots only.

### 4.6 Switching, layout, shared components

- [ ] Light ↔ Dark **without restart**; open ComboBox/ListBox selection is not dropped (Overlay).
- [ ] Liquid → Bento → Liquid: Bento is opaque cards; Liquid glass returns; no leftover Liquid extras on Bento.
- [ ] Resize ~720 width (min) and a wide maximized window. CompactWidth **1100** may drop Home to 2 columns (D10 accepted). Scroll, do not overlap.
- [ ] DataGrid “Recent work”: rows readable, selection/hover distinct.
- [ ] Dialog / tooltip / preview window follow the active Liquid package.

### 4.7 Transparency fallback, a11y, performance

- [ ] `LiquidReduceTransparency` is **False** in extras and `LiquidCustomization.Apply` always writes `false`. There is **no Studio toggle**. Do not invent a control. Note “fallback unexercised.”
- [ ] Keyboard: Tab through Theme picker, radios, Frost combo; focus ring visible (`KeyboardFocus` / `FocusIndicator`).
- [ ] Do not claim 4.5:1 on glass without measuring **pixels** of text vs the wallpaper showing through the card (D09). Token contrast is not enough.
- [ ] Performance: qualitative only unless timed. Watch scroll jank on Home (per-card unnamed DropShadow). No FPS invention.
- [ ] System High Contrast is **not** bound. Record as gap, not a fail of V1 unless text is unreadable in the normal theme.

## 5. Required screenshots

Keep files **out of product globs**. Do not commit PNG into `Theme/` or `step1.ThemeStudio/`. Root `.gitignore` already ignores `/clay-*.png` and `/fluent-*.png`. Prefer a folder outside the repo or a local `docs/theme-engine/evidence/` that is not added unless the owner asks.

Every file caption must include:

`screen | theme | appearance | condition | commit=<git rev-parse HEAD>`

| ID | Screen | Theme | Appearance | Condition | Looks for |
| --- | --- | --- | --- | --- | --- |
| S01 | Home full window | Liquid Glass | Light | Maximized, sidebar open | Canvas, opaque Light chrome, glass cards |
| S02 | Home chart crop | Liquid Glass | Light | KPI row + Monthly + Mix + Channels | D02/D03/V2 |
| S03 | Theme Studio | Liquid Glass | Light | Customization card + Liquid extras | Distinct from Fluent; extras visible |
| S04 | Theme Studio controls | Liquid Glass | Light | Hover + focus on a button and combo | States |
| S05 | Home full window | Liquid Glass | Dark | Maximized, sidebar open | Transparent Dark chrome, night canvas |
| S06 | Home chart crop | Liquid Glass | Dark | Same crop as S02 | Chart text on dark |
| S07 | Theme Studio | Liquid Glass | Dark | Extras + error/warning area | Readability |
| S08 | Home | Liquid Glass | Dark after Light | Switch radios, **no restart** | No stale Light resources |
| S09 | Theme Studio | Liquid Glass | Light after Dark | Switch back | No stale Dark resources |
| S10 | Home | Liquid Glass | Light | Width near 720 | Scroll, no overlap (D10 may 2-col) |
| S11 | Home DataGrid | Liquid Glass | Dark | Recent work rows | Table contrast |
| S12 | Home Mix cell | Liquid Glass | Light | Mix donut + legend | Legend beside pie, not corner |

Optional but useful: S13 Liquid vs Fluent Home Light (same slots, different material).

If a shot cannot be taken, write **NOT TAKEN** and why. Do not substitute a mockup.

## 6. Recording defects and updating D01

1. New visual bugs: add a row to [DEFECT_REGISTER.md](DEFECT_REGISTER.md) with the next ID, severity, summary, status `OPEN`, and the shot ID + commit.
2. D02/D03: if sparks and nested plots are clearly filled, move to a note “pixels seen on `<commit>` shot S02/S06” — still not owner V1. If still hollow/flat, keep FIXED-UNVERIFIED or reopen.
3. D05: if this Windows host built and ran the app, D05 can be narrowed to “CI parked; local Windows used for D01” — D01 itself stays separate.
4. **D01 / V1 stays BLOCKED** until:
   - S01–S07 (minimum) exist from the **actual** WPF window,
   - Light and Dark Home + Studio were inspected by a human,
   - chart text was checked,
   - prior rejects are absent or formally accepted,
   - the owner states accept (ACCEPTANCE_CRITERIA **V1**).
5. Only then may PHASE_STATUS set D01 (V1) to **VERIFIED**. Source PASS + STA PASS is **IMPLEMENTED — NOT VERIFIED** at best.
6. Fail or mixed shots → remain **BLOCKED** or **OPEN**. Do not start Stage E or F.

## 7. Remaining blockers (this Linux host)

| Blocker | Status |
| --- | --- |
| D01 / V1 visual accept | **BLOCKED** |
| D02 / D03 chart pixels | FIXED-UNVERIFIED until S02/S06 |
| D05 no Windows in the agent environment | OPEN |
| `dotnet` / display on this host | Absent |
| GitHub Actions `windows-wpf.yml` | Parked under `cols/ci/` |
| Stage E / F | Not authorized |

## 8. What this handoff did not do

No C#, XAML, theme resources, or behavior changes. No push. No fabricated screenshots. App was **not** launched here.
