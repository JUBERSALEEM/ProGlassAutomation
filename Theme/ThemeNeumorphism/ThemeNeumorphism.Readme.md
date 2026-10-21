# Neumorphism — fourth registered theme (Phase 01 / step1)

Native WPF. Engines stay separate: not Clay hue-matched plump, not Fluent acrylic, not Bento hairline cards.

## Standard (2026)

Soft UI extrusion from a **shared mid-tone canvas**:

- page fill **and** surface fill are the same hex (`#E6EBEF` light, `#2A2F38` dark)
- dual outer glow (light 135° / dark 315°), blur ≈ 2× distance
- convex face gradient toward the light; wells invert it
- press swaps to **inset** (`Concave`), hover **Lifted**
- dark mode is a retune, not an invert (highlight is gray, not white)
- radius stays on SharedTokens; depth / blur / intensity are Neumorph-owned
- shadow alone is not a state: focus ring stays visible

## Engine

`NeumorphSurface` owns plates, rims, sheen, and layout-safe Gaussian glow. Triggers never name a `DropShadowEffect` (MC4111). **Card** keeps dual glow; **Control/Chip** have gutter 0 so nested plates are not a second frame. Rims wrap the corner radius.

N1 (3D rim, no nested gutters) is in. N2 convex/well sheen is not applied yet.

## Completeness

47 contract roles. Windows pixels **0%**.
