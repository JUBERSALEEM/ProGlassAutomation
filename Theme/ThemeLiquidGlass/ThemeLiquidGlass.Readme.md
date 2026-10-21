# Liquid Glass — fifth theme (Phase 01 / step1)

Native WPF reading of Apple Liquid Glass / iOS 26 widgets: a translucent veil over a **theme-owned wallpaper**, not a scene catalog. Wallpaper follows the GlassPulse / WWDC showcase pattern (mesh of overlapping light orbs). Glass follows glass-ui (`--glass-bg`, `--glass-border`, `--glass-shadow`). Not Fluent acrylic, not Clay, not CSS, not desktop capture, not a sine-wave lining, not a built-in scene generator.

## Showcase canvas

`LiquidCanvasBrush` is the Liquid Glass wallpaper — one DrawingBrush in extras, Light and Dark. Cyan / violet / rose orbs on a sky field so glass has something to sit on. No PNG, no extra type, no wallpaper picker.

## Standard

- **Glass face** is a translucent white veil. Clear = 22%. Wallpaper must show through.
- **Edge** is a visible white hairline, plus a short top specular.
- **Lift** is an unnamed `DropShadowEffect` on a near-transparent caster — never on the content tree, never `x:Name`.
- **Inputs** stay `LiquidWellBrush`. Charts stay CPU; no Blur, no BitmapCache.
- **Reduced transparency** swaps in `LiquidOpaqueBrush` and drops sheen/lift.

True Metal refraction, per-card desktop capture, Win2D and extra NuGet are **not** in this build.

## Engine

`LiquidGlassSurface` paints the glass. Frost / specular / tint are Liquid-owned extras. Rounding stays SharedTokens.

## Completeness

47 contract roles. Fifth catalog entry. Windows pixels **0%** — this environment cannot produce a Windows visual proof. L8 visual sign-off is not this sandbox.
