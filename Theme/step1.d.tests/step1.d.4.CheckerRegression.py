"""Prove the portable validator rejects representative regressions, in temporary copies."""
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile

ROOT = Path(__file__).resolve().parents[2]
PALETTE = 'Theme/ThemeBento/ThemeBento.Light.xaml'
BUTTONS = 'Theme/ThemeBento/step1.e.Components/step1.e.3.Buttons.xaml'
FOUNDATION = 'Theme/ThemeBento/step1.e.Components/step1.e.2.Foundation.xaml'
VIEW = 'step1.ThemeStudio/step1.b.ThemeStudioScreen/step1.b.6.OverviewPanel.xaml'
MUTATIONS = [
    ('missing visual style', BUTTONS, 'x:Key="PrimaryButton"', 'x:Key="NotPrimaryButton"'),
    ('wrong visual target', BUTTONS, 'x:Key="PrimaryButton" TargetType="Button"', 'x:Key="PrimaryButton" TargetType="TextBox"'),
    ('cross-theme source', 'Theme/ThemeBento/step1.e.Components/step1.e.1.Components.xaml', 'Source="step1.e.2.Foundation.xaml"', 'Source="../../step1.a.ThemeEngine/Forbidden.xaml"'),
    ('duplicate package key', FOUNDATION, '<Style x:Key="KeyboardFocus"', '<Style x:Key="PrimaryButton" TargetType="Button" />\n <Style x:Key="KeyboardFocus"'),
    ('missing token', PALETTE, 'x:Key="TextPrimary"', 'x:Key="MissingTextPrimary"'),
    ('wrong type', PALETTE, '<sys:Double x:Key="SpacingMedium">8</sys:Double>', '<Thickness x:Key="SpacingMedium">8</Thickness>'),
    ('contrast regression', PALETTE, 'Color="#202334"', 'Color="#FFFFFF"'),
    ('undefined resource', VIEW, '{DynamicResource TextSecondary}', '{DynamicResource DoesNotExist}'),
    ('screen-local palette', VIEW, 'Foreground="{DynamicResource TextSecondary}"', 'Foreground="#123456"'),
    ('duplicate property', VIEW, '<Border Style="{DynamicResource Card}">', '<Border Style="{DynamicResource Card}"><Border.Style><Style TargetType="Border" /></Border.Style>'),
]
baseline = subprocess.run([sys.executable, str(ROOT / 'Theme/step1.d.tests/step1.d.3.verify_sources.py')], capture_output=True, text=True)
assert baseline.returncode == 0, ('Baseline must pass before mutations', baseline.stdout, baseline.stderr)
for name, relative, before, after in MUTATIONS:
    with tempfile.TemporaryDirectory(prefix='proglass-check-') as temporary:
        copy = Path(temporary)
        for directory in ('Theme', 'step1.ThemeStudio'):
            shutil.copytree(ROOT / directory, copy / directory, ignore=shutil.ignore_patterns('bin', 'obj', '__pycache__'))
        for filename in ('App.xaml', 'App.xaml.cs', 'MainWindow.xaml', 'MainWindow.xaml.cs', 'ProGlassAutomation.csproj'):
            shutil.copyfile(ROOT / filename, copy / filename)
        target = copy / relative
        original = target.read_text()
        assert before in original, name
        target.write_text(original.replace(before, after, 1))
        result = subprocess.run([sys.executable, str(copy / 'Theme/step1.d.tests/step1.d.3.verify_sources.py')], capture_output=True, text=True)
        expected = {
            'missing visual style': 'undefined dynamic token PrimaryButton',
            'wrong visual target': 'wrong theme style target', 'cross-theme source': 'Forbidden.xaml',
            'duplicate package key': 'duplicate package key: PrimaryButton',
            'missing token': 'contract mismatch', 'wrong type': 'SpacingMedium',
            'contrast regression': 'TextPrimary/', 'undefined resource': 'undefined dynamic token',
            'screen-local palette': 'scattered literal color', 'duplicate property': 'duplicate assignment',
        }[name]
        assert result.returncode != 0 and 'AssertionError' in result.stderr and expected in result.stderr, (name, result.stdout, result.stderr)
        print('PASS: rejected', name)
print(f'PASS: {len(MUTATIONS)} validator mutation checks; original files were not modified.')
