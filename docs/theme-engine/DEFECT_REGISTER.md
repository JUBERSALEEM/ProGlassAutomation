# Defect register

Status: OPEN / MITIGATED-IN-SOURCE / ACCEPTED-RISK / FIXED-UNVERIFIED / CLOSED.

| ID | Opened | Severity | Summary | Status | Notes |
| --- | --- | --- | --- | --- | --- |
| D01 | 2026-10-22 | High | Liquid Light+Dark not visually accepted | OPEN | Gate for Theme 6. 2026-10-22 D01 run: source inspected (`D01_LIQUID_VISUAL.md`); **visual BLOCKED** (no WPF host). History: L7 opaque, frost plates, lining, wash, orbs, Light chrome, Mix/KPI/Channels holes. |
| D02 | 2026-10-22 | High | Nested cartesian charts flattened by axis chrome | FIXED-UNVERIFIED | `703797e`. Needs Windows Home shot. |
| D03 | 2026-10-22 | High | KPI sparks empty (padding + non-sparkline Orders bars) | FIXED-UNVERIFIED | Same commit + KPI template 72–96 column (`52d1345`). |
| D04 | 2026-10-22 | Medium | `verify_sources.py` FAIL Colors Studio Margin/`{Binding}` | FIXED-UNVERIFIED | C01: `MarginInline` + `{Binding .}`; ColorPickerBar spectrum in code-behind. Python PASS this host. Not Windows-verified. |
| D05 | 2026-10-22 | High | No Windows build/STA in this environment | OPEN | CI parked. |
| D06 | 2026-10-22 | Medium | `docs/04` and `docs/06` stale vs nine-theme order | MITIGATED-IN-SOURCE | C11 supersession banners. Historical lists kept. |
| D07 | 2026-10-22 | Medium | No `ThemeEditSession`; save/revert implicit in VM | FIXED-UNVERIFIED | C03/C04 façade. Windows persist tests NOT RUN. |
| D08 | 2026-10-22 | Low | Persistence filename `bento-customization.json` | ACCEPTED-RISK | Payload is multi-theme; rename needs migration. |
| D09 | 2026-10-22 | Medium | Glass contrast over wallpaper not measurable from tokens | OPEN | Theme 6 design must use inner solid or high veil. |
| D10 | 2026-10-22 | Low | CompactWidth 1100 reflows Studio &lt;1100 DIP | ACCEPTED-RISK | Documented engine behavior (`PlacePacked`). |
| D11 | historic | — | Layout Studio / layout overlay | CLOSED | Hard reset to `d1c4268`. Do not restore. |
| D12 | historic | — | Black charts GPU/BitmapCache | CLOSED | CPU Skia. Do not revert. |
