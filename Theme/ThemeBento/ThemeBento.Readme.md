# Bento Grid / Modular — implementation design specification

Phase 01 authorized 2026-10-20. Only Bento is installed; both appearances share one typed semantic contract. This specification was written before implementation. Runtime comparison remains pending until Windows execution is available.

## Research and native adaptation

Reviewed actual source/documentation through GitHub (not screenshots or rendered web demos):

- [UI/UX Pro Max active Bento Box Grid data](https://github.com/nextlevelbuilder/ui-ux-pro-max-skill/blob/56ea9cf174ed86786187daa9de0e6247cb0747e4/.claude/skills/ui-ux-pro-max/data/styles.csv): modular grouping, varied spans, negative space, neutral surfaces, conditional accessibility. Legacy Bento Grids is deprecated. Avoid applying card layouts to dense data indiscriminately.
- [Magic UI Bento implementation](https://github.com/magicuidesign/magicui/blob/efe76fc5755eca1142dd4f74c1d6f0ace2887e3c/apps/www/registry/magicui/bento-grid.tsx), [actual composition example](https://github.com/magicuidesign/magicui/blob/efe76fc5755eca1142dd4f74c1d6f0ace2887e3c/apps/www/registry/example/bento-demo.tsx), [documentation](https://github.com/magicuidesign/magicui/blob/efe76fc5755eca1142dd4f74c1d6f0ace2887e3c/apps/www/content/docs/components/bento-grid.mdx): asymmetric 1/2-width modules, clear text hierarchy and a restrained surface boundary. Adapt with WPF Grid auto/star sizing, rounded Borders and resource-driven edges; do not copy React, CSS shadows, fixed auto-row heights, hover-reveal actions, marquees or animation libraries. All actions remain visible and keyboard accessible.
- [Ponytail skill](https://github.com/DietrichGebert/ponytail/blob/09c434ab73ca43761c5d8b0c5d341926c2fba710/skills/ponytail/SKILL.md): read affected flows, smallest complete native solution, test meaningful logic, never cut accessibility/validation. No MVVM/DI/theme dependencies needed.
- [Microsoft DynamicResource documentation](https://github.com/dotnet/docs-desktop/blob/main/dotnet-desktop-guide/wpf/advanced/dynamicresource-markup-extension.md), [styles/templates](https://github.com/dotnet/docs-desktop/blob/main/dotnet-desktop-guide/wpf/controls/styles-templates-overview.md), [WPF-specific reference data](https://github.com/nextlevelbuilder/ui-ux-pro-max-skill/blob/56ea9cf174ed86786187daa9de0e6247cb0747e4/.claude/skills/ui-ux-pro-max/data/stacks/wpf.csv). Replacing local dependency values removes dynamic expressions; therefore the engine replaces a single application dictionary and views never recolor themselves.
- [Actual .NET 8 ComboBox source](https://github.com/dotnet/wpf/blob/v8.0.0/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/ComboBox.cs): preserve PART_Popup and selection behavior. Native WPF API checks also use .NET 8 control source, not another XAML dialect.

Limitations: no browser visual research, no exhaustive source audit, and no available Windows rendering session. Initial incorrect reference paths returned 404; corrected paths above were successfully read. No external code or dependency was imported.

## Deliberate appearance values

| Role | Light | Dark |
| --- | --- | --- |
| Canvas | #F4F5F8 | #101218 |
| Primary surface | #FFFFFF | #1A1D27 |
| Secondary plate | #ECEEF5 | #232735 |
| Elevated surface | #FFFFFF | #2B3040 |
| Primary text | #202334 | #F1F3FA |
| Secondary/muted text | #535C72 / #555F74 | #B7BFD3 / #B0BAD0 |
| Accent / on-accent | #5440C8 / #FFFFFF | #B5A5FF / #191331 |
| Secondary accent | #086D67 | #70DCD0 |
| Subtle edge / input outline | #D8DCE7 / #79839A | #353D51 / #8592AC |
| Success / warning / error / information | #176443 / #805100 / #AF263A / #245FAD | #79DEAC / #FFD18A / #FF9BAC / #9FC8FF |

Interaction resources define separate hover, pressed, selected, disabled and focus treatments. Status plates use readable foregrounds and explicit text, never color alone. Disabled controls are deliberately muted, not active-state contrast claims. Opaque surfaces and edge treatment replace costly blur/shadows; no animation. Overlay and disabled opacity are contract tokens. Elevation is a surface/edge distinction, not a fake CSS inset effect.

## Shape, typography, spacing and variants

Segoe UI installed-system typography, Consolas for resource values, fallback through Windows fonts; no font downloads. Display/title/section/body/caption sizes 34/24/18/14/12 DIPs; semibold headings. Spacing 4/8/12/16/24/32 DIPs, control minimum height 38, minimum width 88, rounded controls 8, panels 16, small chips 6, edge 1 DIP and focus edge 2. All concrete values belong to appearance dictionaries, with the same keys/types in both. Font sizes are resources, not per-view literals.

- Primary card: primary surface + subtle edge + generous padding.
- Secondary plate: secondary surface, no decorative shadow; for supporting examples and inset grouping.
- Accent plate: accent-soft surface + accent text, for current theme identity.
- Elevated panel: elevated surface + clear outline; popup/dialog/tooltip tier.
- State examples: primary/secondary/disabled buttons; text input with real validation; checkbox/radio/selection; list row selection; status feedback and determinate progress; themed tooltip/context menu; modeless preview dialog stays open during switches.

## Layout and review plan

Studio uses full available width, opening maximized with normal Windows restore/maximize controls. It uses one independent card lane below 980 content DIPs, two below 1500, and three at/above 1500. Existing cards are assigned to the shortest measured lane when the breakpoint changes; native StackPanels handle wrapping and height changes without shared-row whitespace. No content-width cap. Window minimum 720x600, initial 1280x900 DIPs. Verify 720/1024/1440 widths and 100/150/200% Windows scaling before approval. All content stays available when stacked. The token reference enumerates the live typed contract and can filter by name.

Before calling Bento complete: compare the actual WPF screen against hierarchy, palettes, card/plate variants and state rules here; record discrepancies, fix shared resources/templates, rebuild and rerun switching/negative-resource/binding/layout tests. The current environment's limitations must never be reported as a successful visual review.

## Architecture and extension

`ThemeContract` is the executable key/type manifest. `ThemeService` loads a complete dictionary, validates before replacement, preserves unrelated application resources, and notifies active selection. Invalid selection/resources leave the previous appearance installed. Bento owns all its component styles/templates under step1.e.Components. Its local factory returns a complete package from the appearance values and a fresh dictionary of its own sealed style identities; only those identities are reused across modes to preserve existing control state. No visual dictionary is globally installed by the shared engine; no concrete brushes or mutable candidate dictionaries are cached. Components use DynamicResource for appearance values. The shared contract now also validates 29 required WPF style targets and rejects duplicate merged keys. All concrete tokens are visible in Studio. See the feature README for file responsibilities, Windows test commands and future-theme procedure. No future-theme folder is created.


## Isolation and validation expectations

Bento does not borrow visuals from another theme and is not a fallback for future themes. `step1.e.7.BentoResources.cs` owns composition and lifetime; the non-visual orchestrator calls its registered factory and swaps one complete validated package. Component files have only local invariant StaticResource helpers/inheritance; they do not rely on sibling merge precedence. All appearance-sensitive values/styles remain dynamic to consumers. Style-key removal in a candidate must not damage the installed package; styles are sealed and candidate key dictionaries are fresh.

Windows compiled checks verify missing styles even when Application already has that style, wrong WPF targets, duplicate package keys, unchanged active resources after rejected selection, and existing text/caret/templates/popups/dialogs during mode changes. Portable checks enforce local theme ownership and 76 contrast pairings. See the [current CI/evidence record](../../step1.ThemeStudio/step1.c.docs/step1.c.3.Theme-Isolation-and-CI.md); none of the Windows assertions or visual review has run here. Each future theme defines its own implementation/tests rather than overriding this one.

## Canonical reference additions — clean foundation reset

The page now has live selectors/status in its header; an emphasized feature card with primary/secondary actions; three reference metric plates; a preview-window information card; form/action/validation/progress samples; explicit success/warning/error/information/pending text; a searchable structured review list with header/metadata/status and empty state; surface layers; and live semantic tokens. These are local illustrative patterns, not an old dashboard or an ERP module. Metrics are clearly labeled sample/design-system values, not fabricated business KPIs. The structured list uses the existing ListBox/ListBoxItem contracts; it is not a DataGrid implementation.

Additional source inspection on 2026-10-20:
- [Carbon grid README](https://github.com/carbon-design-system/carbon/blob/42d0ecba41c5f4534a18253cb4a7744b5e17baf6/packages/grid/README.md): explicit grid/row/column context and breakpoint-specific spans. Adapt the principles to WPF Grid/auto/star sizing and a 980-DIP reflow; no CSS classes, framework or dependency imported.
- [Fluent UI button state source](https://github.com/microsoft/fluentui/blob/master/packages/react-components/react-button/library/src/components/Button/useButtonStyles.styles.ts): primary/secondary/subtle hierarchy and distinct hover/pressed/focus/disabled semantics. This is web-source research, not a WPF template to copy. Implement with native WPF Style/ControlTemplate triggers and the existing semantic resources. An attempted WinUI CommonStyles source path returned 404 and is not used as evidence.

For both Light and Dark: normal secondary buttons use secondary surface/strong edge/text; primary uses accent/on-accent, distinct accent hover/pressed; subtle uses transparent surface/edge with neutral hover/pressed and keyboard focus; destructive uses error-surface/error-text, stronger hover edge and pressed text-colored edge. Disabled foreground/surface override active colors. Destructive styling is reserved for clearing local preview text, never decorative emphasis. These two added named roles are validated along with existing styles (29 total). All 77 semantic keys match across modes; PageMaxWidth is the new layout token, not a second palette.

Typography uses native font-metric line height (no fixed clipping-prone body line boxes), wrapped body/caption text and auto-sized rows. Header title/display 34, metric/title 24 or display role, section 18, body 14, caption 12 DIPs. Width changes do not shrink fonts; layout reflows and scrolls. The 1440-DIP content cap, 720-DIP window minimum and existing spacing/radius/edge tokens apply identically to both modes; independent appearance palettes remain as specified above. Light uses quiet neutral surfaces with violet emphasis; Dark uses lifted charcoal surfaces, pale violet and readable status foregrounds, not inverted Light colors.

Worklist column proportions are 3:2:2 at every row; owner metadata is below the title, with wrapped text rather than truncation. Selection/hover/focus consume existing ListBoxItem state resources. Search and no-result feedback are real view-model behavior. Status text always accompanies color. Native tooltip/context menu/progress and a modeless preview exercise shared application lookup. Runtime appearance, accessibility, physical DPI and comparison remain unverified; source checks are not visual approval.

## Approved compact/full-width layout update

The user requested full-screen adaptation, less empty space and more compact cards. WindowState is initially Maximized (normal OS chrome, not kiosk mode). Page/card/plate padding is now 16/16/12 DIPs in both appearances; card gutters 10, item spacing 8, section spacing 12. Readable font sizes and 38-DIP minimum control height are unchanged. The header wraps title/selectors/status in one compact area, rather than occupying several full-width rows. All eight real card instances remain alive across lane changes, with explicit bindings to the root view-model context and focused input restored where possible.

The unused PageMaxWidth role is replaced by ReferenceListMaxHeight (320 DIPs). The live token list uses this bounded internal viewport, keeping all tokens searchable/scrollable without one card becoming many screens tall. There are still 77 typed semantic roles; no extra theme or framework. New native runtime assertions cover restored-window widths 720/1024/1440/1920/2560, lane count, all eight cards, binding/selection preservation and bounds/non-overlap. These assertions have not executed here. Physical DPI, keyboard/wheel behavior and actual rendered balance still require Windows review.
