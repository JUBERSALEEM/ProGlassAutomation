# Liquid Glass — fifth theme (Phase 01 / step1)

Native WPF approximation of Apple Liquid Glass (WWDC 2025 / iOS 26, retuned 2026). Not Fluent acrylic, not Clay extras, not theme 6 Glassmorphism, not CSS `backdrop-filter`, not desktop GDI capture.

## Standard (v1)

- **Canvas** is a saturated opaque sky (`#62B4E8` light, `#0A3A58` dark). Glass is meaningless on a white page.
- **Glass face** is a lens gradient (`LiquidFaceBrush`) with no opaque sheet behind it. The canvas must show through the card.
- **Lens wash** (`LiquidLensBrush`) darkens the bottom edge (iOS 27) and keeps the top brighter.
- **Specular sheen** is a real top band, not a 1 px border. Hover brightens it.
- **Rim + lip** separate the lens from the canvas. Not Clay hue glow.
- **Lift** is an unnamed `DropShadowEffect` on a near-transparent caster — never on the content tree, never `x:Name`.
- **Inputs** use `LiquidWellBrush` (opaque wells). Charts stay CPU; no Blur, no BitmapCache.
- **Reduced transparency** swaps in `LiquidOpaqueBrush` and drops sheen/lift.

True Metal refraction, per-card desktop capture, device-tilt specular, Win2D and extra NuGet are **not** in v1.

## Engine

`LiquidGlassSurface` paints those layers. Frost / specular / tint are Liquid-owned extras. Rounding stays SharedTokens. Font and color overlays still run after material.

## Completeness

47 contract roles. Fifth catalog entry. Windows pixels **0%** — this environment cannot produce a Windows visual proof. L8 visual sign-off is not this sandbox.
