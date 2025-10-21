using Re4QuadExtremeEditor.Editor.Class;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using System;
using System.Text;
using System.Windows.Forms;

namespace Re4QuadExtremeEditor.Editor.Controls
{
    public partial class CameraMoveControl : UserControl
    {
        private readonly CameraControl CameraControl;
        private readonly ObjectControl ObjectControl;

        private bool CameraModeChangedIsEneable = false;
        private bool moveCam_InOut_mouseDown = false;
        private bool moveCam_strafe_mouseDown = false;

        // This is still set externally by MainForm
        public bool isControlDown = false;

        public CameraMoveControl(CameraControl CameraControl, ObjectControl objectControl)
        {
            this.CameraControl = CameraControl;
            InitializeComponent();
            comboBoxCameraMode.SelectedIndex = 0;
            CameraModeChangedIsEneable = true;

            comboBoxCameraMode.MouseWheel += ComboBoxCameraMode_MouseWheel;
            trackBarCamSpeed.MouseWheel += TrackBarCamSpeed_MouseWheel;
            ObjectControl = objectControl;
        }

        public void ResetCamera()
        {
            CameraModeChangedIsEneable = false;
            comboBoxCameraMode.SelectedIndex = 0;
            CameraControl.ResetCamera();
            CameraModeChangedIsEneable = true;
        }

        private void TrackBarCamSpeed_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            if (e.X >= 0 && e.Y >= 0 && e.X < trackBarCamSpeed.Width && e.Y < trackBarCamSpeed.Height)
            {
                ((HandledMouseEventArgs)e).Handled = false;
            }
        }

        private void ComboBoxCameraMode_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = !((ComboBox)sender).DroppedDown;
        }

        private void trackBarCamSpeed_Scroll(object sender, EventArgs e)
        {
            labelCamSpeedPercentage.Text = $"Camera Speed: {CameraControl.SetCameraSpeed(trackBarCamSpeed.Value)}";
        }

        private void comboBoxCameraMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CameraModeChangedIsEneable)
            {
                CameraControl.SetCameraMode(comboBoxCameraMode.SelectedIndex);
            }
        }

        private void buttonGrid_Click(object sender, EventArgs e)
        {
            ObjectControl.ToggleGrid();
        }

        private void textBoxGridSize_TextChanged(object sender, EventArgs e)
        {
            string validatedText = ObjectControl.SetGridSize(textBoxGridSize.Text);
            if (textBoxGridSize.Text != validatedText)
            {
                textBoxGridSize.Text = validatedText;
            }
        }

        private void textBoxGridSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                if (textBoxGridSize.SelectionStart < textBoxGridSize.TextLength)
                {
                    int CacheSelectionStart = textBoxGridSize.SelectionStart;
                    var sb = new StringBuilder(textBoxGridSize.Text);
                    sb[textBoxGridSize.SelectionStart] = e.KeyChar;
                    textBoxGridSize.Text = sb.ToString();
                    textBoxGridSize.SelectionStart = CacheSelectionStart + 1;
                }
            }
            e.Handled = true;
        }

        private void pictureBoxMoveCamStrafe_MouseDown(object sender, MouseEventArgs e)
        {
            moveCam_strafe_mouseDown = true;
            CameraControl.StartStrafe(e.Button);
        }

        private void pictureBoxMoveCamStrafe_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveCam_strafe_mouseDown)
            {
                CameraControl.PerformStrafe(e.X, e.Y, isControlDown);
            }
        }

        private void pictureBoxMoveCamStrafe_MouseUp(object sender, MouseEventArgs e)
        {
            moveCam_strafe_mouseDown = false;
            CameraControl.EndStrafe();
        }

        private void pictureBoxMoveCamStrafe_MouseLeave(object sender, EventArgs e)
        {
            if (moveCam_strafe_mouseDown)
            {
                moveCam_strafe_mouseDown = false;
                CameraControl.EndStrafe();
            }
        }

        private void pictureBoxMoveCamInOut_MouseDown(object sender, MouseEventArgs e)
        {
            moveCam_InOut_mouseDown = true;
            CameraControl.StartZoom(e.Y, e.Button);
        }

        private void pictureBoxMoveCamInOut_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveCam_InOut_mouseDown)
            {
                CameraControl.PerformZoom(e.Y);
            }
        }

        private void pictureBoxMoveCamInOut_MouseUp(object sender, MouseEventArgs e)
        {
            moveCam_InOut_mouseDown = false;
            CameraControl.EndZoom();
        }

        private void pictureBoxMoveCamInOut_MouseLeave(object sender, EventArgs e)
        {
            if (moveCam_InOut_mouseDown)
            {
                moveCam_InOut_mouseDown = false;
                CameraControl.EndZoom();
            }
        }

        public void StartUpdateTranslation()
        {
            labelCamSpeedPercentage.Text = Lang.GetText(eLang.labelCamSpeedPercentage) + " 100%";
            buttonGrid.Text = Lang.GetText(eLang.buttonGrid);
            labelCamModeText.Text = Lang.GetText(eLang.labelCamModeText);
            labelMoveCamText.Text = Lang.GetText(eLang.labelMoveCamText);
            CameraModeChangedIsEneable = false;
            comboBoxCameraMode.Items[0] = Lang.GetText(eLang.CameraMode_Fly);
            comboBoxCameraMode.Items[1] = Lang.GetText(eLang.CameraMode_Orbit);
            comboBoxCameraMode.Items[2] = Lang.GetText(eLang.CameraMode_Top);
            comboBoxCameraMode.Items[3] = Lang.GetText(eLang.CameraMode_Bottom);
            comboBoxCameraMode.Items[4] = Lang.GetText(eLang.CameraMode_Left);
            comboBoxCameraMode.Items[5] = Lang.GetText(eLang.CameraMode_Right);
            comboBoxCameraMode.Items[6] = Lang.GetText(eLang.CameraMode_Front);
            comboBoxCameraMode.Items[7] = Lang.GetText(eLang.CameraMode_Back);
            comboBoxCameraMode.SelectedIndex = 0;
            CameraModeChangedIsEneable = true;
        }
    }
}
