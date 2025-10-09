using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Re4QuadExtremeEditor.Editor.Forms.Custom
{
    public class CustomDarkRenderer : ToolStripProfessionalRenderer
    {
        public CustomDarkRenderer() : base(new CustomDarkColorTable()) { }
    }
    public class CustomDarkColorTable : ProfessionalColorTable
    {
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
}
