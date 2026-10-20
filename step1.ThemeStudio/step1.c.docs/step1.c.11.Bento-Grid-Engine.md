# step1.c.11 — Bento Grid Engine (Phase 01 correction)

Date: **2026-10-20**. Phase 01 correction inside step1, not a new feature and not Claymorphism.

## What the source now does

- **Engine:** `Theme/ThemeBento/step1.e.11.BentoGrid.cs` — native WPF `Panel`, equal-height rows, gutter, explicit spans, packed reflow below `BentoCompactWidth`, `FillAvailable` for viewport mosaics.
- **One studio:** `ThemeStudioView` is the only wired studio. Settings 1×4, compact live preview 2×3 (`AnalyticsOverview Compact="True"`: featured KPI 2×1, chart 2×1, mix|target 1×1), gallery 3×1. `CustomizeWorkspace` is leftover and not in this tree.
- **Business Reference:** viewport `FillAvailable` mosaic (no page `ScrollViewer`). Hero 2×2, pulse 2×2, analytics 4×5 with **unequal KPIs** (2+1+1), work/review 2×2, handoff 4×1. Inner analytics row count matches the outer rowspan so row heights align.
- **Shell:** themed window, Light/Dark, Studio / Business tabs. No fake ERP sidebar.
- **Workflow:** owner may keep `windows-wpf.yml` in `.github/workflows/`. Agents must not move it back to `cols/ci`. The App cannot publish workflow files; the owner’s placement is authoritative.

## Still not claimed

Windows restore/build/STA/visual inspection are not done in this environment. No accessibility certification. No PR without two separate confirmations.
