# step1.a — Executable semantic theme contract

`step1.a.1.ThemeContract.cs` is the authoritative mandatory key/type manifest; this document defines its use and types, not visual values. Both Bento appearances satisfy the same 77 semantic roles and 29 style/target contracts. All visual implementations and concrete values belong to individual themes. [Permanent policy](../../docs/02.UI-UX-Design-System.md) owns governance; [Bento specification](../ThemeBento/ThemeBento.Readme.md) owns visual intent/research.

## Types, constraints and consumers

- **Brush:** semantic surfaces, text, edges, status and interaction roles. Bento uses opaque SolidColorBrush except OverlayBrush/TransparentBrush; future native gradient brushes are allowed by the contract. Brush semantics remain the same across themes. Window, card, input, selection, status, popup and scrollbar templates consume these roles.
- **double:** nonnegative finite DIP sizes/font sizes/stroke widths; opacity keys additionally <=1. Values are dynamic dependency-property references, never CSS lengths. Use dimensions as minimums where appropriate, not rigid content bounds.
- **Thickness:** padding, margins and edge widths. MarginFocus intentionally extends the keyboard adorner 3 DIPs outside the control; it is not a negative size. Each theme’s layouts/templates consume correctly typed Thickness resources, not scalar resources where a Thickness is required.
- **CornerRadius:** panel/control/chip/choice geometry; only use on controls whose actual WPF API supports it, typically template Borders.
- **FontFamily/FontWeight:** body/mono families and heading/body roles. Caption/section/title/display hierarchies combine the appropriate size and weight resources in that theme’s own styles.

Key names describe responsibilities: Status*Surface is the plate behind its matching Status* foreground; StateSelected and AccentSoft are paired with AccentPrimary; TextOnAccent pairs with AccentPrimary/Hover/Pressed. Disabled colors are not used to convey active-state requirements. BorderStrong and FocusIndicator supply perceivable boundaries; BorderSubtle is decorative. Typography contrast is measured against actual expected surfaces, not inferred from color names.

## Mandatory semantic roles and types

These are names/types consumed by current application screens, not a required palette or Bento aesthetic. Theme-private resources and visual treatment may differ. Add mandatory public roles only for concrete consumer needs, then implement them in every installed theme. Refer to the theme's own dictionaries/design specification for values.

| Semantic role | WPF type |
| --- | --- |
| `ApplicationBackground` | `Brush` |
| `SurfacePrimary` | `Brush` |
| `SurfaceSecondary` | `Brush` |
| `SurfaceElevated` | `Brush` |
| `TextPrimary` | `Brush` |
| `TextSecondary` | `Brush` |
| `TextMuted` | `Brush` |
| `AccentPrimary` | `Brush` |
| `AccentSecondary` | `Brush` |
| `TextOnAccent` | `Brush` |
| `AccentSoft` | `Brush` |
| `BorderSubtle` | `Brush` |
| `BorderStrong` | `Brush` |
| `FocusIndicator` | `Brush` |
| `StateHover` | `Brush` |
| `StatePressed` | `Brush` |
| `StateSelected` | `Brush` |
| `AccentHover` | `Brush` |
| `AccentPressed` | `Brush` |
| `DisabledSurface` | `Brush` |
| `DisabledText` | `Brush` |
| `StatusSuccess` | `Brush` |
| `StatusSuccessSurface` | `Brush` |
| `StatusWarning` | `Brush` |
| `StatusWarningSurface` | `Brush` |
| `StatusError` | `Brush` |
| `StatusErrorSurface` | `Brush` |
| `StatusInformation` | `Brush` |
| `StatusInformationSurface` | `Brush` |
| `OverlayBrush` | `Brush` |
| `TransparentBrush` | `Brush` |
| `FontSizeDisplay` | `double` |
| `FontSizeTitle` | `double` |
| `FontSizeSection` | `double` |
| `FontSizeBody` | `double` |
| `FontSizeCaption` | `double` |
| `SpacingSmall` | `double` |
| `SpacingMedium` | `double` |
| `SpacingLarge` | `double` |
| `SpacingSection` | `double` |
| `SpacingPage` | `double` |
| `ControlMinHeight` | `double` |
| `ControlMinWidth` | `double` |
| `ChoiceMarkSize` | `double` |
| `ScrollBarSize` | `double` |
| `DisabledOpacity` | `double` |
| `PopupMaxHeight` | `double` |
| `PageMaxWidth` | `double` |
| `WindowInitialWidth` | `double` |
| `WindowInitialHeight` | `double` |
| `WindowMinWidth` | `double` |
| `WindowMinHeight` | `double` |
| `PaddingControl` | `Thickness` |
| `PaddingCard` | `Thickness` |
| `PaddingPlate` | `Thickness` |
| `PaddingPage` | `Thickness` |
| `MarginItem` | `Thickness` |
| `MarginSection` | `Thickness` |
| `MarginInline` | `Thickness` |
| `MarginPanel` | `Thickness` |
| `BorderThicknessControl` | `Thickness` |
| `FocusThickness` | `Thickness` |
| `PaddingChoice` | `Thickness` |
| `PaddingCompact` | `Thickness` |
| `CardCornerRadius` | `CornerRadius` |
| `ControlCornerRadius` | `CornerRadius` |
| `ChipCornerRadius` | `CornerRadius` |
| `FontFamilyBody` | `FontFamily` |
| `FontFamilyMono` | `FontFamily` |
| `FontWeightHeading` | `FontWeight` |
| `FontWeightBody` | `FontWeight` |
| `PaddingMark` | `Thickness` |
| `PaddingSelection` | `Thickness` |
| `ChoiceCornerRadius` | `CornerRadius` |
| `StrokeWidth` | `double` |
| `SelectionOpacity` | `double` |
| `MarginFocus` | `Thickness` |

## Required consumer style roles and WPF target types

Each complete theme must supply these roles itself, including implicit WPF styles. A Style with the wrong TargetType is rejected before installation. These roles preserve consumer compatibility; their setters/templates/geometry/states are owned independently by each theme. Additional theme-private helpers are allowed, but duplicate keys anywhere in one package are rejected. There is no Bento fallback.

| Style key | Target type |
| --- | --- |
| `ThemedWindow` | `Window` |
| `Caption` | `TextBlock` |
| `SectionHeading` | `TextBlock` |
| `PageHeading` | `TextBlock` |
| `Card` | `Border` |
| `Plate` | `Border` |
| `AccentPlate` | `Border` |
| `ElevatedPlate` | `Border` |
| `TokenValue` | `TextBlock` |
| `KeyboardFocus` | `Control` |
| `SubtleButton` | `Button` |
| `DestructiveButton` | `Button` |
| `PrimaryButton` | `Button` |
| implicit `TextBlock` | `TextBlock` |
| implicit `Button` | `Button` |
| implicit `TextBox` | `TextBox` |
| implicit `ComboBox` | `ComboBox` |
| implicit `ComboBoxItem` | `ComboBoxItem` |
| implicit `CheckBox` | `CheckBox` |
| implicit `RadioButton` | `RadioButton` |
| implicit `ListBox` | `ListBox` |
| implicit `ListBoxItem` | `ListBoxItem` |
| implicit `ToolTip` | `ToolTip` |
| implicit `ContextMenu` | `ContextMenu` |
| implicit `MenuItem` | `MenuItem` |
| implicit `Thumb` | `Thumb` |
| implicit `ScrollBar` | `ScrollBar` |
| implicit `Separator` | `Separator` |
| implicit `ProgressBar` | `ProgressBar` |

## Resource lifetime and replacement

The orchestrator calls the registered theme’s LoadResources(appearance) factory, validates its independent result (tokens, style targets and duplicate merged keys), and installs/replaces one entire package slot. No separate global visual dictionary survives switching themes. Application state and unrelated resources are not deliberately cleared. Loading/validation failure retains the prior slot/state with contextual diagnostics; a synchronous activation exception attempts restoration and is rethrown. Do not treat this as a guarantee against arbitrary later consumer/rendering faults; runtime tests are necessary.

Bento itself chooses to compose a fresh Light/Dark value dictionary with a fresh component-key dictionary referencing its own sealed styles. Only style identities are reused inside Bento; neither mutable dictionaries nor concrete brushes are cached across loads. This preserves existing templates and control state for mode changes without imposing that design on another theme. Components are local to ThemeBento; the shared engine never loads or retains them separately.

Screens consume active resources/styles through DynamicResource. Bento uses local StaticResource only for invariant inheritance/helpers in the same file. Compiled/source checks reject duplicate package keys and visual dependencies on another theme; candidate validation never borrows from Application.Current. Open windows/dialogs/popups and editing/binding state must be verified on Windows.

## Future-theme addition — independent implementation, no screen rewrite

Only after Bento approval and separate authorization:
1. Create the established stable theme directory only when real implementation begins. Supply its two appearance dictionaries and its own complete component/template/card/plate/typography/depth/state implementation, documentation and validation expectations. Do not inherit Bento, add scattered global overrides or duplicate application screens.
2. Provide a theme-owned composition factory that returns a complete package. It may share invariant implementation within that theme but must not share mutable candidate dictionaries or leak values/visuals from another theme. The shared engine assumes no visual layout, template structure or palette strategy.
3. Register one ThemeDefinition(name, stableFolder, factory) in ThemeCatalog. The orchestrator and installed-theme selector discover this real registration without view edits, theme-specific branches or separate application executables.
4. Validate the 77 public roles and 29 style target types independently; validate the theme’s own extra resources, accessibility/state/contrast expectations and duplicate/cross-theme keys. Extend per-theme source/runtime checks and CI before registration is accepted. Missing styles must fail even if the active theme supplies the same key.
5. Run research/specification/build/runtime/comparison/correction/regression/user review. Verify existing screens, windows/dialogs/popups, editing/selection/focus/scroll state and diagnostics. Preserve failure status. Do not implement an extra theme merely to demonstrate extensibility.

Only Bento is registered. [Windows CI](../../.github/workflows/windows-wpf.yml) is configured but has not verified the current unpushed code; see the [current isolation/CI evidence](../../step1.ThemeStudio/step1.c.docs/step1.c.3.Theme-Isolation-and-CI.md). Actual rendered visual inspection remains separate from compilation and runtime assertions.
