"""Portable source checks; these do NOT compile or execute WPF."""
from pathlib import Path
import math
import runpy
import re
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
X = '{http://schemas.microsoft.com/winfx/2006/xaml}'
P = '{http://schemas.microsoft.com/winfx/2006/xaml/presentation}'
ENGINE = ROOT / 'Theme/step1.a.ThemeEngine'
contract = dict(re.findall(r'\["([^"]+)"\] = typeof\(([^)]+)\)',
                           (ENGINE / 'step1.a.1.ThemeContract.cs').read_text().split('RequiredStyles')[0]))
assert contract, 'Typed contract missing'

def local(tag):
    return tag.split('}')[-1]

def luminance(color):
    channels = [int(color[i:i+2], 16) / 255 for i in (1, 3, 5)]
    linear = [x / 12.92 if x <= .04045 else ((x + .055) / 1.055) ** 2.4 for x in channels]
    return sum(x * weight for x, weight in zip(linear, (.2126, .7152, .0722)))

contrast_checks = 0
for mode in ('Light', 'Dark'):
    path = ROOT / f'Theme/ThemeBento/ThemeBento.{mode}.xaml'
    tree = ET.parse(path).getroot()
    keys = [item.attrib[X+'Key'] for item in tree]
    assert len(keys) == len(set(keys)), f'{mode}: duplicate token keys'
    values = {item.attrib[X+'Key']: item for item in tree}
    assert set(values) == set(contract), f'{mode}: contract mismatch: {set(values) ^ set(contract)}'
    for key, type_name in contract.items():
        assert local(values[key].tag) in ({'SolidColorBrush', 'LinearGradientBrush', 'RadialGradientBrush'} if type_name == 'Brush' else {{'double': 'Double'}.get(type_name, type_name)}), (key, type_name)
        if type_name == 'double':
            number = float(values[key].text)
            assert math.isfinite(number) and number >= 0, key
            if key.endswith('Opacity'):
                assert number <= 1, key
    pairs = [(fg, bg, 4.5) for fg in ('TextPrimary', 'TextSecondary', 'TextMuted')
             for bg in ('ApplicationBackground', 'SurfacePrimary', 'SurfaceSecondary', 'SurfaceElevated', 'StateHover', 'StatePressed', 'StateSelected', 'AccentSoft')]
    pairs += [('TextOnAccent', bg, 4.5) for bg in ('AccentPrimary', 'AccentHover', 'AccentPressed')]
    pairs += [(f'Status{s}', f'Status{s}Surface', 4.5) for s in ('Success', 'Warning', 'Error', 'Information')]
    pairs += [('AccentPrimary', 'AccentSoft', 4.5), ('AccentPrimary', 'StateSelected', 4.5)]
    pairs += [(fg, bg, 3) for fg in ('FocusIndicator', 'BorderStrong')
              for bg in ('SurfacePrimary', 'SurfaceSecondary')]
    for fg, bg, minimum in pairs:
        a, b = (luminance(values[k].attrib['Color']) for k in (fg, bg))
        ratio = (max(a, b) + .05) / (min(a, b) + .05)
        assert ratio >= minimum, f'{mode} {fg}/{bg}: {ratio:.2f} < {minimum}'
        contrast_checks += 1

xamls = [ROOT/'App.xaml', ROOT/'MainWindow.xaml', *ROOT.glob('Theme/**/*.xaml'),
         *ROOT.glob('step1.ThemeStudio/step1.b.ThemeStudioScreen/*.xaml')]
style_keys = set()
for path in xamls:
    tree = ET.parse(path).getroot()
    for node in tree.iter():
        if X+'Key' in node.attrib:
            style_keys.add(node.attrib[X+'Key'])
        if local(node.tag) == 'Style' and 'TargetType' in node.attrib:
            style_keys.add('{x:Type '+node.attrib['TargetType']+'}')
        for attribute in node.attrib:
            name = local(attribute)
            assert name not in ('LetterSpacing', 'box-shadow', 'backdrop-filter', 'gap'), (path, name)
        # Catch a property assigned as both attribute and property element.
        for child in node:
            child_name = local(child.tag)
            if '.' in child_name:
                prop = child_name.split('.', 1)[1]
                assert prop not in node.attrib, f'{path}: duplicate assignment to {prop}'
        if local(node.tag) in ('Border', 'Button', 'UserControl', 'ScrollViewer', 'Window'):
            direct_content = [c for c in node if '.' not in local(c.tag)]
            assert len(direct_content) <= 1, f'{path}: multiple content children'
    # Dictionary-local key uniqueness (different dictionaries may share implicit keys by design).
    if local(tree.tag) == 'ResourceDictionary':
        direct_keys = [n.attrib[X+'Key'] for n in tree if X+'Key' in n.attrib]
        assert len(direct_keys) == len(set(direct_keys)), f'{path}: duplicate keys'

for path in xamls:
    text = path.read_text()
    for token in re.findall(r'\{DynamicResource ([^{}]+)\}', text):
        assert token in contract or token in style_keys, f'{path}: undefined dynamic token {token}'
    for reference in re.findall(r'\{StaticResource (\{x:Type [^}]+\}|[^{}]+)\}', text):
        assert reference in style_keys, f'{path}: undefined style {reference}'
    for node in ET.parse(path).getroot().iter():
        if local(node.tag) == 'ResourceDictionary' and 'Source' in node.attrib:
            assert (path.parent / node.attrib['Source']).is_file(), (path, node.attrib['Source'])
    if 'Theme/ThemeBento/' not in path.as_posix():
        assert not re.search(r'#[0-9A-Fa-f]{6,8}\b', text), f'{path}: scattered literal color'
    if path.parent.name == 'step1.b.ThemeStudioScreen':
        for prop in ('FontSize', 'CornerRadius', 'Padding', 'Margin', 'Foreground', 'Background', 'BorderBrush'):
            for value in re.findall(rf'\b{prop}="([^"]+)"', text):
                assert value.startswith(('{DynamicResource ', '{Binding ')), (path, prop, value)

# Check x:Class/code-behind pairing and declared handlers without claiming compilation.
for path in xamls:
    tree = ET.parse(path).getroot()
    class_name = tree.attrib.get(X+'Class')
    if not class_name:
        continue
    behind = Path(str(path) + '.cs')
    assert behind.is_file(), f'{path}: missing code-behind'
    namespace, name = class_name.rsplit('.', 1)
    code = behind.read_text()
    assert f'namespace {namespace};' in code and f'partial class {name} ' in code, (path, class_name)
    for node in tree.iter():
        for event in ('Click', 'SizeChanged', 'Loaded'):
            if event in node.attrib:
                assert re.search(r'\b' + re.escape(node.attrib[event]) + r'\s*\(', code), (path, event)

project = ET.parse(ROOT/'ProGlassAutomation.csproj').getroot()
assert not project.findall('.//PackageReference'), 'No product dependencies needed'
assert project.find('.//TargetFramework').text == 'net8.0-windows7.0'
assert not any(ROOT.glob('Theme/ThemeClaymorphism*')), 'Bento-only phase'
print(f'PASS: {len(contract)} typed tokens × 2 complete variants; {contrast_checks} measured contrast pairs.')
print(f'PASS: {len(xamls)} XML documents, content/property checks, resource references, dictionary paths and no view-local styling literals.')
print('LIMITATION: source checks are not a C#/XAML compilation, runtime switching test, visual inspection, or accessibility certification.')

runpy.run_path(str(ROOT / 'Theme/step1.d.tests/step1.d.5.verify_isolation.py'), run_name='__main__')
