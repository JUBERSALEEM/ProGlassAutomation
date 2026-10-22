# Master plan — theme engine, Theme Studio, Glassmorphism

**Feature:** step1 / Phase 01 (no new feature number).  
**First run:** research and planning only (2026-10-22). **No engine or Glassmorphism implementation in that run.**

Authoritative companions: [ARCHITECTURE_DECISIONS](ARCHITECTURE_DECISIONS.md), [PHASE_STATUS](PHASE_STATUS.md), [ACCEPTANCE_CRITERIA](ACCEPTANCE_CRITERIA.md).

## Critical path

```
A Discovery (this run)
 → B Architecture freeze
 → C Engine gaps (session façade, identity metadata, Colors Margin defect)
 → Liquid visual gate (D01) ★
 → E Studio completeness
 → F Glassmorphism (Theme 6)  ← blocked until ★
 → G Integration
 → H QA on Windows
 → I Owner acceptance
```

Themes 7–9 are **after** F, each with its own visual gate. Do not skip the sequence.

## Stage A — Discovery (A01–A08)

| ID | Title | Objective | Depends | Inspect | Change | Tests | Accept | Evidence | Risk |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| A01 | Repo inventory | Trace engine, packages, Studio, tests | — | Theme/, step1.ThemeStudio/, App, MainWindow | docs only | — | Audit exists | REPOSITORY_AUDIT.md | Stale docs/04 |
| A02 | Baseline tests | Record what actually ran | A01 | step1.d.tests | docs | run python | Honest PASS/FAIL/NOT RUN | BASELINE_TEST_RESULTS.md | Fake PASS |
| A03 | Theme coverage | Five installed vs four unbuilt | A01 | Catalog, extras | docs | isolation | Table complete | EXISTING_THEME_COVERAGE.md | Calling Liquid “done” |
| A04 | ExcelSticker audit | Session/save/contrast patterns | — | GitHub ExcelSticker Theming | docs | — | Reuse vs reject listed | RESEARCH_FINDINGS §4 | Copying Clay UI |
| A05 | WPF/a11y/glass research | Official + design sources | — | Learn, WCAG, glass articles | docs | — | Register filled | SOURCE_REGISTER.md | CSS-as-WPF |
| A06 | Architecture ADRs | Binding decisions | A01–A05 | — | ADR file | — | ADR-01–13 | ARCHITECTURE_DECISIONS.md | Second engine |
| A07 | Roadmap + criteria | Granular phases | A06 | — | this file | review | No circular deps | MASTER_PLAN + ACCEPTANCE + TEST_STRATEGY | Padding to 100 |
| A08 | Checkpoint system | Survive the next run | A07 | — | STATUS, CHECKPOINT, CHANGELOG, DEFECTS | — | Next action clear | RUN_CHECKPOINT.md | Implementing F early |

## Stage B — Architecture freeze (B01–B08)

| ID | Title | Objective | Depends | Notes |
| --- | --- | --- | --- | --- |
| B01 | Identity model | Spec nine identities + ImplementationStatus without catalog placeholders | A06 | Code later in C |
| B02 | Token capability matrix | Which extras belong to which theme | A06 | Fluent = none |
| B03 | Resource update protocol | Slot replace + Overlay; no Clear() | A06 | Already implemented; document as frozen |
| B04 | Session protocol | Preview/Save/Revert/Dispose | A04 | Façade on CustomizationStore |
| B05 | Persistence migration rules | v2 stay; v3 only if rename/fields required | A06 | 64 KB |
| B06 | Glass criteria Light/Dark | Frost vs Liquid; fallback | A05, ADR-07 | No code |
| B07 | Studio IA | Gallery completeness badges; extras panels gated | A03 | Settings sidebar stays |
| B08 | ADR review | Owner-visible summary; stop if conflict with Liquid gate | B01–B07 | |

## Stage C — Core engine gaps (C01–C12)

Implement **only after** A/B. Do not rewrite ThemeService.

| ID | Title | Files (expected) | Accept |
| --- | --- | --- | --- |
| C01 | Fix ColorsStudio Margin literal (D04) | `step1.b.27.ColorsStudioView.xaml` | verify_sources PASS |
| C02 | Theme identity metadata (not Installed) | new small type next to Catalog | Isolation still 5 packages |
| C03 | Session façade | new cs under step1.g or Studio | One editor; dispose reverts |
| C04 | Wire VM to session | `step1.b.17.Customization.cs` | No second JSON |
| C05 | Import/export bounds tests | ThemeTests | Oversize/XAML rejected |
| C06 | Overlay validation already present — regression only | ThemeService | Selection retained |
| C07 | Notification exception path documented + test | existing | |
| C08 | ReduceTransparency contract for future glass | only if a **shared** token is justified; else keep theme-owned | Do not glass Bento |
| C09 | Diagnostics: load warning surface in Studio | TokensPanel / customization header | |
| C10 | Do not register themes 6–9 | Catalog | Isolation 5 |
| C11 | Stale docs/04 and /06 note or patch | docs | No behavior change |
| C12 | Engine gap review | — | C01–C11 done or deferred with ID |

## Stage D — Shared components (D01–D08)

Mostly **already exist**. This stage is audit-and-fix, not a new control kit.

| ID | Title | Accept |
| --- | --- | --- |
| D01 | Confirm 47 styles on all five packages | isolation |
| D02 | Nested chart compact chrome (D02/D03) Windows confirm | user shot |
| D03 | DataGrid readability Light/Dark all five | shot |
| D04 | Focus rings 3:1 | token + spot check |
| D05 | Disabled/hover/pressed/selected | Studio controls panel |
| D06 | Dialog/tooltip/popup | PreviewWindow |
| D07 | Density: cards fit Liquid 120 row | no clip |
| D08 | No Effect on text presenters | grep |

## Stage E — Theme Studio (E01–E12)

| ID | Title | Accept |
| --- | --- | --- |
| E01 | Theme list shows 1–5 complete; 6–9 “not installed” | no fake preview |
| E02 | Light/Dark bound to ThemeService | |
| E03 | Color plates + HEX/spectrum (exists) — verify persist | |
| E04 | Texts Studio fonts (exists) — no crash | |
| E05 | Density/geometry (exists) | |
| E06 | Theme-owned extras only when CanCustomize* | Fluent: none |
| E07 | Live Home/Studio preview same pipeline | |
| E08 | Save/Revert/Reset/unsaved close via session | C03 |
| E09 | Import/export | C05 |
| E10 | Accessibility: contrast guard messaging | |
| E11 | Chart Lab still full axes | not compact-by-mistake |
| E12 | Studio usable at 720 width (scroll, not overlap) | |

## Stage F — Glassmorphism (F01–F08) — **BLOCKED on D01 / V1**

| ID | Title | Notes |
| --- | --- | --- |
| F01 | Package folder ThemeGlassmorphism Light/Dark + extras | Only after Liquid accepted |
| F02 | Loader + Catalog entry | Then isolation expects **six** packages |
| F03 | Foundation/buttons/inputs/… complete styles | Copy contract, not Liquid extras |
| F04 | Frost surface (ADR-07) | No BlurEffect on charts |
| F05 | Light material | |
| F06 | Dark material | |
| F07 | Reduced-transparency fallback | |
| F08 | Tables + navigation on glass | Contrast |

## Stage G — Integration (G01–G06)

| ID | Title |
| --- | --- |
| G01 | Shell: still no Liquid extras from MainWindow; Theme 6 uses contract only |
| G02 | Home/Studio mosaics unchanged slots |
| G03 | PreviewWindow follows theme |
| G04 | Texts/Colors overlays survive theme 6 switch |
| G05 | Hard-coded color grep |
| G06 | Open dialogs update on Apply |

## Stage H — QA (H01–H08)

Windows required. H01 build, H02 contracts, H03 STA, H04 persistence, H05 switch leak, H06 visual matrix, H07 DPI, H08 performance notes.

## Stage I — Acceptance (I01–I06)

I01 engine sign-off, I02 Studio sign-off, I03 Glass Light/Dark sign-off, I04 coverage report, I05 defect review, I06 owner approval. No publish without authorization.

## Phase count

A8 + B8 + C12 + D8 + E12 + F8 + G6 + H8 + I6 = **76** independently verifiable phases. Not padded to 100.

## Explicit non-goals until owner says otherwise

- Spatial, Swiss, Holographic implementation (after F).
- Fluent extras/lighting engine.
- Layout Studio restore.
- PR / production publish.
- CSS, capture blur, scene-catalog wallpapers.
