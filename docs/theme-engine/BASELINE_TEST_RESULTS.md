# Baseline test results

**Date:** 2026-10-22  
**HEAD:** `703797e`  
**Host:** Linux sandbox — **no `dotnet`**, **no WPF runtime**, **0% sandbox pixels**.

## Executed this audit

| Command | Result | Notes |
| --- | --- | --- |
| `python3 Theme/step1.d.tests/step1.d.5.verify_isolation.py` | **PASS** (run after `52d1345` / `703797e`) | Isolation of five packages, 47 styles, both modes |
| `python3 Theme/step1.d.tests/step1.d.6.verify_foundation.py` | **PASS** | 63 Compile / 78 Page globs; Compact+FillAvailable present |
| `python3 Theme/step1.d.tests/step1.d.3.verify_sources.py` | **FAIL** | `step1.b.27.ColorsStudioView.xaml` `Margin="0,0,2,0"` — **pre-existing**, not introduced by 2026-10-22 Liquid chart work |
| `python3 Theme/step1.d.tests/step1.d.4.CheckerRegression.py` | **FAIL** | Requires verify_sources baseline PASS |
| `dotnet build` / STA `ThemeTests` | **NOT RUN** | SDK and Windows absent |
| GitHub Actions `windows-wpf.yml` | **NOT RUN** | File parked at `cols/ci/`; Actions API 403 |

## Visual baseline (user pixels, not this host)

User screenshots after `6b22ffa` / `52d1345` (Clay Dark Home and Liquid Light/Dark Studio/Home) showed:

- KPI sparks missing or empty.
- Monthly performance Y-axis chrome flattening series.
- Mix donut in the corner (addressed `23fd30d`).
- Channels / Revenue vs target hollow.
- Theme-token density making the same slots look differently clipped.

Code responses: `52d1345` (compact mosaic + scrolling text cards), `703797e` (compact LiveCharts chrome). **Windows pixels after `703797e` have not been accepted.** Liquid Light+Dark remain **open**.

## Classification rule

| Label | Meaning |
| --- | --- |
| PASS (source) | Python isolation/foundation on this host |
| FAIL (source) | verify_sources literal Margin in Colors Studio |
| NOT RUN | Anything needing Windows or dotnet |
| OPEN | User visual acceptance |

Do not quote source PASS as visual or compilation evidence.
