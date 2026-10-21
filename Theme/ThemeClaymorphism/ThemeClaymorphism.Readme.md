# Claymorphism — second registered theme (Phase 01 / step1)

Native WPF. Engines stay separate: not Bento plates, not Neumorph gray volume, not Fluent acrylic.

## Engine

`ClaySurface` is canvas-melted clay:

- **stadium / pill** corners (`Control`/`Chip` radius = half control height; cards stay plump)
- faces match the canvas (`SurfacePrimary` = `ApplicationBackground`) so cards are not white sheets
- **3D curve** on the face: top-left highlight → bottom-right shade (`ClayCurveBrush`)
- hue-matched Gaussian glow inside a layout gutter (dark 315°, light 135°) — not a second filled card, not a purple offset blob
- concave wells invert the curve and hide outer glow
- **Rank** Chip / Control / Card; **Lifted** deepens glow; press inverts to a well

Concave fields, lists, tabs, progress, scroll tracks, and check/radio wells use extras thicknesses (`ClayWellShadeThickness`) so studio lighting/relief still apply. Volume scales glow blur and distance.

## Completeness

47 contract roles. `ClayCustomization` stays Clay-owned. Windows pixels **0%**.
