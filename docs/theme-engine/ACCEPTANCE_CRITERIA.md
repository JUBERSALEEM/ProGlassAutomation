# Acceptance criteria

Nothing below is “passed” until its evidence column exists.

## Engine

| ID | Criterion | Evidence required |
| --- | --- | --- |
| E1 | Exactly the authorized complete themes load; incomplete ones are not in `Installed` | Catalog source + isolation PASS |
| E2 | Light/Dark apply without restart; previous package restored on failure | STA ThemeTests on Windows |
| E3 | Invalid tokens rejected before swap | ContractChecks |
| E4 | Overlay does not drop ComboBox/ListBox selection | Existing analytics/runtime checks |
| E5 | Persistence 64 KB, versioned, corrupt → defaults | Store tests + file fixture |
| E6 | One edit session; dispose reverts unsaved | New session tests (not written yet) |
| E7 | Theme change cannot alter business data | N/A in this repo (no ERP); still a regression checklist if modules return |

## Theme Studio

| ID | Criterion | Evidence |
| --- | --- | --- |
| S1 | Every visible control changes a real token or is disabled with a reason | Manual matrix on Windows |
| S2 | Live preview uses `ThemeService`, not a fake palette | Code review + runtime |
| S3 | Save persists; Revert restores last save; close with unsaved restores | Session tests + screenshots |
| S4 | Import/export JSON only, size-limited | Tests |
| S5 | Incomplete themes (6–9) listed as in-development or hidden until packaged | Studio UI after metadata work |
| S6 | Extreme valid values do not overlap/clip beyond scroll | Manual resize 720–2560 |

## Visual / Liquid gate

| ID | Criterion | Evidence |
| --- | --- | --- |
| V1 | Liquid Light+Dark Home and Theme Studio accepted by the user | User statement + Windows shots |
| V2 | KPI sparks visible; Mix donut beside legend; nested plots not flattened | Shots after `703797e` |
| V3 | Same **slots** across the five installed themes | Cross-theme shots |
| V4 | No lining, orbs, opaque L7, wash, frost plates (prior rejects) | Negative visual |

**V1 is the gate for Theme 6 code.**

## Glassmorphism (after V1)

| ID | Criterion | Evidence |
| --- | --- | --- |
| G1 | Recognizable frost Light and Dark, not a Liquid clone | Shots vs Liquid |
| G2 | Body text contrast 4.5:1 on **effective** fill (solid inner or opaque enough veil) | Measured on Windows |
| G3 | Reduced-transparency fallback looks intentional | Toggle shots |
| G4 | No BlurEffect on charts/text; no capture; no CSS | Source grep + isolation |
| G5 | Tables readable | DataGrid shot |

## Quality bar that is never “done” from Linux

Build, STA runtime, 100/150/200% DPI, keyboard, high contrast, performance numbers — **NOT RUN** until Windows CI or a local Windows host exists.
