# Liquid Glass — fifth theme (Phase 01 / step1)

Native WPF approximation of Apple Liquid Glass (WWDC 2025 / iOS 26, retuned 2026). Optics follow AmirHossein Aghajari’s rounded-rect SDF lens as adapted in dragosniamtu/WPF-Liquid-Glass-Effect (MIT `LiquidGlassSample.glsl`): inward displacement, chromatic aberration, edge highlight. Not Fluent acrylic, not Clay extras, not theme 6 Glassmorphism, not CSS `backdrop-filter`, not desktop GDI capture, not XamlTemplates.FX.Wpf.

## Standard

- **Canvas** is a saturated sky (`#62B4E8` light, `#0A3A58` dark) plus `LiquidCanvasBrush` caustics behind the workspace so glass has something to bend.
- **Face** is a thin tint. The card body is a CPU SDF refraction bitmap (`LiquidGlassSurface.RefractionBrush`) sampling a watery field — centre stays see-through, the lip warps, splits R/G/B and picks up a meniscus.
- **Chromatic fringe** (`LiquidChromaticBrush`) sits on the rounded rect. Specular sheen is a top band. Rim + lip separate the lens from the canvas.
- **Lift** is an unnamed `DropShadowEffect` on a near-transparent caster — never on the content tree, never `x:Name`.
- **Inputs** stay `LiquidWellBrush`. Charts stay CPU; no Blur, no BitmapCache.
- **Reduced transparency** swaps in `LiquidOpaqueBrush` and drops refraction/sheen/lift.

True Metal refraction, per-card desktop capture, device-tilt specular, Win2D and extra NuGet are **not** in this build.

## Engine

`LiquidGlassSurface` paints those layers. Frost / specular / tint / refraction strength are Liquid-owned extras. Rounding stays SharedTokens. Font and color overlays still run after material.

## Completeness

47 contract roles. Fifth catalog entry. Windows pixels **0%** — this environment cannot produce a Windows visual proof. L8 visual sign-off is not this sandbox.
