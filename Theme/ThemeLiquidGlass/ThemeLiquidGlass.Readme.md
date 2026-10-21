# Liquid Glass — fifth theme (Phase 01 / step1)

Native WPF approximation of Apple Liquid Glass (WWDC 2025 / iOS 26, retuned 2026). Not Fluent acrylic, not Clay extras, not theme 6 Glassmorphism, not CSS `backdrop-filter`, not desktop GDI capture.

## Standard (v1)

- **Canvas** is saturated and opaque (`ApplicationBackground`) so text and charts do not fall through the window.
- **Glass face** is a translucent fill (`LiquidFaceBrush`) over that canvas — cool white in Light, dark navy glass in Dark.
- **Specular inset** is the tell: a bright top edge, brighter on hover. Concave wells invert it.
- **Darkened rim** (iOS 27 separation) is a 1 px inner shade, not a Clay hue glow.
- **Lift** is an unnamed outer `DropShadowEffect` on Card rank only. Never `x:Name` a `DropShadowEffect`.
- **Capsule chrome** uses `ControlCornerRadius` / `ChipCornerRadius`. Cards bind `CardCornerRadius` (SharedTokens).
- **Inputs** stay opaque enough for 4.5:1. Charts stay CPU; do not Blur LiveCharts or enable BitmapCache.
- **Reduced transparency** collapses face, specular and lift to opaque `SurfacePrimary`.

True Metal refraction, per-card desktop capture, device-tilt specular, Win2D and extra NuGet are **not** in v1.

## Engine

`LiquidGlassSurface` paints those layers. Frost / specular / tint are Liquid-owned extras. Rounding stays SharedTokens. Font and color overlays still run after material.

## Completeness

47 contract roles when components are present. Catalog registration is a later phase. Windows pixels **0%** — this environment cannot produce a Windows visual proof.
