# Neumorphism — fourth registered theme (Phase 01 / step1)

Native WPF. Engines stay separate: not Clay hue-matched plump, not Fluent acrylic, not Bento hairline cards.

## Standard (2026)

Soft UI extrusion from a **shared mid-tone canvas**:

- page fill **and** surface fill are the same hex (`#E6EBEF` light, `#2A2F38` dark)
- dual outer glow (light 145° / dark 325°), blur ≈ 2× distance
- convex **face** is a 145° canvas pair; wells invert it
- press is **inset only** (`Concave`); hover is a stronger shadow, not a fill
- dark mode is a retune, not an invert (highlight is gray, not white)
- radius stays on SharedTokens; depth / blur / intensity are Neumorph-owned and do not rewrite the gutter
- shadow alone is not a state: focus ring stays visible
- shadow colors are mixed from the canvas (± toward white/black)

## Engine

`NeumorphSurface` owns plates, rims, sheen, clipped inset wells, and layout-safe Gaussian glow. Triggers never name a `DropShadowEffect` (MC4111). Control/Chip keep dual glow with gutter 0. Card near plates stay off.

N1–N2 and M1–M13 are in. Studio copy matches press-is-inset.

## Completeness

47 contract roles. Windows pixels **0%**.
