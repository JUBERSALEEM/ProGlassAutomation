# Research findings

**Run:** first (research and planning only). **Date:** 2026-10-22.  
**Limitation:** This sandbox cannot launch WPF. Microsoft Learn sometimes returned an authorization interstitial. Research continued via GitHub (ExcelSticker), secondary technical articles, and the live ProGlass tree.

## 1. WPF theming (what ProGlass already does)

WPF has no `ThemeResource` (that is WinUI/UWP). Runtime theme switching is:

1. Put theme keys in a `ResourceDictionary`.
2. Reference them with `{DynamicResource}`.
3. Replace or mutate the dictionary on the dispatcher.

ProGlass `ThemeService` already:

- Loads a **complete package** per theme × appearance.
- Validates tokens + required styles **before** swap (`ResourceValidation`).
- Replaces **one** `MergedDictionaries` slot (does not `Clear()` the application dictionary — preserves `UnrelatedResource` in tests).
- Restores the previous slot if parse/apply throws.
- Raises `INotifyPropertyChanged` after a successful swap; subscriber failures become `ThemeNotificationException` **after** the theme is active.

Implications:

- Do **not** introduce a second engine that also mutates `Application.Resources`.
- Overlay (`ThemeService.Overlay`) exists specifically so font/color plates do not replace the dictionary while a ComboBox is open.
- `StaticResource` inside a **theme's own** control template is acceptable for in-package keys (S01: theme self-contained lookup). Cross-package and app-level theme keys must stay `DynamicResource`.

Performance: DynamicResource is a listener. ProGlass already scopes it to semantic tokens, not every literal.

## 2. Acrylic / blur in classic WPF

Findings (S05, S13):

- WinUI Acrylic/Mica are **not** available as first-class WPF brushes.
- `BlurEffect` / `DropShadowEffect` on a live visual promote that subtree to software rendering and can disable ClearType (ExcelSticker S18: never put Effect on text presenters).
- DWM accent-state acrylic (undocumented) requires `AllowsTransparency` + custom chrome — conflicts with a normal app window (standing product rule: chrome looks like a **normal app**).
- LiveCharts Skia surfaces are **opaque**. GPU Skia + BitmapCache already produced **black charts** (`05a14bd`). Charts will never frost.

**Glassmorphism in this product cannot be CSS `backdrop-filter` and cannot be desktop capture.** Honest material:

| Approach | Verdict |
| --- | --- |
| CSS / WebView glass | Forbidden (native WPF only) |
| GDI/DXGI wallpaper capture as card background | Forbidden (standing) |
| `BlurEffect` on cards containing text or charts | Reject — software + ClearType + chart black |
| DWM window acrylic | Optional later for **window** only, not per-card; not Theme 6 v1 |
| Veil + hairline + specular (Liquid already) | Allowed; Theme 6 must be **frostier / more opaque fill** than Liquid, not a clone |
| Solid fallback when reduced-transparency | Required |

Theme 6 (Glassmorphism) vs Theme 5 (Liquid Glass):

- Liquid: thin iOS/Tahoe veil, wallpaper scene, no lining/orbs (many visual rejects already).
- Glassmorphism: frosted **layer stack**, stronger fill (~40–55% for text cards per S11/S12), 1px luminance border, blur **simulated** by fill+noise or omitted with an honest fallback.

Do not glass other themes.

## 3. Accessibility

- Token-pair 4.5:1 is already tested in `ContractChecks` for all **installed** packages.
- Translucent surfaces **invalidate** those numbers unless the **effective** fill (veil over worst-case wallpaper) still passes.
- Focus: `FocusIndicator` + `KeyboardFocus` style exist. Must remain 3:1 non-text (S08).
- High contrast: not auto-bound to `SystemParameters.HighContrast`. Record as gap, not a fake ninth-theme “HC”.
- Reduced effects: Liquid has `LiquidReduceTransparency`. Theme 6 needs the same capability token, theme-owned.

## 4. ExcelSticker customization (reuse vs reject)

Inspected branch `arena/4ed12585-excelsticker` (S16–S18).

**Reuse as patterns (not code copy):**

- Single open `ThemeEditSession`.
- Preview applies draft; Save validates, atomic write, then advances baseline; Dispose reverts if unsaved.
- 64 KB JSON, no executable import, temp-file replace.
- Contrast guard can block save.
- Effect never on text content.
- Theme engine must not change business data (ExcelSticker: print mm; ProGlass: no ERP calculations in this repo anyway).

**Do not copy:**

- Clay-only `ThemeTokens` bag into ProGlass (ProGlass is **multi-theme**, capability-aware).
- `ThemeStudioWindow` as a second window product — ProGlass Studio is a **sidebar Settings page** in the shell.
- OceanGlass `%LOCALAPPDATA%` path — ProGlass already uses `%LOCALAPPDATA%\ProGlassAutomation\bento-customization.json`.
- Clay bevel renderer / print pipeline.

ProGlass already has `CustomizationStore` (version 2, 64 KB, per-theme state for Bento/Clay/Neu/Liquid, SharedGeometry, Font, Color plates). Gap vs ExcelSticker: **no explicit EditSession type**; save/revert lives in `ThemeStudioViewModel` / `Customization.cs`. Plan: add a session façade **on top of** the store, not a second store.

## 5. Dashboard / Studio visual research

Used during Liquid work; still binding:

- KPI = number + spark, not a full axis chart (S15). Nested plots in short cells need compact chrome (`703797e`).
- Bento grid: match tile size to content (charts need 2 rows; chips 1 row). Do not 1×1 catalog that removes graphs (rejected `d0e1f09`).
- Do not LEFT-big Mix/Target (`77ff238`).

## 6. Unresolved research questions

1. Can Theme 6 ship a **recognizable frost** without GPU backdrop blur? Hypothesis: yes, with higher-opacity fill + hairline + optional CPU noise, plus solid fallback. **Must be proven on Windows pixels**, not this document.
2. Should DWM acrylic ever apply to the **window** for Fluent only? Standing rule: Fluent has **no extras/lighting engine**. Defer.
3. Auto high-contrast: out of first implementation; document as I-phase optional.

## 7. What this research forbids

- Starting Glassmorphism code this run.
- Registering four empty theme folders as “complete”.
- A second customization JSON/engine.
- CSS, WebView, or shader glass.
- Claiming WCAG certification from token math alone.
- Claiming Windows build/visual pass from Linux source scripts.
