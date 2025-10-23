using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Re4QuadX.Editor.Class.Enums;
using Re4QuadX.Editor.Class.TreeNodeObj;
using Re4QuadX.Editor.Class;
using OpenTK;
using NsCamera;

namespace Re4QuadX.Editor.Controls
{
    public partial class ObjectMoveControl : UserControl
    {
        private ObjectControl ObjectControl;

        private bool comboBoxMoveMode_IsChangeable = false;
        private bool checkBoxLockMoveSquareHorizontal_IsChangeable = true;
        private bool checkBoxLockMoveSquareVertical_IsChangeable = true;

        private MoveControlType activeDrag = MoveControlType.Null;
        private MouseEventArgs latestMouseEvent = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);

        // Public properties so MainForm can access the state
        public MoveControlType ActiveDrag => activeDrag;
        public MouseEventArgs LatestMouseEvent => latestMouseEvent;

        public ObjectMoveControl(ObjectControl objectControl)
        {
            InitializeComponent();

            this.ObjectControl = objectControl;

            EnableAll(false);
            UpdatePictureBoxImages();
            comboBoxMoveMode.MouseWheel += ComboBoxMoveMode_MouseWheel;
            trackBarMoveSpeed.MouseWheel += TrackBarMoveSpeed_MouseWheel;
            comboBoxMoveMode_IsChangeable = false;
            comboBoxMoveMode.Items.Add(new MoveObjTypeObjForListBox(MoveObjType.Null, ""));
            comboBoxMoveMode.SelectedIndex = 0;
        }

        public void UpdateSelection()
        {
            var availableModes = ObjectControl.GetAvailableMoveModes();
            var selectedMode = ObjectControl.MoveObjTypeSelected;

            comboBoxMoveMode_IsChangeable = false;
            comboBoxMoveMode.Items.Clear();
            comboBoxMoveMode.Items.AddRange(availableModes.ToArray());

            int indexToSelect = availableModes.FindIndex(item => item.ID == selectedMode);
            comboBoxMoveMode.SelectedIndex = (indexToSelect != -1) ? indexToSelect : 0;
            comboBoxMoveMode_IsChangeable = true;

            if (comboBoxMoveMode.SelectedItem != null)
            {
                comboBoxMoveMode_SelectedIndexChanged(comboBoxMoveMode, EventArgs.Empty);
            }
        }

        private void EnableAll(bool enableAll)
        {
            this.Enabled = enableAll;
            comboBoxMoveMode.Enabled = enableAll;
            buttonDropToGround.Enabled = enableAll;
            checkBoxObjKeepOnGround.Enabled = enableAll;
            checkBoxLockMoveSquareHorizontal.Enabled = enableAll;
            checkBoxLockMoveSquareVertical.Enabled = enableAll;
            checkBoxMoveRelativeCam.Enabled = enableAll;
            checkBoxTriggerZoneKeepOnGround.Enabled = enableAll;
            trackBarMoveSpeed.Enabled = enableAll;

            moveObjHorizontal1.Enabled = ObjectControl.EnableHorizontal1 && enableAll;
            moveObjHorizontal2.Enabled = ObjectControl.EnableHorizontal2 && enableAll;
            moveObjHorizontal3.Enabled = ObjectControl.EnableHorizontal3 && enableAll;
            moveObjVertical.Enabled = ObjectControl.EnableVertical && enableAll;
            moveObjSquare.Enabled = ObjectControl.EnableSquare && enableAll;
        }

        private void UpdatePictureBoxImages()
        {
            if (ObjectControl.EnableHorizontal1)
            {
                moveObjHorizontal1.BackgroundImage = Properties.Resources.HorizontalYelow;
            }
            else
            {
                moveObjHorizontal1.BackgroundImage = Properties.Resources.HorizontalDisable;
            }
            if (ObjectControl.EnableHorizontal2)
            {
                moveObjHorizontal2.BackgroundImage = Properties.Resources.HorizontalYelow;
            }
            else
            {
                moveObjHorizontal2.BackgroundImage = Properties.Resources.HorizontalDisable;
            }
            if (ObjectControl.EnableHorizontal3)
            {
                moveObjHorizontal3.BackgroundImage = Properties.Resources.HorizontalYelow;
            }
            else
            {
                moveObjHorizontal3.BackgroundImage = Properties.Resources.HorizontalDisable;
            }

            if (ObjectControl.EnableVertical)
            {
                moveObjVertical.BackgroundImage = Properties.Resources.VerticalRed;
            }
            else
            {
                moveObjVertical.BackgroundImage = Properties.Resources.VerticalDisable;
            }

            if (ObjectControl.EnableSquare)
            {
                if (MoveObj.LockMoveSquareVertical)
                {
                    moveObjSquare.BackgroundImage = Properties.Resources.SquareRedLookVertical;
                }
                else if (MoveObj.LockMoveSquareHorisontal)
                {
                    moveObjSquare.BackgroundImage = Properties.Resources.SquareRedLookHorisontal;
                }
                else
                {
                    moveObjSquare.BackgroundImage = Properties.Resources.SquareRed;
                }
            }
            else
            {
                moveObjSquare.BackgroundImage = Properties.Resources.SquareDisable;
            }
        }

        private void TrackBarMoveSpeed_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            if (e.X >= 0 && e.Y >= 0 && e.X < trackBarMoveSpeed.Width && e.Y < trackBarMoveSpeed.Height)
            {
                ((HandledMouseEventArgs)e).Handled = false;
            }
        }

        private void ComboBoxMoveMode_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = !((ComboBox)sender).DroppedDown;
        }

        private void ObjectMoveControl_Resize(object sender, EventArgs e)
        {
            int width = this.Width - comboBoxMoveMode.Location.X;
            if (width > 800)
            {
                width = 800;
            }
            comboBoxMoveMode.Size = new Size(width, comboBoxMoveMode.Size.Height);
        }

        private void trackBarMoveSpeed_Scroll(object sender, EventArgs e)
        {
            labelObjSpeed.Text = ObjectControl.SetMoveSpeed(trackBarMoveSpeed.Value);
        }

        private void comboBoxMoveMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMoveMode_IsChangeable)
            {
                if (comboBoxMoveMode.SelectedItem is MoveObjTypeObjForListBox obj)
                {
                    ObjectControl.SetSelectedMoveMode(obj.ID);
                    EnableAll(obj.ID != MoveObjType.Null);
                    UpdatePictureBoxImages();
                }
                else
                {
                    ObjectControl.SetSelectedMoveMode(MoveObjType.Null);
                    EnableAll(false);
                    UpdatePictureBoxImages();
                }
            }
        }

        private void buttonDropToGround_Click(object sender, EventArgs e)
        {
            ObjectControl.DropToGround();
        }

        private void checkBoxKeepOnGround_CheckedChanged(object sender, EventArgs e)
        {
            ObjectControl.SetObjKeepOnGround(checkBoxObjKeepOnGround.Checked);
        }

        private void checkBoxTriggerZoneKeepOnGround_CheckedChanged(object sender, EventArgs e)
        {
            ObjectControl.SetTriggerZoneKeepOnGround(checkBoxTriggerZoneKeepOnGround.Checked);
        }

        private void checkBoxMoveRelativeCam_CheckedChanged(object sender, EventArgs e)
        {
            ObjectControl.SetMoveRelativeCamera(checkBoxMoveRelativeCam.Checked);
        }

        private void checkBoxLockMoveSquareHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLockMoveSquareHorizontal_IsChangeable)
            {
                checkBoxLockMoveSquareVertical_IsChangeable = false;
                checkBoxLockMoveSquareVertical.Checked = false;
                ObjectControl.SetLockMoveSquare(checkBoxLockMoveSquareHorizontal.Checked, false);
                UpdatePictureBoxImages();
                checkBoxLockMoveSquareVertical_IsChangeable = true;
            }
        }

        private void checkBoxLockMoveSquareVertical_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLockMoveSquareVertical_IsChangeable)
            {
                checkBoxLockMoveSquareHorizontal_IsChangeable = false;
                checkBoxLockMoveSquareHorizontal.Checked = false;
                ObjectControl.SetLockMoveSquare(false, checkBoxLockMoveSquareVertical.Checked);
                UpdatePictureBoxImages();
                checkBoxLockMoveSquareHorizontal_IsChangeable = true;
            }
        }

        private void moveObjSquare_MouseDown(object sender, MouseEventArgs e)
        {
            if (ObjectControl.EnableSquare)
            {
                activeDrag = MoveControlType.Square;
                ObjectControl.StartMove(MoveControlType.Square, e.Location, e.Button);
            }
        }

        private void moveObjSquare_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Square)
            {
                latestMouseEvent = e;
            }
        }

        private void moveObjSquare_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Square)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjSquare_MouseLeave(object sender, EventArgs e)
        {
            if (activeDrag == MoveControlType.Square)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjVertical_MouseDown(object sender, MouseEventArgs e)
        {
            if (ObjectControl.EnableVertical)
            {
                activeDrag = MoveControlType.Vertical;
                ObjectControl.StartMove(MoveControlType.Vertical, e.Location, e.Button);
            }
        }

        private void moveObjVertical_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Vertical)
            {
                latestMouseEvent = e;
            }
        }

        private void moveObjVertical_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Vertical)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjVertical_MouseLeave(object sender, EventArgs e)
        {
            if (activeDrag == MoveControlType.Vertical)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjHorizontal1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ObjectControl.EnableHorizontal1)
            {
                activeDrag = MoveControlType.Horizontal1;
                ObjectControl.StartMove(MoveControlType.Horizontal1, e.Location, e.Button);
            }
        }

        private void moveObjHorizontal1_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal1)
            {
                latestMouseEvent = e;
            }
        }

        private void moveObjHorizontal1_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal1)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjHorizontal1_MouseLeave(object sender, EventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal1)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjHorizontal2_MouseDown(object sender, MouseEventArgs e)
        {
            if (ObjectControl.EnableHorizontal2)
            {
                activeDrag = MoveControlType.Horizontal2;
                ObjectControl.StartMove(MoveControlType.Horizontal2, e.Location, e.Button);
            }
        }

        private void moveObjHorizontal2_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal2)
            {
                latestMouseEvent = e;
            }
        }

        private void moveObjHorizontal2_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal2)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjHorizontal2_MouseLeave(object sender, EventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal2)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjHorizontal3_MouseDown(object sender, MouseEventArgs e)
        {
            if (ObjectControl.EnableHorizontal3)
            {
                activeDrag = MoveControlType.Horizontal3;
                ObjectControl.StartMove(MoveControlType.Horizontal3, e.Location, e.Button);
            }
        }

        private void moveObjHorizontal3_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal3)
            {
                latestMouseEvent = e;
            }
        }

        private void moveObjHorizontal3_MouseUp(object sender, MouseEventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal3)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        private void moveObjHorizontal3_MouseLeave(object sender, EventArgs e)
        {
            if (activeDrag == MoveControlType.Horizontal3)
            {
                activeDrag = MoveControlType.Null;
                ObjectControl.EndMove();
            }
        }

        public void StartUpdateTranslation()
        {
            labelObjSpeed.Text = Lang.GetText(eLang.labelObjSpeed) + " 100%";
            buttonDropToGround.Text = Lang.GetText(eLang.buttonDropToGround);
            checkBoxObjKeepOnGround.Text = Lang.GetText(eLang.checkBoxObjKeepOnGround);
            checkBoxTriggerZoneKeepOnGround.Text = Lang.GetText(eLang.checkBoxTriggerZoneKeepOnGround);
            checkBoxLockMoveSquareHorizontal.Text = Lang.GetText(eLang.checkBoxLockMoveSquareHorizontal);
            checkBoxLockMoveSquareVertical.Text = Lang.GetText(eLang.checkBoxLockMoveSquareVertical);
            checkBoxMoveRelativeCam.Text = Lang.GetText(eLang.checkBoxMoveRelativeCam);
        }
    }
}