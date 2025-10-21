using NsCamera;
using Re4QuadExtremeEditor.Editor.Class;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using ReaLTaiizor.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Re4QuadExtremeEditor.Editor.Controls
{
    public partial class ViewportCameraDropdown : UserControl
    {
        private Camera camera;
        public CameraControl cameraControl;

        public ViewportCameraDropdown(ref Camera camera, CameraControl cameraControl)
        {
            this.camera = camera;
            this.cameraControl = cameraControl; 
            InitializeComponent();

            UpdateLanguage();

            ApplyTheme();
        }

        private void ApplyTheme()
        {
            ThemeManager.ApplyThemeRecursive(this);
        }

        public void UpdateControls()
        {
            //FOV
            camFovSlider.Value = (int)Globals.FOV;
            fovValueLabel.Text = $"{camFovSlider.Value}°";

            //speed
            float speedMultiplier = camera.CamSpeedMultiplier * 100f;
            int speedValue;
            if (speedMultiplier >= 100){
                speedValue = 50 + (int)((speedMultiplier - 100) / 8f);
            }else{
                speedValue = (int)((speedMultiplier / 100f) * 50f);
            }
            camSpeedSlider.Value = Math.Max(camSpeedSlider.Minimum, Math.Min(camSpeedSlider.Maximum, speedValue));
            camSpeedValueLabel.Text = cameraControl.SetCameraSpeed(camSpeedSlider.Value);

            // Camera Mode
            if (camMoveTypeCombox.Items.Count > 0){
                camMoveTypeCombox.SelectedIndex = (int)camera.CamMode;
            }
        }

        public void UpdateLanguage()
        {
            camMoveTypeCombox.Items.Clear();
            camMoveTypeCombox.Items.Add("Fly");
            camMoveTypeCombox.Items.Add("Orbit");
            camMoveTypeCombox.Items.Add("Top");
            camMoveTypeCombox.Items.Add("Bottom");
            camMoveTypeCombox.Items.Add("Left");
            camMoveTypeCombox.Items.Add("Right");
            camMoveTypeCombox.Items.Add("Front");
            camMoveTypeCombox.Items.Add("Back");
            UpdateControls();
        }

        private void camMoveTypeCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            cameraControl.SetCameraMode(camMoveTypeCombox.SelectedIndex);
        }

        private void camSpeedSlider_Scroll(object sender)
        {
            camSpeedValueLabel.Text = cameraControl.SetCameraSpeed(camSpeedSlider.Value);
        }

        private void camFovSlider_Scroll(object sender)
        {
            float newFov = camFovSlider.Value;
            cameraControl.SetFov(newFov);
            fovValueLabel.Text = $"{newFov}°";
        }
    }
}
