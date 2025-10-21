using NsCamera;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Re4QuadExtremeEditor.Editor.Class
{
    public class CameraControl
    {
        private readonly Camera camera;
        private readonly CustomDelegates.ActivateMethod updateGL;
        private readonly Action UpdateProjectionMatrix;

        private bool moveCam_Invert = false;
        private int moveCam_InOut_lastPosY = 0;

        public CameraControl(ref Camera camera, 
            CustomDelegates.ActivateMethod updateGL, 
            Action updateProjectionMatrix)
        {
            this.camera = camera;
            this.updateGL = updateGL;
            this.UpdateProjectionMatrix = updateProjectionMatrix;
        }
        public void SetFov(float fov)
        {
            Globals.FOV = (int)Math.Max(10f, Math.Min(150f, fov));

            TryUpdateProjectionMatrix();
        }

        public string SetCameraSpeed(int trackBarValue)
        {
            float newValue;
            if (trackBarValue > 50)
            {
                newValue = 100.0f + ((trackBarValue - 50) * 8f);
            }
            else
            {
                newValue = (trackBarValue / 50.0f) * 100f;
            }
            if (newValue < 1f)
            {
                newValue = 1f;
            }
            else if (newValue > 96f && newValue < 114f)
            {
                newValue = 100f;
            }

            camera.CamSpeedMultiplier = newValue / 100.0f;
            return (newValue).ToString();
        }

        public void SetCameraMode(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0: // Fly
                    camera.SetToFlyMode();

                    break;
                case 1: // Orbit
                    camera.SetToOrbitMode();
                    break;
                case 2: // Top
                    camera.setCameraMode_LookDirection(Camera.LookDirection.TOP);
                    break;
                case 3: // Bottom
                    camera.setCameraMode_LookDirection(Camera.LookDirection.BOTTOM);
                    break;
                case 4: // Left
                    camera.setCameraMode_LookDirection(Camera.LookDirection.LEFT);
                    break;
                case 5: // Right
                    camera.setCameraMode_LookDirection(Camera.LookDirection.RIGHT);
                    break;
                case 6: // Front
                    camera.setCameraMode_LookDirection(Camera.LookDirection.FRONT);
                    break;
                case 7: // Back
                    camera.setCameraMode_LookDirection(Camera.LookDirection.BACK);
                    break;
            }

            TryUpdateProjectionMatrix();
        }

        public void ResetCamera()
        {
            camera.ResetCameraToZero();
            TryUpdateProjectionMatrix();
        }

        private void TryUpdateProjectionMatrix()
        {
            if (UpdateProjectionMatrix != null && updateGL != null) {
                UpdateProjectionMatrix?.Invoke();
                updateGL?.Invoke();
            }
        }

        public void StartStrafe(MouseButtons button)
        {
            camera.resetMouseStuff();
            camera.SaveCameraPosition();
            moveCam_Invert = (button == MouseButtons.Right);
        }

        public void PerformStrafe(int x, int y, bool isControlDown)
        {
            camera.updateCameraOffsetMatrixWithMouse(isControlDown, x, y, moveCam_Invert);
            TryUpdateProjectionMatrix();
        }
        public void EndStrafe()
        {
            camera.resetMouseStuff();
            camera.SaveCameraPosition();
            moveCam_Invert = false;
        }

        public void StartZoom(int y, MouseButtons button)
        {
            moveCam_InOut_lastPosY = y;
            moveCam_Invert = (button == MouseButtons.Right);
        }

        public void PerformZoom(int y)
        {
            camera.resetMouseStuff();
            camera.updateCameraMatrixWithScrollWheel((y - moveCam_InOut_lastPosY) * -10, moveCam_Invert);
            camera.SaveCameraPosition();
            moveCam_InOut_lastPosY = y;
            TryUpdateProjectionMatrix();
        }
        public void EndZoom()
        {
            moveCam_Invert = false;
        }
    }
}
