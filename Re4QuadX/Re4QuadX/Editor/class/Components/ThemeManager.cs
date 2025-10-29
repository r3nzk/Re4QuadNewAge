using NsMultiselectTreeView;
using PowerLib.Winform.Controls;
using Re4QuadX.Editor.Class.CustomDelegates;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Re4QuadX.Editor.Class
{
    public enum EditorTheme
    {
        Dark,
        Light,
        Custom
    }

    public struct ColorPalette
    {
        public Color FormBackground;
        public Color Background;
        public Color BackgroundSubtle;
        public Color BackgroundDarker;
        public Color Text;
        public Color TextBright;
        public Color TextDarker;
        public Color Highlight;
        public Color TextOnHighlight;
        public Color Border;
        public Color Line;

        public Color ConsoleInfo;
        public Color ConsoleWarning;
        public Color ConsoleError;
    }

    public static class ThemeManager
    {
        private static readonly Dictionary<EditorTheme, ColorPalette> Palettes;

        static ThemeManager()
        {
            Palettes = new Dictionary<EditorTheme, ColorPalette>();

            var darkPalette = new ColorPalette
            {
                FormBackground = ColorTranslator.FromHtml("#1F1F1F"),
                Background = ColorTranslator.FromHtml("#181818"),
                BackgroundSubtle = ColorTranslator.FromHtml("#1c1c1c"),
                BackgroundDarker = ColorTranslator.FromHtml("#121212"),
                Text = ColorTranslator.FromHtml("#BABABA"),
                TextBright = ColorTranslator.FromHtml("#DBDBDB"),
                TextDarker = ColorTranslator.FromHtml("#878787"),
                Highlight = ColorTranslator.FromHtml("#4F4F4F"),
                TextOnHighlight = Color.White,
                Border = ColorTranslator.FromHtml("#1F1F1F"),
                Line = ColorTranslator.FromHtml("#4A4A4A"),

                ConsoleInfo = ColorTranslator.FromHtml("#adadad"),
                ConsoleWarning = ColorTranslator.FromHtml("#c2b34f"),
                ConsoleError = ColorTranslator.FromHtml("#c26161")
            };
            Palettes.Add(EditorTheme.Dark, darkPalette);

            var lightPalette = new ColorPalette
            {
                //most basic colors are hardcoded as default in form design

                ConsoleInfo = ColorTranslator.FromHtml("#1F1F1F"),
                ConsoleWarning = ColorTranslator.FromHtml("#c2b34f"),
                ConsoleError = ColorTranslator.FromHtml("#c26161")
            };
            Palettes.Add(EditorTheme.Light, lightPalette);
        }

        public static ColorPalette GetCurrentPalette()
        {
            EditorTheme theme = Globals.BackupConfigs.SelectedTheme;
            if (Palettes.TryGetValue(theme, out var palette))
            {
                return palette;
            }
            return Palettes[EditorTheme.Light];
        }

        public static void ApplyTheme(Control control)
        {
            EditorTheme theme = Globals.BackupConfigs.SelectedTheme;

            if (!Palettes.TryGetValue(theme, out var palette))
            {
                palette = Palettes[EditorTheme.Dark];
            }

            if (control is XButton button) { ApplyButtonTheme(button, palette); }
            else if (control is SkyComboBox comboBox) { ApplyComboBoxTheme(comboBox, palette); }
            else if (control is PropertyGrid grid) { ApplyPropertyGridTheme(grid, palette); }
            else if (control is MultiselectTreeView tree) { ApplyTreeViewTheme(tree, palette); }
            else if (control is XTabControl tabControl) { ApplyTabControlTheme(tabControl, palette); }
            else if (control is MenuStrip menu) { ApplyMenuStripTheme(menu, palette); }
            else if (control is ToolStrip toolStrip) { ApplyToolStripTheme(toolStrip, palette); }
            else if (control is SplitContainer splitter) { ApplySplitContainerTheme(splitter, palette); }
            else if (control is RichTextBox richTextBox) { ApplyRichTextBoxTheme(richTextBox, palette); }
            else if (control is TextBox textBox) { ApplyTextBoxTheme(textBox, palette); }
            else if (control is ListBox listBox) { ApplyListBoxTheme(listBox, palette); }
            else if (control is System.Windows.Forms.CheckBox checkBox) { ApplyCheckBoxTheme(checkBox, palette); }
            else if (control is Label label) { label.ForeColor = palette.Text; label.BackColor = Color.Transparent; }
            else if (control is System.Windows.Forms.Panel panel) { panel.BackColor = palette.Background; }
            else if (control is Form form) { form.BackColor = palette.FormBackground; }
            else if (control is UserControl userControl) { userControl.BackColor = palette.Background; userControl.ForeColor = palette.Text; }
        }

        public static void ApplyThemeRecursive(Control parentControl)
        {
            ApplyTheme(parentControl);

            foreach (Control childControl in parentControl.Controls)
            {
                ApplyThemeRecursive(childControl);
            }
        }

        private static void ApplyButtonTheme(XButton button, ColorPalette palette)
        {
            button.StartColor = palette.BackgroundSubtle;
            button.EndColor = palette.Background;
            button.ForeColor = palette.Text;
            button.BorderColor = palette.Background;
            button.HoldingStartColor = palette.Highlight;
            button.HoldingEndColor = palette.Highlight;
            button.HoldingForeColor = palette.TextOnHighlight;
            button.CheckedStartColor = palette.Highlight;
            button.CheckedEndColor = palette.Highlight;
            button.CheckedForeColor = palette.TextOnHighlight;
            button.DefaultButtonBorderColor = palette.Highlight;
        }

        private static void ApplyCheckBoxTheme(System.Windows.Forms.CheckBox checkBox, ColorPalette palette)
        {
            checkBox.ForeColor = palette.Text;
            checkBox.BackColor = Color.Transparent;
        }

        private static void ApplyComboBoxTheme(SkyComboBox comboBox, ColorPalette palette)
        {
            comboBox.BGColorA = palette.BackgroundSubtle;
            comboBox.BGColorB = palette.Background;
            if (comboBox is Control c) { c.ForeColor = palette.Text; }
            comboBox.BorderColorA = palette.Background;
            comboBox.BorderColorB = palette.Background;
            comboBox.BorderColorC = palette.Background;
            comboBox.BorderColorD = palette.Background;
            comboBox.ListBackColor = palette.Background;
            comboBox.ListForeColor = palette.Text;
            comboBox.ForeColor = palette.Text; 
            comboBox.ListBorderColor = palette.Highlight;
            comboBox.ItemHighlightColor = palette.Highlight;
            comboBox.ListSelectedBackColorA = comboBox.ListSelectedBackColorB = palette.Highlight;
            comboBox.LineColorA = comboBox.LineColorB = comboBox.LineColorC = palette.Background;
            comboBox.TriangleColorA = palette.Text;
            comboBox.TriangleColorB = palette.Text;
        }

        private static void ApplyPropertyGridTheme(PropertyGrid grid, ColorPalette palette)
        {
            grid.BackColor = palette.Background;
            grid.ViewBackColor = palette.Background;
            grid.ViewForeColor = palette.TextBright;
            grid.LineColor = palette.BackgroundSubtle;
            grid.CategorySplitterColor = palette.Border;
            grid.HelpBackColor = palette.Background;
            grid.HelpForeColor = palette.TextDarker;
            grid.HelpBorderColor = palette.Border;
            grid.ViewBorderColor = palette.Border;
            grid.CategoryForeColor = palette.TextOnHighlight;
            grid.DisabledItemForeColor = palette.Text;
            grid.SelectedItemWithFocusBackColor = palette.Highlight;
            grid.SelectedItemWithFocusForeColor = palette.TextOnHighlight;
        }

        private static void ApplyTreeViewTheme(MultiselectTreeView tree, ColorPalette palette)
        {
            tree.BackColor = palette.Background;
            tree.ForeColor = palette.Text;
            tree.LineColor = palette.Line;
            tree.SelectedNodeBackColor = palette.Highlight;
        }
        private static void ApplyTabControlTheme(XTabControl panel, ColorPalette palette)
        {
            panel.BackColor = palette.Border;
            panel.BorderColor = palette.Border;
            panel.ForeColor = palette.Text;
            panel.HeaderBackColorStart = palette.Border;
            panel.HeaderBackColorEnd = palette.Border;
            panel.HeaderForeColor = palette.Text;
            panel.HeaderSelectedBackColorStart = palette.Background;
            panel.HeaderSelectedBackColorEnd = palette.Background;
            panel.HeaderSelectedForeColor = palette.Text;
            foreach (System.Windows.Forms.TabPage tabPage in panel.TabPages)
            {
                tabPage.BackColor = palette.Background;
            }
        }

        private static void ApplyMenuStripTheme(MenuStrip menu, ColorPalette palette)
        {
            menu.BackColor = palette.BackgroundDarker;
            menu.ForeColor = palette.Text;
            menu.Renderer = new CustomToolStripRenderer(palette);
        }

        private static void ApplyToolStripTheme(ToolStrip toolStrip, ColorPalette palette)
        {
            toolStrip.BackColor = palette.BackgroundDarker;
            toolStrip.Renderer = new CustomToolStripRenderer(palette);
        }

        private static void ApplySplitContainerTheme(SplitContainer splitter, ColorPalette palette)
        {
            splitter.BackColor = palette.BackgroundDarker;
        }

        private static void ApplyRichTextBoxTheme(RichTextBox rtb, ColorPalette palette)
        {
            rtb.BackColor = palette.Background;
            rtb.ForeColor = palette.Text;
        }

        private static void ApplyTextBoxTheme(TextBox textBox, ColorPalette palette)
        {
            textBox.BackColor = palette.BackgroundDarker;
            textBox.ForeColor = palette.Text;
        }

        private static void ApplyListBoxTheme(ListBox listBox, ColorPalette palette)
        {
            listBox.BackColor = palette.Background;
            listBox.ForeColor = palette.Text;
        }

        private class CustomProfessionalColorTable : ProfessionalColorTable
        {
            private readonly ColorPalette _palette;

            public CustomProfessionalColorTable(ColorPalette palette)
            {
                _palette = palette;
                UseSystemColors = false;
            }

            //dropdowns
            public override Color MenuBorder => _palette.Highlight;
            public override Color MenuItemBorder => _palette.TextDarker;
            public override Color MenuItemSelected => _palette.Highlight;
            public override Color MenuItemPressedGradientBegin => _palette.Highlight;
            public override Color MenuItemPressedGradientEnd => _palette.Highlight;
            public override Color MenuItemSelectedGradientBegin => _palette.Highlight;
            public override Color MenuItemSelectedGradientEnd => _palette.Highlight;
            public override Color ToolStripDropDownBackground => _palette.BackgroundSubtle;
            public override Color ImageMarginGradientBegin => _palette.BackgroundSubtle;
            public override Color ImageMarginGradientMiddle => _palette.BackgroundSubtle;
            public override Color ImageMarginGradientEnd => _palette.BackgroundSubtle;

            //toolstrip overrides
            public override Color ToolStripPanelGradientBegin => Color.FromArgb(28, 28, 28);
            public override Color ToolStripPanelGradientEnd => Color.FromArgb(28, 28, 28);
            public override Color ToolStripBorder => Color.Transparent;
            public override Color ToolStripGradientBegin => Color.Transparent;
            public override Color ToolStripGradientEnd => Color.Transparent;
            public override Color ToolStripGradientMiddle => Color.Transparent;
            public override Color ButtonSelectedBorder => ColorTranslator.FromHtml("#5c5c5c");
            public override Color ButtonSelectedGradientBegin => ColorTranslator.FromHtml("#404040");
            public override Color ButtonSelectedGradientEnd => ColorTranslator.FromHtml("#404040");
            public override Color ButtonSelectedHighlight => ColorTranslator.FromHtml("#404040");
            public override Color OverflowButtonGradientBegin => Color.Transparent;
            public override Color OverflowButtonGradientEnd => Color.Transparent;
            public override Color SeparatorDark => ColorTranslator.FromHtml("#525252");
            public override Color SeparatorLight => ColorTranslator.FromHtml("#525252");
        }

        public class CustomToolStripRenderer : ToolStripProfessionalRenderer
        {
            private readonly ColorPalette _palette;

            public CustomToolStripRenderer(ColorPalette palette) : base(new CustomProfessionalColorTable(palette))
            {
                _palette = palette;
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = _palette.Text;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = _palette.Text;
                base.OnRenderArrow(e);
            }
        }
    }
}