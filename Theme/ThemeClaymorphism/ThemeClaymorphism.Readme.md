# Claymorphism — second registered theme (Phase 01 / step1)

Date: **2026-10-20**. Native WPF. Engines stay separate: Bento cards remain a flat `Border` in their templates.

## Engine

`ClaySurface` is the clay volume (clay.css recipe, no CSS):

- modest colored **drop** (bottom-right)
- **inner shade** on the face (bottom-right inset)
- **inner light** on the face (top-left inset)

Concave inverts the insets and hides the drop. `ShowDrop` stays false on tabs so selected pills do not jump. Hue extras (`ClayDrop*` / `ClayShade*`) tint cards, plates, and KPIs.

## Completeness

Source chrome for the 47 contract roles is filled. Catalog registers Clay. `ClayCustomization` is a Clay-owned engine (accent, canvas, volume, plumpness, density) — it does not call BentoCustomization. Windows pixels **0%** (workflow stays in `cols/ci`; no PC). Reports URL is sign-in gated; public chrome there is Crystal, not Clay.
