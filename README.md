# ProGlass V3.0 — Bento Theme Studio

Fresh native **WPF / .NET 8** desktop implementation in the existing root project. **Phase 01 implementation is present; compilation and Windows runtime acceptance remain unverified in this environment.** Bento Grid / Modular Light and Dark are the only implemented appearances. No future themes or ERP/database workflows run.

## What is implemented

- Non-visual orchestration with 99 semantic token roles and 46 required WPF style targets, complete theme-package validation/replacement and active-selection notifications.
- Bento-isolated native styles/templates for introduced buttons, inputs, choices, selections, panels, text, status/progress, tooltips, context menus and scrollbars.
- Separate Theme Studio customization and Business Reference sample pages: real accent/density/shape/type controls, Light/Dark switching, live tokens, preview commands/validation and a shared-resource modeless window.
- No additional test-framework dependency: Windows STA runtime checks and portable source/contrast/regression checks.

- Real typed reusable charts: vertical/horizontal/grouped/stacked bars, multi-series line/area, pie/donut and sparkline, with native point tooltips, keyboard series toggles and textual/source-value alternatives.
- Business Reference opens first, with mixed KPI plates, asymmetric analytics, targets/activity and a themed sortable/selectable DataGrid. Studio remains separate, with chart/scenario controls and optional collapsed diagnostics instead of a large contract dump.
- [Chart reuse/dependency guide](Theme/step1.f.Analytics/step1.f.6.Analytics-README.md) explains future module bindings, validation and distribution-license obligations.

Only Bento is installed: the installed-theme selector truthfully has one choice. Light/Dark is the functional visual variant selector; no unavailable theme is represented as selectable. Runtime behavior is implemented but not claimed proven here.

## Run and verify on Windows

Prerequisites: supported Windows 10/11 development environment, .NET 8 SDK (or a compatible later SDK with .NET 8 targeting/runtime support), and WPF tooling. One reviewed/pinned product package: LiveChartsCore.SkiaSharpView.WPF 2.0.4 (MIT), with its native SkiaSharp/HarfBuzz dependencies. The retained `net8.0-windows7.0` TFM is an API baseline, **not** a Windows 7 runtime-support claim. The existing `.slnx` needs a toolchain that supports that solution format; project commands do not depend on it.

```powershell
dotnet build ProGlassAutomation.csproj --configuration Release
dotnet run --project ProGlassAutomation.csproj --configuration Release
dotnet run --project Theme/step1.d.tests/step1.d.ThemeTests.csproj --configuration Release
```

These are the intended Windows commands, not successful execution evidence. In this Linux container, `dotnet` is absent; project and solution build attempts exited 127. SDK download connection failed. No WPF UI was launched or screenshot captured. See the [current foundation evidence](step1.ThemeStudio/step1.c.docs/step1.c.9.Reusable-Analytics.md) and [manual review checklist](step1.ThemeStudio/step1.c.docs/step1.c.2.Phase01-Verification.md).

Portable checks actually executed here:

```sh
python3 Theme/step1.d.tests/step1.d.3.verify_sources.py
python3 Theme/step1.d.tests/step1.d.4.CheckerRegression.py
python3 Theme/step1.d.tests/step1.d.6.verify_foundation.py
```

## Repository boundaries

The latest clean-foundation instruction explicitly authorized removal of the prior SheetStore/dashboard implementation. All 47 inspected legacy-only files are removed; App/MainWindow start only the V3.0 reference page. No database, DI or legacy navigation is used; real analytics use the single reviewed WPF chart library. The root WPF project, AssemblyInfo, solution and approved documentation/theme work remain. See the [before-cleanup inventory](step1.ThemeStudio/step1.c.docs/step1.c.4.Cleanup-Inventory.md) and [current implementation/evidence report](step1.ThemeStudio/step1.c.docs/step1.c.9.Reusable-Analytics.md).

## Documentation map

Read [AGENTS.md](AGENTS.md) first, then:
- [Naming and stable Theme directories](docs/00.Project-Rules.md)
- [Architecture principles](docs/01.Architecture-Principles.md)
- [Semantic contract and design policy](docs/02.UI-UX-Design-System.md)
- [Development and testing](docs/03.Development-and-Testing.md)
- [Exact eight-theme roadmap and acceptance gates](docs/04.Project-Roadmap.md)
- [Decision log](docs/05.Decision-Log.md)
- [Feature registry](docs/06.Feature-Registry.md)
- [Theme Studio responsibilities and extension procedure](step1.ThemeStudio/step1.c.docs/step1.c.1.ThemeStudio-README.md)
- [Bento research and concrete design specification](Theme/ThemeBento/ThemeBento.Readme.md)

All theme implementation is under `Theme/`, with numbered shared implementation and stable individual-theme names. Studio files follow step1 hierarchy. Root App/MainWindow/project/manifest filenames and paired XAML/code-behind basenames are documented tool-compatibility exceptions. Phase 01 is awaiting Windows verification and user review; Claymorphism and step2 remain unauthorized.

## Windows CI verification

[cols/ci/windows-wpf.yml](cols/ci/windows-wpf.yml) checks out on windows-2022, installs/selects official .NET 8, restores/builds both actual projects, runs portable checks, compiled contracts (`--contracts-only`) and a bounded STA WPF runtime smoke test with software rendering. It preserves failure/timeouts and creates no release or executable upload. Local YAML checks passed; **the workflow itself has not run**. GitHub Actions API access returned HTTP 403. The workflow is now restored locally to .github/workflows per the latest instruction. These changes are not committed/pushed and cannot run remotely yet; the existing GitHub connection also lacks workflow-write permission. No SDK/runtime installation was attempted again in this container. Windows runtime assertions remain distinct from human visual approval/DPI/accessibility/performance review.

Bento owns its complete visual engine in Theme/ThemeBento. The shared engine contains no XAML or persistent visual dictionary. Future themes supply independent implementations through the same contract without screen rewrites; no future theme or placeholder is registered.


## Seeing the actual Theme Studio

Theme Studio is the root application's startup content, not hidden behind navigation. On Windows, open this branch's `ProGlassAutomation.csproj` as the startup project or run the `dotnet run` command above. No SheetStore service or database is needed. The browser preview here cannot execute WPF; there is no genuine rendered screenshot or remote Windows session available in this environment. Do not confuse the unchanged remote master branch with these local, uncommitted changes.

Latest user instruction: workflow stored **inactive** at `cols/ci/windows-wpf.yml` so the code can be pushed without workflow-write permission. This supersedes earlier restoration/activation statements. No Windows CI run occurs until it is moved back to `.github/workflows/` with suitable permission; restoration commands are in the file header.

## Current rich Bento workspace

The startup tab is **Theme Studio**: edit Ocean/Indigo/Teal accent, compact/comfortable density, crisp/soft/rounded corners and small/standard/large body typography. Reset design keeps sample inputs separate. **Business Reference** is a distinct tab with a compact KPI band, wide themed feature/trend, selected-work review list, status badges, preview form and handoff surfaces. It is illustrative local data, not an implemented ERP module. All design changes use the real active package and carry across mode switches and both pages. Settings are not saved to disk.

The current source supplies 82 semantic tokens and 35 required styles; portable checks cover 258 contrast pairings and 14 deliberate regressions. The compiled harness adds 108 appearance/customization combinations and native page-switching checks, but those Windows tests have not run here. The workflow is still intentionally inactive in cols/ci. See the current feature report for exact responsibilities and limitations.
