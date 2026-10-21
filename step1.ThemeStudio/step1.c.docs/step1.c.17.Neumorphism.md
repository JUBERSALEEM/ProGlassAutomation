# Neumorphism (theme 4)

Separate package `Theme/ThemeNeumorphism`. Not Bento, Clay, or Fluent.

## Engine

- **Gaussian dual shadows** (`DropShadowEffect`) at 145° / 325°. Card glow sits in `NeumorphGutter`. Control/Chip glow has gutter 0.
- **Face fill** is an opaque 145° canvas pair. Wells invert it and show a clipped dual inset pair. WPF cannot `inset` a `DropShadowEffect`.
- **Near plates** stay collapsed. Rounding stays **shared geometry**. Depth / blur / intensity are Neumorph-owned and do not rewrite the gutter.
- **Pattern:** raised cards/KPI/hero/metrics on the canvas; inset fields/lists/tabs/progress/scroll/checks; buttons rest extruded and **press is the inset well only**. Hover is a shadow change, not a fill.

Windows pixels remain **0%**. This environment cannot produce a Windows visual proof.
