# step1.c.11 — Bento Grid Engine (Phase 01 correction)

Date: **2026-10-20**. This is a Phase 01 correction inside step1, not a new feature number and not Claymorphism. It exists because the previous Theme Studio/Business Reference composition was a stacked dashboard and a 3-pane editor, not a bento mosaic.

## Intent

Make Bento a real layout engine, then let Theme Studio and Business Reference consume it.

- **Engine:** `Theme/ThemeBento/step1.e.11.BentoGrid.cs` — native WPF `Panel` with equal-height rows, gutter, explicit column/row spans, and packed reflow below `BentoCompactWidth`.
- **Studio:** `ThemeStudioView` is a 4-column fill mosaic: live preview (2×2), settings rail (1×3), signals, review controls, chart gallery, diagnostics.
- **Business Reference:** one page-level mosaic (hero 2×2, pulse 2×2, analytics 4×6, work/review 2×2, handoff 4×1) plus an inner analytics mosaic (unequal KPI/chart/table cells). No `UniformGrid` of equal KPI tiles.
- **Shell:** fake ERP sidebar removed. The window consumes `ThemedWindow` / `ApplicationBackground` so Light/Dark restyles chrome.

Shared engine stays non-visual. No future theme, no ERP module, no new package.

## Native adaptation (not CSS)

Magic UI Bento uses CSS `grid-cols-3`, `auto-rows`, `col-span`, `gap`. Those are not WPF APIs. This panel uses `MeasureOverride` / `ArrangeOverride`, attached `Column` / `Row` / `ColumnSpan` / `RowSpan`, and a `Gutter` property (the attribute name `gap` is forbidden in this repository). Compact mode packs in document order and does not mutate the wide-layout slots.

## New contract roles

Tokens (both appearances): `BentoRowHeight` 168, `BentoGutter` 8, `BentoCompactWidth` 1100. Density/spacing customization updates row height and gutter. Style target: `BentoTile` → `Border` (fill the cell; margin is the grid’s gutter). Contract is now **102 roles / 47 style targets**.

## Verification

Portable source/isolation/foundation/regression checks are the evidence available here. Windows restore/build/STA runtime/visual inspection remain **unverified** (no .NET SDK in this environment; workflow stays inactive under `cols/ci`). Runtime checks were updated to assert `BentoGrid.EffectiveColumns` instead of collapsing a CSS-like second star column.

## Not claimed

Rendered polish, overlap-free proof at every DPI, accessibility certification, or that nested studio preview cells are as tall as the business page. The business page is the canonical mosaic; Studio is the live workbench that edits it.
