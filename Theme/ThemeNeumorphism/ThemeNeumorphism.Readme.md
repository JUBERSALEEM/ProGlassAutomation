# Neumorphism — fourth registered theme (Phase 01 / step1)

Native WPF. Engines stay separate: not Clay hue-matched plump, not Fluent acrylic, not Bento hairline cards.

## Standard (2026)

Soft UI extrusion from a **shared mid-tone canvas**:

- page fill **and** surface fill are the same hex (`#E6EBEF` light, `#2A2F38` dark)
- dual outer glow (WPF light 125° / dark 305° = CSS 145deg), blur ≈ 2× distance
- convex **face fill** is a 145° canvas pair; wells invert it
- press is **inset only** (`Concave`); hover is a stronger shadow, not a fill
- dark mode is a retune, not an invert (highlight is gray, not white)
- card radius stays on SharedTokens; Neu control/chip stay 12 after studio rounding
- depth / blur / intensity are Neumorph-owned and do not rewrite the gutter
- shadow alone is not a state: focus ring stays visible
- shadow colors mix from live `SurfacePrimary`
- rims 0 (web is shadow-only); button hairline 0
- press/hover are discrete (duration 0)

## Engine

`NeumorphSurface` owns plates, the 145° face, clipped 6/12 inset pair, and layout-safe Gaussian glow. Triggers never name a `DropShadowEffect` (MC4111). Control/Chip glow uses a small gutter (6/4). Card near plates stay off. StatusBadge stays a `Border` (one Effect).

N1–N2, M1–M14, R1–R14, S1–S10 are in.

## Completeness

47 contract roles. Windows pixels **0%**.
