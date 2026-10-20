# Fluent / Acrylic — third registered theme (Phase 01 / step1)

Date: **2026-10-20**. Native WPF. Engines stay separate: Bento stays a flat `Border`, Clay stays `ClaySurface`. No Win2D, no extra NuGet, no CSS `backdrop-filter`.

## Engine

`FluentSurface` is the acrylic volume (native layers, not desktop blur):

- **tint** wash over the face
- **luminosity** highlight on the top edge (bottom when concave)
- **thin stroke**

Hue extras (`FluentTint*` / `FluentStroke*`) tint cards, plates, and KPIs. `ShowTint` stays false on unselected tabs.

## Completeness

Source chrome for the 47 contract roles is filled. Mica/acrylic Light and Dark palettes are Fluent-owned (not Bento copies). Catalog registers Fluent; last theme restores on launch. Bento customization stays off. Windows pixels **0%** (workflow stays in `cols/ci`; no PC).
