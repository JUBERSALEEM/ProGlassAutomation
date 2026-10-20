# ProGlass V3.0 — Bento Theme Studio

Fresh native **WPF / .NET 8** desktop implementation in the existing root project. **Phase 01 implementation is present; compilation and Windows runtime acceptance remain unverified in this environment.** Bento Grid / Modular Light and Dark are the only implemented appearances. No future themes or ERP/database workflows run.

## What is implemented

- Non-visual orchestration with 76 semantic token roles and 27 required WPF style targets, complete theme-package validation/replacement and active-selection notifications.
- Bento-isolated native styles/templates for introduced buttons, inputs, choices, selections, panels, text, status/progress, tooltips, context menus and scrollbars.
- Studio with modular adaptive layout, real appearance switching, preview commands/validation, live filtered token reference and a modeless shared-resource preview window.
- Dependency-free Windows STA runtime checks and portable source/contrast/regression checks.

Only Bento is installed: the installed-theme selector truthfully has one choice. Light/Dark is the functional visual variant selector; no unavailable theme is represented as selectable. Runtime behavior is implemented but not claimed proven here.

## Run and verify on Windows

Prerequisites: supported Windows 10/11 development environment, .NET 8 SDK (or a compatible later SDK with .NET 8 targeting/runtime support), and WPF tooling. No product NuGet dependencies. The retained `net8.0-windows7.0` TFM is an API baseline, **not** a Windows 7 runtime-support claim. The existing `.slnx` needs a toolchain that supports that solution format; project commands do not depend on it.

```powershell
dotnet build ProGlassAutomation.csproj --configuration Release
dotnet run --project ProGlassAutomation.csproj --configuration Release
dotnet run --project Theme/step1.d.tests/step1.d.ThemeTests.csproj --configuration Release
```

These are the intended Windows commands, not successful execution evidence. In this Linux container, `dotnet` is absent; project and solution build attempts exited 127. SDK download connection failed. No WPF UI was launched or screenshot captured. See the [current isolation/CI evidence](step1.ThemeStudio/step1.c.docs/step1.c.3.Theme-Isolation-and-CI.md) and [manual review checklist](step1.ThemeStudio/step1.c.docs/step1.c.2.Phase01-Verification.md).

Portable checks actually executed here:

```sh
python3 Theme/step1.d.tests/step1.d.3.verify_sources.py
python3 Theme/step1.d.tests/step1.d.4.CheckerRegression.py
```

## Repository boundaries

Phase 01 authorization explicitly permitted using the existing root WPF starter. App/MainWindow and project configuration now integrate the fresh Studio; startup no longer initializes SheetStore or a database. Unrelated `SheetStore/`, `Shared/`, `Converters/` and root `ViewModels/` files remain untouched and excluded through explicit Compile/Page items. No parallel app directory or migration was introduced. AssemblyInfo remains unchanged. See the [decision log](docs/05.Decision-Log.md) for the historical inspection and current authorization.

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

[docs/ci/windows-wpf.yml](docs/ci/windows-wpf.yml) checks out on windows-2022, installs/selects official .NET 8, restores/builds both actual projects, runs portable checks, compiled contracts (`--contracts-only`) and a bounded STA WPF runtime smoke test with software rendering. It preserves failure/timeouts and creates no release or executable upload. Local YAML checks passed; **the workflow itself has not run**. GitHub Actions API access returned HTTP 403. The initial push was rejected for missing workflow-write permission; the workflow is now stored inactive under docs/ci at the user’s request. Its header contains commands to restore it to .github/workflows later. It will not run until restored. No SDK/runtime installation was attempted again in this container. Windows runtime assertions remain distinct from human visual approval/DPI/accessibility/performance review.

Bento owns its complete visual engine in Theme/ThemeBento. The shared engine contains no XAML or persistent visual dictionary. Future themes supply independent implementations through the same contract without screen rewrites; no future theme or placeholder is registered.

Workflow activation status: `docs/ci/windows-wpf.yml` is an **inactive template**, moved at the user’s request after GitHub rejected the push for missing workflow-write permission. Restore it to `.github/workflows/windows-wpf.yml` using the commands in its header before expecting CI runs; restoration requires workflow-write permission.
