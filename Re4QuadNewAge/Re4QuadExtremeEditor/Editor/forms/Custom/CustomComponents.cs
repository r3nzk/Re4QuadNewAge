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
        public override Color ButtonSelectedBorder => Color.FromArgb(28, 28, 28);
        public override Color ButtonSelectedGradientBegin => Color.FromArgb(40, 40, 40);
        public override Color ButtonSelectedGradientEnd => Color.FromArgb(40, 40, 40);
    }

    public class CustomSearchBox : TextBox
    {

        private Padding _padding = new Padding(3, 3, 3, 3);
        private string _placeholderText = "Pesquisar...";
        private bool _showPlaceholder = true;

        [Category("Layout")]
        [Description("Define o espaçamento interno do texto.")]
        public new Padding Padding
        {
            get { return _padding; }
            set
            {
                if (_padding != value)
                {
                    _padding = value;
                    this.Invalidate();
                }
            }
        }

        [Category("Aparência")]
        [Description("Define o texto de dica que aparece quando o campo está vazio.")]
        public string PlaceholderText
        {
            get { return _placeholderText; }
            set
            {
                if (_placeholderText != value)
                {
                    _placeholderText = value;
                    this.Invalidate();
                }
            }
        }

        public CustomSearchBox()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
        }


        protected override void OnEnter(EventArgs e)
        {
            _showPlaceholder = false;
            this.Invalidate();
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                _showPlaceholder = true;
            }
            this.Invalidate();
            base.OnLeave(e);
        }
    }
}
