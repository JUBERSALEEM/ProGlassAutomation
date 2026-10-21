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

`ClaySurface` owns that recipe. Rank Chip / Control / Card. **Card** keeps a small glow gutter; **Control/Chip** have gutter 0 so nested plates are not a second frame. Volume scales glow on cards only. Plumpness owns radii. `ClayCustomization` stays Clay-owned.

C1–C5 are in: 3D corner rims, no nested gutters, pastel faces with Card drop, semantic KPI clay, pill controls. Neumorph N1–N2 are not applied.

## Completeness

47 contract roles. Windows pixels **0%**.
