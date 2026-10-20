# Claymorphism — second registered theme (Phase 01 / step1)

Date: **2026-10-20**. Native WPF. Not a new feature number. Bento remains the default and the mosaic engine.

## What this package is

Claymorphism is a **complete second theme** in `ThemeCatalog`. It fills the same 102-token / 47-style contract so Theme Studio and Business Reference keep their BentoGrid mosaics and swap skins through `ThemeService`.

It is not a Bento accent, not CSS, and not DropShadow/blur. Lift is a second native `Border` in control templates (opaque contact edge + face). Cards use a light top edge (`SurfaceElevated` on `SurfacePrimary`) and large radii.

## Light / Dark (source dictionaries)

| Role | Light | Dark |
| --- | --- | --- |
| Canvas | #E7DCCF | #1A1612 |
| Card | #FFF7EE | #2A241E |
| Inset / raised | #F3E8DC / #FFFFFF | #221C18 / #352E26 |
| Text | #1C150F / #4E4339 / #5C5147 | #F6F0E8 / #D2C4B4 / #C4B5A4 |
| Accent / on-accent | #9A3518 / #FFFFFF | #E08A5C / #1A1612 |
| Hero | #2A140C → #5A2414 | #140E0A → #3A1C10 |

Radii 22 / 16 / 12. Control height 36. Gutter 12. Row 180. Border 2 DIP.

## What it does not take from Bento

Bento customization (accent/density/shape) stays Bento-owned. Studio `CanCustomize` is false while Clay is active. Pick **Claymorphism** in the theme list; Light/Dark still apply.

Windows restore/build/pixels are not claimed.
