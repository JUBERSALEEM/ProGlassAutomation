# Neumorphism (theme 4)

Separate package `Theme/ThemeNeumorphism`. Not Bento, Clay, or Fluent.

## Engine

- **Gaussian dual shadows** at WPF 125° / 305° (CSS 145deg). Card glow sits in `NeumorphGutter`. Control/Chip use a 6/4 gutter so clip does not eat the pair.
- **Face fill** is the opaque 145° canvas pair (no leftover sheen layer). Wells invert it; Face clips a 6/12 dual DropShadow pair (WPF: one Effect per plate).
- **Rims are 0.** Web recipe is shadow-only.
- **Near plates** stay collapsed. Card radius stays **shared geometry**; Neu control/chip stay 12.
- Depth / blur / intensity are Neumorph-owned and do not rewrite the gutter. Spread token exists (0).
- **Pattern:** raised cards/KPI/hero/metrics on the canvas; inset fields/lists/tabs/progress/scroll/checks; buttons rest extruded and **press is the inset well only**. Hover is a shadow change, not a fill. Unselected rows stay flat.
- Press/hover are discrete setters (duration 0).

Windows pixels remain **0%**. This environment cannot produce a Windows visual proof.
