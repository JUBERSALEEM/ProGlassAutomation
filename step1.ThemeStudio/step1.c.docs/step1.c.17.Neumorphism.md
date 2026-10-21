# Neumorphism (theme 4)

Separate package `Theme/ThemeNeumorphism`. Not Bento, Clay, or Fluent.

## How complete

First install was a hard 3px offset plate — roughly a third of the material (convex/concave flag only).

This upgrade is the volume engine:

- **Cascade:** three light plates (top-left) and three dark plates (bottom-right), ranked Chip / Control / Card.
- **Rim:** convex highlight + shade on the face; concave inverts into a well.
- **Sheen / well wash:** top-left light on raised faces, top-left shade in wells.
- **Pattern:** raised cards/KPI/hero/menus; inset fields, lists, tabs, progress, scroll tracks, check/radio wells; buttons lift on hover and invert on press.
- **Canvas melt:** face fill matches `SurfacePrimary` / `ApplicationBackground`. Accent is a rail or a filled primary button, not a candy card.

Still not a full studio (no depth knobs). Gaussian `DropShadowEffect` is intentionally unused so volume stays in layout and does not clip. Windows pixels remain unproven.

## Studio

Shared card rounding only. No neumorph knobs.
