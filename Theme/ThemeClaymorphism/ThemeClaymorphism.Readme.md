# Claymorphism — second registered theme (Phase 01 / step1)

Native WPF. Engines stay separate: not Bento plates, not Neumorph gray volume, not Fluent acrylic.

## Standard (2026)

Clay is a **floating inflated object**, not an extrusion:

- stadium / pill corners (radius scales with height)
- faces can melt into a tinted canvas; they are never sharp white sheets
- **three-shadow recipe**: outer hue-matched glow + inset light (top-left) + inset shade (bottom-right)
- 3-stop face curve (highlight → mid → shade)
- press **squish** (scale ~0.97) into a well; hover lifts the glow
- wells invert the curve and hide outer glow

## Engine

`ClaySurface` owns that recipe. Rank Chip / Control / Card. Volume scales glow blur and distance. Plumpness owns radii. `ClayCustomization` stays Clay-owned.

## Completeness

47 contract roles. Windows pixels **0%**.
