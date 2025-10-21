using NsCamera;
using Re4QuadExtremeEditor.Editor.Class;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using ReaLTaiizor.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Re4QuadExtremeEditor.Editor.Controls
{
    public partial class ViewportGizmoDropdown : UserControl
    {
        private Camera camera;
        public ObjectControl objectControl;
        private Gizmo gizmo;

        public ViewportGizmoDropdown(ref Camera camera, ObjectControl objectControl, Gizmo gizmo)
        {
            this.camera = camera;
            this.objectControl = objectControl; 
            this.gizmo = gizmo;
            InitializeComponent();

            drawGridCheckbox.Checked = Globals.CamGridEnable;

            UpdateDisplayValues();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyThemeRecursive(this);
        }

        public void UpdateDisplayValues()
        {
            drawGridCheckbox.CheckedChanged -= drawGridCheckbox_CheckedChanged;
            drawGridCheckbox.Checked = Globals.CamGridEnable;
            gridSizeTextbox.Text = Globals.CamGridvalue.ToString();

            drawGridCheckbox.CheckedChanged += drawGridCheckbox_CheckedChanged;
        }
        private void drawGridCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            objectControl.ToggleGrid();
        }
        private void gridSizeTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gridSizeTextbox.Text = objectControl.SetGridSize(gridSizeTextbox.Text);
                e.SuppressKeyPress = true;
                this.ActiveControl = null;
            }
        }

        private void gridSizeTextbox_Validated(object sender, EventArgs e)
        {
            gridSizeTextbox.Text = objectControl.SetGridSize(gridSizeTextbox.Text);
        }
    }
}
