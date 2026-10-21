# Neumorphism (theme 4)

Separate package `Theme/ThemeNeumorphism`. Not Bento, Clay, or Fluent.

## Engine

- **Gaussian dual shadows** (`DropShadowEffect`) inside a layout gutter (`NeumorphGutter`) so blur stays in the card box and does not clip.
- **Near plates + inner rim + sheen** still give a hard edge. Wells stay inset (no outer glow).
- **Studio knobs (Neumorph-owned):** Depth, Blur, Intensity. Rounding stays **shared geometry**.
- **Pattern:** raised cards/KPI/hero; inset fields/lists/tabs/progress/scroll/checks; buttons lift then press.

Windows pixels remain **0%**. This environment cannot produce a Windows visual proof.
