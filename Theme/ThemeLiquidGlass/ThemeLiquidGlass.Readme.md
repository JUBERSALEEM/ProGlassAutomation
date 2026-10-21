# Liquid Glass — fifth theme (Phase 01 / step1)

Native WPF reading of Apple Liquid Glass (iOS 26 / macOS Tahoe Control Center). Wallpaper is a **coded Tahoe sky/water scene** (`LiquidWallpaper`), not a PNG and not a scene catalog. Glass is a translucent veil + inset specular + hairline + lift. Not Fluent acrylic, not Clay, not CSS, not desktop capture, not a sine-wave lining.

## Showcase canvas

`LiquidWallpaper.Create` paints sky → water, a sun/moon bloom and a warm sea path. `LiquidResources.Load` installs it as `LiquidCanvasBrush`. Light and Dark only.

## Standard

- **Glass face** is an iOS-like veil (`frost × 0.40`). Clear ≈ 9% white. Token stays 0.22/0.40/0.62.
- **Edge** is a white hairline plus a 1px inset specular lip.
- **Lift** is an unnamed `DropShadowEffect` on a near-transparent caster — never on the content tree, never `x:Name`.
- **Inputs** stay `LiquidWellBrush`. Charts stay CPU; no Blur, no BitmapCache.
- **Reduced transparency** swaps in `LiquidOpaqueBrush` and drops sheen/lift.

True Metal refraction of the wallpaper through the pane is **not** in this build.

## Completeness

47 contract roles. Fifth catalog entry. Windows pixels **0%**. L8 visual sign-off is not this sandbox.
