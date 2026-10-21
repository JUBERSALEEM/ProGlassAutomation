# Liquid Glass — fifth theme (Phase 01 / step1)

Native WPF reading of Apple Liquid Glass / iOS 26 widgets and the glass-ui recipe (`--glass-bg: rgba(255,255,255,0.25)`, `--glass-border: rgba(255,255,255,0.18)`, `--blur: 30px`, `--glass-shadow: 0 8px 32px`). Not Fluent acrylic, not Clay extras, not theme 6 Glassmorphism, not CSS `backdrop-filter`, not desktop capture, not a sine-wave lining field.

## Standard

- **Wallpaper** is a smooth sky→violet→rose gradient (`LiquidCanvasBrush`). No striped caustics.
- **Glass face** is a translucent white veil (`LiquidFaceBrush`). Clear = 22% (glass-ui default). The wallpaper must show through the card.
- **Edge** is a hairline white border (`--glass-border`), not a chromatic fringe.
- **Specular** is a short top sheen (`inset 0 1px white`). Hover brightens it.
- **Lift** is an unnamed `DropShadowEffect` on a near-transparent caster — never on the content tree, never `x:Name`.
- **Inputs** stay `LiquidWellBrush`. Charts stay CPU; no Blur, no BitmapCache.
- **Reduced transparency** swaps in `LiquidOpaqueBrush` and drops sheen/lift.

True Metal refraction, per-card desktop capture, Win2D and extra NuGet are **not** in this build.

## Engine

`LiquidGlassSurface` paints those layers. Frost / specular / tint are Liquid-owned extras. Rounding stays SharedTokens. Font and color overlays still run after material.

## Completeness

47 contract roles. Fifth catalog entry. Windows pixels **0%** — this environment cannot produce a Windows visual proof. L8 visual sign-off is not this sandbox.
