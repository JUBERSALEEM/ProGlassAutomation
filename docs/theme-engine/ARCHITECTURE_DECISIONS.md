# Architecture decisions

**Date:** 2026-10-22. Decisions are binding for later implementation runs unless a later ADR supersedes them in this file.

## ADR-01 — Extend the existing engine; do not replace it

**Decision:** Keep `ThemeService` + `ThemeCatalog` + `ThemeContract` + per-folder visual packages.  
**Rejected:** A parallel ExcelSticker `ThemeEngine` writing different keys into `Application.Resources`.  
**Why:** Mission rule 9 (no second disconnected customization system). Slot-replace + Overlay already solve runtime switch and ComboBox selection.

## ADR-02 — Catalog stays complete-only

**Decision:** `ThemeCatalog.Installed` lists only themes that have Light+Dark packages and loaders. Unfinished identities (6–9) get a **separate** metadata list (`ImplementationStatus`) for Studio, never a fake dictionary.  
**Rejected:** Empty `ThemeGlassmorphism/` folders or catalog placeholders (current comment forbids them).  
**Why:** Isolation tests and “do not advertise incomplete as complete.”

## ADR-03 — Nine-theme order (supersedes `docs/04` eight-theme list)

1. Bento Grid / Modular  
2. Claymorphism  
3. Fluent / Acrylic  
4. Neumorphism  
5. Liquid Glass  
6. Glassmorphism  
7. Spatial / Immersive  
8. Swiss International  
9. Holographic / Iridescent  

Light+Dark each. Fluent remains theme 3, not an extra. **Theme 6 implementation is gated on user acceptance of Liquid Light+Dark** (standing Phase 01 instruction). Planning Glassmorphism is allowed; coding it is not, until that gate.

## ADR-04 — Capability-aware tokens, not one mega-token bag

**Decision:** Shared contract tokens stay required for every theme. Material extras stay **theme-owned Extras.xaml** (Liquid frost, Clay plump, Neu depth). Studio shows extras only when `CanCustomizeX` is true. Fluent extras engine stays **absent**.  
**Rejected:** Forcing blur sliders onto Bento/Swiss.

## ADR-05 — Customization transactions as a façade

**Decision:** Add an ExcelSticker-like session **around** `CustomizationStore` (one editor, preview/save/revert/dispose). Do not change the JSON schema casually (version 2). If schema must grow, bump version and migrate.  
**Rejected:** `%LOCALAPPDATA%\OceanGlass\theme.json`.

## ADR-06 — DynamicResource policy

**Decision:** Semantic tokens and theme styles used from app/Studio XAML: `DynamicResource`. In-package template internals may `StaticResource` same-dictionary keys. Never local hex in Studio screens (verify_sources).  
**Rejected:** Clearing all MergedDictionaries on switch.

## ADR-07 — Glassmorphism material (Theme 6), when authorized

**Decision:** Frost = **higher-opacity fill + luminance hairline + optional CPU noise**, solid fallback, no `BlurEffect` on text/charts, no desktop capture, no CSS, no Liquid wallpaper clone, do not glass other themes.  
**Rejected:** `backdrop-filter` emulation via capture; GLSL lining (`718f8bc`); orbs (`5515aff`).

## ADR-08 — Layout is shared; material is not

**Decision:** Home/Studio mosaics remain one XAML tree. Themes must not ship different column/row maps. Density tokens (row height, gutter, padding) may differ; cards must **fit the smallest** (Liquid 120) via fill+scroll and compact chart chrome.  
**Rejected:** Per-theme page XAML.

## ADR-09 — Feature number

**Decision:** Remain **step1 / Phase 01**. Engine/Studio/Glassmorphism work in this mission is still that feature unless the owner allocates step2.  
**Rejected:** Inventing step2 for Theme 6.

## ADR-10 — Effects and charts

**Decision:** Never `x:Name` a DropShadowEffect. Never BlurEffect/BitmapCache on LiveCharts. CPU Skia. Compact axis chrome when plot height &lt; 160 or `ShowAxis=false`.  
**Rejected:** GPU Skia.

## ADR-11 — Persistence safety

**Decision:** Keep 64 KB cap, JSON data only, no imported XAML/scripts/shaders/fonts-as-code. Preview must not overwrite the saved file. Corrupt load → defaults + warning.  
**Already true** in `CustomizationStore`; session façade must not weaken it.

## ADR-12 — Windows evidence

**Decision:** Source scripts are not visual proof. Sandbox pixels 0%. Do not fake screenshots.  
**Rejected:** “It compiles in my head.”

## ADR-13 — ExcelSticker scope

**Decision:** Import **architecture** (session, atomic save, contrast guard, no Effect on text). Do not import Clay studio UI, print, or sticker canvas.
