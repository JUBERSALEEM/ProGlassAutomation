# Claymorphism — second registered theme (Phase 01 / step1)

Date: **2026-10-20**. Native WPF. Bento files are not modified. Clay does not load Bento resources.

## Pattern (from the reference set)

Soft extruded tiles on a cool mist canvas: **32 DIP radius**, white inset edge, outer drop shadow, pastel plates (mint / lavender / sky / peach), coral pill actions, navy text. Dark is navy-violet, not brown.

Accent coral is darker than marketing salmon so **4.5:1** still holds. No 3D illustration pack, no shop/kanban/player chrome, no CSS.

## Light / Dark

| Role | Light | Dark |
| --- | --- | --- |
| Canvas | #F3F0FB | #1A1730 |
| Card | #FFFFFF | #2A2548 |
| Text | #1A1F4D | #F4F1FF |
| Accent / on-accent | #C81E1E / #FFFFFF | #FF8A80 / #1A1730 |
| Hero | #E4D8FF → #FFD6EA (navy type) | #2A2150 → #4A3A80 |
| Plates | mint / lavender / sky / peach | deep mint / violet / navy / clay |

Radii 32 / 24 / 16. Control height 40. Gutter 14. Shadow color lives in `*.Extras.xaml` (not the 102-token file).

## Kept separate

- Folder `Theme/ThemeClaymorphism/` only. No Bento XAML/C# edits.
- Factory does not cache or seal Bento styles.
- Bento customization does not run while Clay is selected.

Screens still compose with `BentoGrid` (shared mosaic panel, not a Clay visual). Contract keys `BentoRowHeight` / `BentoTile` stay because they are the shared contract, not Bento paint.

Windows pixels are not claimed.
