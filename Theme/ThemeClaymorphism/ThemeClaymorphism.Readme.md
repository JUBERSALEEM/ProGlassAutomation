# Claymorphism — second registered theme (Phase 01 / step1)

Native WPF. Engines stay separate: not Bento plates, not Neumorph gray volume, not Fluent acrylic.

## Standard (2026)

Clay is a **floating inflated object**, not an extrusion:

- stadium / pill corners (radius scales with height)
- faces can melt into a tinted canvas; they are never sharp white sheets
- **outer drop** is one hue-matched shadow down-right (not dual Neumorph glows)
- **inset rim** is a rounded-rect ring (outer minus inner geometry) so highlight wraps the top-left arc and shade wraps the bottom-right — not four-sided `BorderThickness`
- 3-stop face curve (highlight → mid → shade)
- Control / Chip ranks **pop** with a compact drop (gutter stays 0)
- press **squish** (scale ~0.97) into a well; hover lifts the glow
- wells invert the curve and hide outer glow

## Engine

`ClaySurface` owns that recipe. `ClayCurveRim` draws the wrapping ring. Rank Chip / Control / Card. **Card** keeps a small glow gutter; **Control/Chip** have gutter 0 so nested plates are not a second frame, but they keep a compact drop so pills read as popping clay. Volume scales glow on cards and rim width. Plumpness owns radii. `ClayCustomization` stays Clay-owned.

CheckBox track stays a 40×24 `Border` pill (not `ClaySurface`) so On is not a plump bead.

C1–C5 are in: 3D corner rims, no nested gutters, pastel faces with Card drop, semantic KPI clay, pill controls. Neumorph N1–N2 are not applied.

## Completeness

47 contract roles. Windows pixels **0%**.
