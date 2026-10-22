# Stage B — architecture freeze

**Date:** 2026-10-22. Binding for Stage C. Does not authorize Theme 6 code.

## B01 — Identity model

Nine identities in order (ADR-03). `ThemeCatalog.Installed` remains **complete packages only**.

| Order | Name | Status | Material extras |
| --- | --- | --- | --- |
| 1 | Bento Grid / Modular | Installed | BentoCustomization (not a second engine) |
| 2 | Claymorphism | Installed | Clay owned |
| 3 | Fluent / Acrylic | Installed | **None** (geometry/font/color plates only) |
| 4 | Neumorphism | Installed | Neu owned |
| 5 | Liquid Glass | Installed | Liquid owned; visual **not accepted** |
| 6 | Glassmorphism | Planned | After Liquid accept |
| 7 | Spatial / Immersive | Planned | After 6 |
| 8 | Swiss International | Planned | After 7 |
| 9 | Holographic / Iridescent | Planned | After 8 |

Studio gallery (E01, later) reads `Sequence`. Theme picker today binds `InstalledThemes` (five). Incomplete themes must never appear as selectable installed packages.

## B02 — Token capability matrix

| Capability | Bento | Clay | Fluent | Neu | Liquid | Glass (later) |
| --- | --- | --- | --- | --- | --- | --- |
| Contract tokens + 47 styles | required | required | required | required | required | required |
| Shared card rounding | yes | ThemeOwned plump | yes | rounding shared; depth owned | yes | TBD |
| Accent/density/type studio | Bento | Clay | no extras engine | Neu | Liquid | — |
| Font overlay | global | global | global | global | global | global |
| Color plates | global | global | global | global | global | global |
| BlurEffect / capture | no | no | no | no | no | no |
| ReduceTransparency | — | — | — | — | extras | extras, not contract |

Do **not** add a shared contract blur token (C08 deferred).

## B03 — Resource update protocol (frozen)

Already implemented; do not replace:

1. `Load` + `ResourceValidation` **before** publish.
2. Replace **one** `MergedDictionaries` slot.
3. On failure, restore previous slot.
4. `Overlay` mutates live package then validates (fonts/colors; keeps ComboBox selection).
5. Never `MergedDictionaries.Clear()`.

## B04 — Session protocol

Façade around `CustomizationStore` (v2):

- One open session.
- Preview = live `Apply` / `Overlay` (already); **does not write disk**.
- Save = validate + atomic write + advance baseline.
- Failed save = previous file and baseline unchanged.
- Discard/Revert = restore baseline to live resources.
- Import = live only until Save.
- Dispose is available; the shell VM lives for the process, so Discard is the user-facing revert.

## B05 — Persistence

- Path: `%LOCALAPPDATA%\ProGlassAutomation\bento-customization.json` (name kept; D08 accepted).
- Version **2**. No v3 this run.
- 64 KB. JSON only. Reject markup/XAML.
- Corrupt/unsupported → defaults + `LoadWarning`.
- Preview must not overwrite the file.

## B06 — Glassmorphism criteria (planning only)

Light: frosted light fill, dark text, 4.5:1 on **effective** fill. Dark: darker frost, light text. Distinct from Liquid veil/wallpaper. Solid fallback. No Stage F code.

## B07 — Studio IA

- Settings sidebar stays. Theme Studio is not the window.
- Extras panels already gated (`CanCustomizeBento/Clay/…`, `IsFluent`).
- Completeness badges = E01 (after C02 metadata exists).
- Tokens panel = diagnostics, not a second editor.

## B08 — ADR review vs Liquid gate

No conflict: ADR-01/02/05 proceed. ADR-07/F remain blocked on D01. Fluent extras engine stays absent. Layout overlay stays dead.
