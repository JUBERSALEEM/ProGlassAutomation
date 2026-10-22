# Run checkpoint

**Time:** 2026-10-22  
**Branch:** `arena/f1e0ac70-proglassautomation`  
**Run type:** Diagnose Windows persistence contract + Python environment  
**Do not push/publish** unless the owner authorizes.

## Last completed

Empty-import reject in `CustomizationStore.ReadFile` (`HasVersion2`). D13 FIXED-UNVERIFIED. **D01 / V1 remains BLOCKED.**

## Current phase

D01 still **BLOCKED**. Re-run ThemeTests on Windows. Do not start E or F.

## Work performed this run

- Traced `CustomizationPersistenceChecks` line 50: `ReadFile` of `{}` expected `ArgumentException`.
- Root cause: `SavedCustomization` constructor defaults (`Version = 2`, parameterless ctor supplies `Preferences`) so `{}` deserialized as a valid v2 package.
- Fix: require an explicit JSON `Version` number 2 before deserialize. Test expectation unchanged. v2 `WriteFile` still emits Version.
- Python on **this** Linux host: `/usr/bin/python3` 3.11.2. Windows `C:\Python314\python.exe` (0x80070003) is **not reachable** from here.

## Files changed

- `Theme/step1.g.Customization/step1.g.2.CustomizationStore.cs`
- docs: checkpoint, changelog, defect D13

## Commands

| Command | Host | Result |
| --- | --- | --- |
| `python3 Theme/step1.d.tests/step1.d.3.verify_sources.py` | Linux | **PASS** exit 0 |
| `python3 Theme/step1.d.tests/step1.d.5.verify_isolation.py` | Linux | **PASS** exit 0 |
| `python3 Theme/step1.d.tests/step1.d.6.verify_foundation.py` | Linux | **PASS** exit 0 |
| `python3 Theme/step1.d.tests/step1.d.4.CheckerRegression.py` | Linux | **PASS** exit 0 |
| `C:\Python314\python.exe` | Windows (reported) | **NOT RUN** — 0x80070003 path not found |
| `dotnet` ThemeTests `--contracts-only` and full STA | this host | **NOT RUN** (no SDK). Prior Windows run: exception then **exit 1** |
| WPF launch / D01 shots | | **NOT RUN** |

## Test-runner note

`RuntimeChecks.Main` calls `ContractChecks.Run()` (includes persistence) **before** any final PASS. On failure it `WriteLine`s the exception and **returns 1**. Earlier `PASS:` lines from CustomizationChecks/AnalyticsChecks do not mean the process succeeded.

## Explicitly not done

D01 visual accept, Stage E/F, package upgrades (NU1701), push, Windows Python repair (path not on this host).
