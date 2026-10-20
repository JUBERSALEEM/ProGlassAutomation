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
assert not project.findall('.//PackageReference'), 'No database/UI/DI packages needed'
for kind in ('Compile', 'Page'):
    included = []
    for item in project.findall(f'.//{kind}'):
        for pattern in item.attrib['Include'].split(';'):
            paths = [p for p in ROOT.glob(pattern) if p.is_file()]
            assert paths, f'Broken project glob: {pattern}'
            included.extend(paths)
    assert len(included) == len(set(included)), f'Duplicate {kind} items'
    if kind == 'Compile':
        expected = set(ROOT.glob('*.cs')) | set(ROOT.glob('Theme/step1.a.ThemeEngine/**/*.cs')) | set(ROOT.glob('Theme/ThemeBento/**/*.cs')) | set(ROOT.glob('step1.ThemeStudio/step1.b.ThemeStudioScreen/**/*.cs'))
        assert set(included) == expected, 'Application code missing from project'
    print(f'PASS: {len(included)} {kind} items; existing paths and no duplicates.')

for path in [*ROOT.glob('*.cs'), *ROOT.glob('Theme/**/*.cs'), *ROOT.glob('step1.ThemeStudio/**/*.cs'), *ROOT.glob('**/*.xaml')]:
    if any(part in ('bin', 'obj') for part in path.parts):
        continue
    text = path.read_text()
    assert not re.search(r'ProGlassAutomation\.(SheetStore|Converters|ViewModels)|Microsoft\.Data\.Sqlite|Dapper|ShellStyles|SheetStoreModule', text), f'Legacy reference: {path}'
app = (ROOT / 'App.xaml.cs').read_text()
assert 'Themes.Initialize(Resources)' in app and 'MainWindow.Show()' in app
assert '<studio:ThemeStudioView />' in (ROOT / 'MainWindow.xaml').read_text()
view = (ROOT / 'step1.ThemeStudio/step1.b.ThemeStudioScreen/step1.b.1.ThemeStudioView.xaml').read_text()
for component in ('Overview', 'Appearance', 'Metrics', 'Controls', 'TypeAndStatus', 'Worklist', 'Surfaces', 'Tokens'):
    assert f'x:Name="{component}"' in view, f'Missing reference section: {component}'
assert 'x:Name="ThemePicker"' in view and 'Binding IsDark' in view
workflow = ROOT / 'cols/ci/windows-wpf.yml'
assert workflow.is_file() and not (ROOT / '.github/workflows/windows-wpf.yml').exists()
for command in ('dotnet build ProGlassAutomation.csproj', '--contracts-only', 'step1.d.6.verify_foundation.py'):
    assert command in workflow.read_text(), f'Missing CI verification: {command}'
print('PASS: 47 inventoried legacy files absent; no legacy source/startup dependencies; all eight reference sections, live header and Windows workflow present.')
print('LIMITATION: this is a source audit, not Windows compilation, runtime or visual inspection.')
