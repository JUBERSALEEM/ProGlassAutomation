# Test strategy

## Layers

1. **Portable Python (this host)** — isolation, foundation, source literals, checker regression. Fast. Not visual.
2. **Compiled contracts** — `ThemeTests --contracts-only` (Windows). Token types, contrast, negative packages.
3. **STA runtime** — MainWindow, theme switch, chart identity, DataGrid selection. Windows only.
4. **Persistence fixtures** — corrupt JSON, oversize, unknown theme name, version mismatch.
5. **Session tests** (planned) — preview vs save vs dispose.
6. **Manual visual** — owner screenshots; agent does not fake them.
7. **CI** — `cols/ci/windows-wpf.yml` when published by owner with `workflows` permission.

## Rules

- Never mark VERIFIED without the matching layer.
- `verify_sources.py` currently fails on Colors Studio `Margin="0,0,2,0"` — fix is a **defect**, not a test waiver.
- Isolation must still see **five** packages until 6–9 are real.
- Analytics renderer tests must not reference ThemeStudio/ThemeService (ownership boundary).
- Do not add screenshot PNGs to product source (gitignore). Evidence lives outside the app glob.

## Performance

Measure on Windows when available: theme-switch latency, overlay latency, memory after 20 switches, scroll of Home mosaic. No FPS claims without a clock.
