"""Portable clean-foundation guards; never compilation or rendered-UI evidence."""
from pathlib import Path
import re
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
inventory = ROOT / 'step1.ThemeStudio/step1.c.docs/step1.c.4.Cleanup-Inventory.md'
removed = re.findall(r'^\| `([^`]+)` \| Remove \|', inventory.read_text(), re.M)
assert len(removed) == 47, 'Cleanup inventory must retain all 47 inspected legacy paths'
for relative in removed:
    assert not (ROOT / relative).exists(), f'Legacy file returned: {relative}'
for directory in ('SheetStore', 'Converters', 'Shared', 'ViewModels'):
    assert not (ROOT / directory).exists(), f'Legacy tree returned: {directory}'

project = ET.parse(ROOT / 'ProGlassAutomation.csproj').getroot()
assert project.find('.//TargetFramework').text == 'net8.0-windows7.0'
assert [(p.attrib['Include'], p.attrib['Version']) for p in project.findall('.//PackageReference')] == [('LiveChartsCore.SkiaSharpView.WPF', '2.0.4')], 'Only the reviewed pinned WPF chart dependency is approved'
for kind in ('Compile', 'Page'):
    included = []
    for item in project.findall(f'.//{kind}'):
        for pattern in item.attrib['Include'].split(';'):
            paths = [p for p in ROOT.glob(pattern) if p.is_file()]
            assert paths, f'Broken project glob: {pattern}'
            included.extend(paths)
    assert len(included) == len(set(included)), f'Duplicate {kind} items'
    if kind == 'Compile':
        expected = set(ROOT.glob('*.cs')) | set(ROOT.glob('Theme/step1.a.ThemeEngine/**/*.cs')) | set(ROOT.glob('Theme/ThemeBento/**/*.cs')) | set(ROOT.glob('Theme/ThemeClaymorphism/**/*.cs')) | set(ROOT.glob('Theme/ThemeFluent/**/*.cs')) | set(ROOT.glob('Theme/ThemeNeumorphism/**/*.cs')) | set(ROOT.glob('Theme/step1.f.Analytics/**/*.cs')) | set(ROOT.glob('Theme/step1.g.Customization/**/*.cs')) | set(ROOT.glob('step1.ThemeStudio/step1.b.ThemeStudioScreen/**/*.cs'))
        assert set(included) == expected, 'Application code missing from project'
    print(f'PASS: {len(included)} {kind} items; existing paths and no duplicates.')

for path in [*ROOT.glob('*.cs'), *ROOT.glob('Theme/**/*.cs'), *ROOT.glob('step1.ThemeStudio/**/*.cs'), *ROOT.glob('**/*.xaml')]:
    if any(part in ('bin', 'obj') for part in path.parts):
        continue
    text = path.read_text()
    assert not re.search(r'ProGlassAutomation\.(SheetStore|Converters|ViewModels)|Microsoft\.Data\.Sqlite|Dapper|ShellStyles|SheetStoreModule', text), f'Legacy reference: {path}'
app = (ROOT / 'App.xaml.cs').read_text()
assert 'Themes.Initialize(Resources)' in app and 'MainWindow.Show()' in app
runtime = (ROOT / 'Theme/step1.d.tests/step1.d.1.RuntimeChecks.cs').read_text()
assert 'ThemeService.InstalledThemes' in runtime and 'CanCustomize' in runtime and 'ThemeService.Fluent' in runtime
assert 'Apply("Claymorphism"' not in runtime, 'Clay is installed; do not treat it as missing'
shell = (ROOT/'MainWindow.xaml').read_text()
assert '<studio:ThemeStudioView />' in shell and '<studio:BusinessReferenceView />' in shell
assert 'SelectedIndex="{Binding SelectedWorkspace, Mode=TwoWay}"' in shell
assert 'WindowState="Maximized"' in shell
screen = ROOT/'step1.ThemeStudio/step1.b.ThemeStudioScreen'
view = (screen/'step1.b.1.ThemeStudioView.xaml').read_text()
sample = (screen/'step1.b.18.BusinessReferenceView.xaml').read_text()
assert '<local:CustomizationPanel' in view and '<local:TokensPanel' in view
assert 'CustomizationPanel' not in sample and 'TokensPanel' not in sample, 'Business page must not contain engine settings'
assert 'MaxWidth=' not in view and 'MaxWidth=' not in sample, 'Use full viewport width'
assert 'bento:BentoGrid' in view and 'bento:BentoGrid' in sample, 'Studio and reference must use the Bento grid'
assert 'FillAvailable="True"' in view and 'FillAvailable="True"' in sample, 'Studio and reference mosaics fill the viewport'
assert 'Compact="True"' in view, 'Studio preview must use the compact 2×3 mosaic, not a nested full page'
assert 'ScrollViewer' in sample and 'BusinessModules' in sample, 'Home mosaic must scroll when it is taller than the viewport'
assert 'UniformGrid' not in sample, 'Equal KPI strips are not a bento mosaic'
assert 'BentoGrid.ColumnSpan' in sample and 'BentoGrid.RowSpan' in sample, 'Reference mosaic must declare spans'
for component in ('OverviewPanel', 'AnalyticsOverview', 'WorklistPanel', 'ControlsPanel'):
    assert component in sample, f'Missing business reference pattern: {component}'
grid = ROOT / 'Theme/ThemeBento/step1.e.11.BentoGrid.cs'
assert grid.is_file() and 'class BentoGrid : Panel' in grid.read_text() and 'PlacePacked' in grid.read_text()
settings = (screen/'step1.b.16.CustomizationPanel.xaml').read_text()
for name in ('ThemePicker', 'FontPicker', 'ColorPlatePicker', 'AccentPicker', 'DensityPicker', 'ShapePicker', 'TypePicker'):
    assert f'x:Name="{name}"' in settings
for prop in ('SelectedAccent', 'SelectedDensity', 'SelectedShape', 'SelectedTypography'):
    assert f'Binding {prop}' in settings and f'public ' in (screen/'step1.b.17.Customization.cs').read_text()
assert 'Binding IsDark' in settings and 'Binding ResetDesign' in settings
active = ROOT / '.github/workflows/windows-wpf.yml'
parked = ROOT / 'cols/ci/windows-wpf.yml'
workflow = active if active.is_file() else parked
assert workflow.is_file(), 'Windows workflow YAML missing from .github/workflows or cols/ci'
# Owner may place the file in .github/workflows. Never require that path to be absent.
for command in ('dotnet build ProGlassAutomation.csproj', '--contracts-only', 'step1.d.6.verify_foundation.py'):
    assert command in workflow.read_text(), f'Missing CI verification: {command}'
print('PASS: 47 inventoried legacy files absent; no legacy source/startup dependencies; separate customization/business pages, live settings and Windows workflow present.')
print('LIMITATION: this is a source audit, not Windows compilation, runtime or visual inspection.')
