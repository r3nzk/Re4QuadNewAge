using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NsCamera;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Re4QuadExtremeEditor.Editor;
using Re4QuadExtremeEditor.Editor.Class;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using Re4QuadExtremeEditor.Editor.Class.Files;
using Re4QuadExtremeEditor.Editor.Class.MyProperty;
using Re4QuadExtremeEditor.Editor.Class.MyProperty._EFF_Property;
using Re4QuadExtremeEditor.Editor.Class.MyProperty.CustomAttribute;
using Re4QuadExtremeEditor.Editor.Class.ObjMethods;
using Re4QuadExtremeEditor.Editor.Class.Shaders;
using Re4QuadExtremeEditor.Editor.Class.TreeNodeObj;
using Re4QuadExtremeEditor.Editor.Controls;
using Re4QuadExtremeEditor.Editor.forms;
using Re4QuadExtremeEditor.Editor.Forms;
using ReaLTaiizor.Controls;
using SimpleEndianBinaryIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;
using ViewerBase;
using static OpenTK.Graphics.OpenGL.GL;


namespace Re4QuadExtremeEditor
{
    public partial class MainForm : Form
    {
        public static MainForm instance { get; private set; }

        readonly System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        GLControl glControl;
        Gizmo gizmo;
        ObjectControl objectControl;
        CameraControl cameraControl;

        //legacy gl controls
        CameraMoveControl cameraMove;
        ObjectMoveControl objectMove;

        //for launch options detection/focus
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        //searchbar placeholder
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        #region Camera // variaveis para a camera
        Camera camera = new Camera();
        Matrix4 camMtx = Matrix4.Identity;
        Matrix4 ProjMatrix;
        // movimentação da camera
        bool isShiftDown = false, isControlDown = false, isQDown = false, isEDown = false;
        bool isMouseDown = false, isMouseMove = false;
        bool isWDown = false, isSDown = false, isADown = false, isDDown = false;
        //gizmo fields
        private bool isDraggingGizmo = false;
        private Gizmo.GizmoAxis draggedGizmoAxis = Gizmo.GizmoAxis.None;
        private Point lastMousePosition;
        //movimentação camera no glControl
        MouseButtons MouseButtonsRight = MouseButtons.Right; //botão para movimentação camera
        MouseButtons MouseButtonsLeft = MouseButtons.Left; // botão para selecionar objeto
        //dictionaries to store initial state of objects when a drag starts
        private Dictionary<int, Vector3> initialDragPositions = new Dictionary<int, Vector3>();
        private Dictionary<int, Vector3> initialDragRotations = new Dictionary<int, Vector3>();
        #endregion

        // Property que fica no PropertyGrid quando não tem nada selecionado;
        readonly NoneProperty none = new NoneProperty();

        // define se esta com o PropertyGrid selecionado;
        bool InPropertyGrid = false;

        //filtering suite
        private object propertyGridOriginalObj;
        private readonly Dictionary<TreeNode, (TreeNode Parent, int Index)> _hiddenNodes = new Dictionary<TreeNode, (TreeNode, int)>();

        //project
        private string currentProjectPath = null;

        UpdateMethods updateMethods;

        public MainForm()
        {
            instance = this;

            InitializeComponent();
            propertyGridObjs.SelectedItemWithFocusBackColor = Color.FromArgb(0x70, 0xBB, 0xDB);
            propertyGridObjs.SelectedItemWithFocusForeColor = Color.Black;
            treeViewObjs.SelectedNodeBackColor = Color.FromArgb(0x70, 0xBB, 0xDB);
            treeViewObjs.Font = Globals.TreeNodeFontText;


            SetObjectToPropertyGrid(none);
            DataBase.SelectedNodes = treeViewObjs.SelectedNodes;

            //viewport actions setup
            glControl = new OpenTK.GLControl();
            glControl.Dock = DockStyle.Fill;
            glControl.Name = "glControl";
            glControl.TabIndex = 999;
            glControl.TabStop = false;
            glControl.Paint += GlControl_Paint;
            glControl.Load += GlControl_Load;
            glControl.KeyDown += GlControl_KeyDown;
            glControl.KeyUp += GlControl_KeyUp;
            glControl.Leave += GlControl_Leave;
            glControl.MouseWheel += GlControl_MouseWheel;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseLeave += GlControl_MouseLeave;
            glControl.Resize += GlControl_Resize;
            glViewport.Controls.Add(glControl);

            camera.getSelectedObject = getSelectedObject;

            //setup session
            buildTreeView();
            UpdateTreeViewNodes();
            currentRoomLabelToggle(false);

            // new controls
            objectControl = new ObjectControl(ref camera, UpdateGL, UpdateCameraMatrix, UpdatePropertyGrid, UpdateTreeViewObjs);
            cameraControl = new CameraControl(ref camera, UpdateGL, UpdateProjectionMatrix);

            // setup old controls
            objectMove = new ObjectMoveControl(objectControl);
            objectMove.Location = new Point(0, 0);
            objectMove.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            objectMove.Name = "objectMove";
            objectMove.TabIndex = 995;
            objectMove.TabStop = false;
            glControls_old.Controls.Add(objectMove);

            cameraMove = new CameraMoveControl(cameraControl, objectControl); //assigning object controll too to keep legacy controls exactly as they were
            cameraMove.Location = new Point(objectMove.Right, objectMove.Top);
            cameraMove.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            cameraMove.Name = "cameraMove";
            cameraMove.TabIndex = 998;
            cameraMove.TabStop = false;
            glControls_old.Controls.Add(cameraMove);


            //setup console
            Editor.Console.RegisterOutputControl(this.consoleBox);

            //editor tools
            selectTool(0); //set start tool (move)
            selectGizmospace(true); // set localspace as default

            //input
            KeyPreview = true;
            myTimer.Tick += updateWASDControls;
            myTimer.Interval = 10;
            myTimer.Enabled = false;

            camMtx = camera.GetViewMatrix();
            ProjMatrix = ReturnNewProjMatrix();

            updateMethods = new UpdateMethods();
            updateMethods.UpdateGL = UpdateGL;
            updateMethods.UpdatePropertyGrid = UpdatePropertyGrid;
            updateMethods.UpdateTreeViewObjs = UpdateTreeViewObjs;
            updateMethods.UpdateMoveObjSelection = objectMove.UpdateSelection;
            updateMethods.UpdateOrbitCamera = UpdateOrbitCamera;

            Globals.updateMethods = updateMethods;

            //add searchbars placeholders
            SetPlaceholder(treeView_searchField, "Filter: name, t:type, g:group");
            SetPlaceholder(propertyGrid_searchField, "Filter: b:byte, o:offset, name");

            //assign re4 preferred ver into toolstrip dropdown text
            PopulatePreferredVerDropdown();

            GenerateViewportUtility();

            //property grid sorting
            propertyGridButton_Categorized.Checked = true; // categorized always default
            propertyGridButton_ShowAll.Checked = Globals.PropertyGridShowAllByDefault;
            UpdatePropertyGridDisplay();

            ApplyTheme();

            //project setup
            UpdateFormTitle();

            if (Globals.BackupConfigs.UseInvertedMouseButtons)
            {
                MouseButtonsRight = MouseButtons.Left;
                MouseButtonsLeft = MouseButtons.Right;
            }



            Editor.Console.Log("Finished setup, you are running RE4QuadX ver 1.0");
        }

        private void UpdateFormTitle()
        {
            //project based title form
            /*if (string.IsNullOrEmpty(currentProjectPath))
            {
                this.Text = "New Project - QuadX";
            }else
            {
                this.Text = Path.GetFileName(currentProjectPath) + " - QuadX";
            }*/


            if (DataBase.SelectedRoom != null)
            {
                var roomId = DataBase.SelectedRoom.GetRoomId().ToString("X4");
                roomId = roomId.Substring(1);
                this.Text = $"r{roomId} - RE4QuadX";
            }
            else
            {
                this.Text = "RE4QuadX";
            }
        }

        public void SelectNode(TreeNode nodeToSelect)
        {
            treeViewObjs.ToSelectSingleNode(nodeToSelect);
            UpdateGL();
        }

        private void ResetEditorState()
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to create a new QuadX Project? All unsaved progress will be lost.",
                "Create New Project?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms (clicks Yes), proceed with the reset.
            if (result == DialogResult.Yes)
            {
                // clear current room
                if (DataBase.SelectedRoom != null)
                {
                    DataBase.SelectedRoom.ClearGL();
                    DataBase.SelectedRoom = null;
                    currentRoomLabel.Visible = false;
                    currentRoomLabel.Text = "";
                }

                //clear the current selection and PropertyGrid to ensure a clean state.
                TreeViewUpdateSelectedsClear();
                UpdateTreeViewObjs();
                UpdatePropertyGrid();
                UpdateTreeViewNodes();

                //clear all files loaded
                clearAllObjects();

                UpdateGL();

                cameraMove.ResetCamera();

                Editor.Console.Clear();
                Editor.Console.Log("New QuadX Environment created.");
            }
        }

        private void clearAllObjects()
        {
            FileManager.ClearESL();
            FileManager.ClearETS();
            FileManager.ClearITA();
            FileManager.ClearAEV();
            FileManager.ClearDSE();
            FileManager.ClearFSE();
            FileManager.ClearSAR();
            FileManager.ClearEAR();
            FileManager.ClearEMI();
            FileManager.ClearESE();
            FileManager.ClearLIT();
            FileManager.ClearEFFBLOB();
            FileManager.ClearQuadCustom();
        }

        private void buildTreeView()
        {
            treeViewObjs.Nodes.Add(DataBase.NodeESL);
            treeViewObjs.Nodes.Add(DataBase.NodeETS);
            treeViewObjs.Nodes.Add(DataBase.NodeITA);
            treeViewObjs.Nodes.Add(DataBase.NodeAEV);
            treeViewObjs.Nodes.Add(DataBase.NodeEXTRAS);
            treeViewObjs.Nodes.Add(DataBase.NodeDSE);
            treeViewObjs.Nodes.Add(DataBase.NodeFSE);
            treeViewObjs.Nodes.Add(DataBase.NodeEAR);
            treeViewObjs.Nodes.Add(DataBase.NodeSAR);
            treeViewObjs.Nodes.Add(DataBase.NodeEMI);
            treeViewObjs.Nodes.Add(DataBase.NodeESE);
            treeViewObjs.Nodes.Add(DataBase.NodeQuadCustom);
            treeViewObjs.Nodes.Add(DataBase.NodeLIT_Groups);
            treeViewObjs.Nodes.Add(DataBase.NodeLIT_Entrys);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table0);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table1);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table2);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table3);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table4);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table6);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table7_Effect_0);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table8_Effect_1);
            treeViewObjs.Nodes.Add(DataBase.NodeEFF_Table9);
        }

        public void UpdateTreeViewNodes()
        {
            List<TreeNode> allRootNodes = new List<TreeNode>
            {
                DataBase.NodeESL,
                DataBase.NodeETS,
                DataBase.NodeITA,
                DataBase.NodeAEV,
                DataBase.NodeEXTRAS,
                DataBase.NodeDSE,
                DataBase.NodeFSE,
                DataBase.NodeEAR,
                DataBase.NodeSAR,
                DataBase.NodeEMI,
                DataBase.NodeESE,
                DataBase.NodeQuadCustom,
                DataBase.NodeLIT_Groups,
                DataBase.NodeLIT_Entrys,
                DataBase.NodeEFF_Table0,
                DataBase.NodeEFF_Table1,
                DataBase.NodeEFF_Table2,
                DataBase.NodeEFF_Table3,
                DataBase.NodeEFF_Table4,
                DataBase.NodeEFF_Table6,
                DataBase.NodeEFF_Table7_Effect_0,
                DataBase.NodeEFF_Table8_Effect_1,
                DataBase.NodeEFF_Table9
            };

            treeViewObjs.BeginUpdate();
            treeViewObjs.Nodes.Clear();

            foreach (TreeNode rootNode in allRootNodes){
                if (rootNode == null) continue;

                if (!Globals.TreeViewHideEmptyRoot || rootNode.Nodes.Count > 0)
                {
                    treeViewObjs.Nodes.Add(rootNode);
                }
            }

            treeViewObjs.EndUpdate();
        }

        private void currentRoomLabelToggle(bool state = true, string newText = "")
        {
            currentRoomLabel.Visible = state;
            currentRoomLabel.Text = newText;
            UpdateFormTitle();
        }

        #region GlControl Events

        private Matrix4 ReturnNewProjMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(Globals.FOV * ((float)Math.PI / 180.0f), (float)glControl.Width / (float)glControl.Height, 0.01f, 1_000_000f);
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            glControl.Context.Update(glControl.WindowInfo);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            ProjMatrix = ReturnNewProjMatrix();
            glControl.Invalidate();
        }

        private void splitContainerMain_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            glControl.Invalidate();
        }

        private void GlControl_MouseLeave(object sender, EventArgs e)
        {
            camera.resetMouseStuff();
            isMouseDown = false;
            isMouseMove = false;
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtonsLeft && isDraggingGizmo)
            {
                isDraggingGizmo = false;
                isMouseDown = false;
                isMouseMove = false;
                objectControl.EndMove();
            }

            if (e.Button == MouseButtonsRight)
            {
                camera.resetMouseStuff();
                isMouseDown = false;
                isMouseMove = false;

                //cursor behaviour
                Cursor.Show();
                CenterCursorInViewport();

                camera.SaveCameraPosition();
            }

            if (!isWDown && !isSDown && !isADown && !isDDown && !isMouseMove && !isShiftDown && !isQDown && !isEDown)
            {
                myTimer.Enabled = false;
            }
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            glControl.Focus();

            if (e.Button == MouseButtonsRight)
            {
                camera.resetMouseStuff();
                camera.SaveCameraPosition();

                //input
                isMouseDown = true;
                isMouseMove = true;
                myTimer.Enabled = true;

                //cursor
                Cursor.Hide();
                CenterCursorInViewport();
            }
            else if (e.Button == MouseButtonsLeft)
            {
                selectObject(e.X, e.Y);

                if (isDraggingGizmo) {
                    isMouseDown = true;
                    isMouseMove = true;
                    myTimer.Enabled = true;
                }
                glControl.Invalidate();
            }
        }

        private void CenterCursorInViewport()
        {
            Point localCenter = new Point(glControl.Width / 2, glControl.Height / 2);
            Point screenCenter = glControl.PointToScreen(localCenter);

            Cursor.Position = screenCenter;
        }

        /// <summary>
        /// metodo destinado para a seleção dos objetos no ambiente GL
        /// </summary>
        private void selectObject(int mx, int my)
        {
            NewAgeTheRender.TheRender.AllRender(ref camMtx, ref ProjMatrix, camera.Position, camera.SelectedObjPosY(), gizmo, camera, Globals.CurrentTool, Globals.CurrentGizmoSpace, true); // renderiza o ambiente GL no modo seleção.

            int h = glControl.Height;
            byte[] pixel = new byte[4];
            GL.ReadPixels(mx, h - my, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, pixel);

            //Editor.Console.Log($"Pixel read: R={pixel[0]}, G={pixel[1]}, B={pixel[2]}"); //debug selected object read

            if (pixel[0] == 255 && DataBase.SelectedNodes.Count > 0)
            {
                isDraggingGizmo = true;
                draggedGizmoAxis = (Gizmo.GizmoAxis)(pixel[1]); //g channel (1, 2, or 3) determines the axis.
                lastMousePosition = new Point(mx, my);

                objectControl.StartMove(MoveControlType.Square, lastMousePosition, MouseButtons.Left);
                return;
            }

            //Console.WriteLine("pixel[0]: " + pixel[0]); // lineID
            //Console.WriteLine("pixel[1]: " + pixel[1]); // lineID
            //Console.WriteLine("pixel[2]: " + pixel[2]); // id da lista
            //Console.WriteLine("pixel[3]: " + pixel[3]);

            // listas
            // aviso: proibido usar os valores 0 e 255, pois fazem parte das cores preta (renderização do cenario) e da cor branca (fundo);
            if (pixel[2] > 0 && pixel[2] < 255)
            {
                ushort LineID = BitConverter.ToUInt16(pixel, 0);

                TreeNode selected = null;
                switch (pixel[2])
                {
                    case (byte)GroupType.ESL:
                        int index1 = DataBase.NodeESL.Nodes.IndexOfKey(LineID.ToString());
                        if (index1 > -1) { selected = DataBase.NodeESL.Nodes[index1]; }
                        break;
                    case (byte)GroupType.ETS:
                        int index2 = DataBase.NodeETS.Nodes.IndexOfKey(LineID.ToString());
                        if (index2 > -1) { selected = DataBase.NodeETS.Nodes[index2]; }
                        break;
                    case (byte)GroupType.ITA:
                        int index3 = DataBase.NodeITA.Nodes.IndexOfKey(LineID.ToString());
                        if (index3 > -1) { selected = DataBase.NodeITA.Nodes[index3]; }
                        break;
                    case (byte)GroupType.AEV:
                        int index4 = DataBase.NodeAEV.Nodes.IndexOfKey(LineID.ToString());
                        if (index4 > -1) { selected = DataBase.NodeAEV.Nodes[index4]; }
                        break;
                    case (byte)GroupType.EXTRAS:
                        int index5 = DataBase.NodeEXTRAS.Nodes.IndexOfKey(LineID.ToString());
                        if (index5 > -1) { selected = DataBase.NodeEXTRAS.Nodes[index5]; }
                        break;
                    case (byte)GroupType.EAR:
                        int index6 = DataBase.NodeEAR.Nodes.IndexOfKey(LineID.ToString());
                        if (index6 > -1) { selected = DataBase.NodeEAR.Nodes[index6]; }
                        break;
                    case (byte)GroupType.SAR:
                        int index7 = DataBase.NodeSAR.Nodes.IndexOfKey(LineID.ToString());
                        if (index7 > -1) { selected = DataBase.NodeSAR.Nodes[index7]; }
                        break;
                    case (byte)GroupType.EMI:
                        int index8 = DataBase.NodeEMI.Nodes.IndexOfKey(LineID.ToString());
                        if (index8 > -1) { selected = DataBase.NodeEMI.Nodes[index8]; }
                        break;
                    case (byte)GroupType.ESE:
                        int index9 = DataBase.NodeESE.Nodes.IndexOfKey(LineID.ToString());
                        if (index9 > -1) { selected = DataBase.NodeESE.Nodes[index9]; }
                        break;
                    case (byte)GroupType.FSE:
                        int index10 = DataBase.NodeFSE.Nodes.IndexOfKey(LineID.ToString());
                        if (index10 > -1) { selected = DataBase.NodeFSE.Nodes[index10]; }
                        break;
                    case (byte)GroupType.QUAD_CUSTOM:
                        int index11 = DataBase.NodeQuadCustom.Nodes.IndexOfKey(LineID.ToString());
                        if (index11 > -1) { selected = DataBase.NodeQuadCustom.Nodes[index11]; }
                        break;
                    case (byte)GroupType.LIT_ENTRYS:
                        int index12 = DataBase.NodeLIT_Entrys.Nodes.IndexOfKey(LineID.ToString());
                        if (index12 > -1) { selected = DataBase.NodeLIT_Entrys.Nodes[index12]; }
                        break;
                    case (byte)GroupType.EFF_EffectEntry:
                        int index13 = DataBase.NodeEFF_EffectEntry.Nodes.IndexOfKey(LineID.ToString());
                        if (index13 > -1) { selected = DataBase.NodeEFF_EffectEntry.Nodes[index13]; }
                        break;
                    case (byte)GroupType.EFF_Table7_Effect_0:
                        int index14 = DataBase.NodeEFF_Table7_Effect_0.Nodes.IndexOfKey(LineID.ToString());
                        if (index14 > -1) { selected = DataBase.NodeEFF_Table7_Effect_0.Nodes[index14]; }
                        break;
                    case (byte)GroupType.EFF_Table8_Effect_1:
                        int index15 = DataBase.NodeEFF_Table8_Effect_1.Nodes.IndexOfKey(LineID.ToString());
                        if (index15 > -1) { selected = DataBase.NodeEFF_Table8_Effect_1.Nodes[index15]; }
                        break;
                    case (byte)GroupType.EFF_Table9:
                        int index16 = DataBase.NodeEFF_Table9.Nodes.IndexOfKey(LineID.ToString());
                        if (index16 > -1) { selected = DataBase.NodeEFF_Table9.Nodes[index16]; }
                        break;
                }

                if (selected != null)
                {
                    if (isControlDown) // add ou remove da seleção
                    {
                        treeViewObjs.ToSelectMultiNode(selected);
                    }
                    else // seleciona so esse
                    {
                        treeViewObjs.ToSelectSingleNode(selected);
                    }

                }
            }
        }

        private void MoveGizmo(MouseEventArgs e)
        {
            if (DataBase.SelectedNodes.Count == 0) return;

            //calculate drag
            Point currentMousePosition = e.Location;
            Vector2 mouseDelta = new Vector2(currentMousePosition.X - lastMousePosition.X, currentMousePosition.Y - lastMousePosition.Y);
            lastMousePosition = currentMousePosition;

            if (mouseDelta.LengthSquared < 0.001f) return; //invalid drag factor

            //get gizmospace
            Matrix4 objectRotation = Matrix4.Identity;
            if (Globals.CurrentGizmoSpace == GizmoSpace.Local && DataBase.LastSelectNode is Object3D obj){
                objectRotation = obj.GetRotationMatrix();
            }

            var gizmoPos = gizmo.Position;

            //1D drag (translate and rotate)
            if (draggedGizmoAxis == Gizmo.GizmoAxis.X || draggedGizmoAxis == Gizmo.GizmoAxis.Y || draggedGizmoAxis == Gizmo.GizmoAxis.Z){
                Vector3 axisVector = Vector3.Zero;
                switch (draggedGizmoAxis)
                {
                    case Gizmo.GizmoAxis.X: axisVector = Vector3.UnitX; break;
                    case Gizmo.GizmoAxis.Y: axisVector = Vector3.UnitY; break;
                    case Gizmo.GizmoAxis.Z: axisVector = Vector3.UnitZ; break;
                }

                axisVector = Vector3.TransformVector(axisVector, objectRotation);

                var gizmoPos_screen_nullable = Camera.Project(gizmoPos, ProjMatrix, camMtx, glControl.ClientRectangle);
                var axisEnd_screen_nullable = Camera.Project(gizmoPos + axisVector, ProjMatrix, camMtx, glControl.ClientRectangle);

                if (!gizmoPos_screen_nullable.HasValue || !axisEnd_screen_nullable.HasValue) return;

                var gizmoPos_screen = gizmoPos_screen_nullable.Value;
                var axisEnd_screen = axisEnd_screen_nullable.Value;

                Vector2 axisOnScreen = (axisEnd_screen.Xy - gizmoPos_screen.Xy);
                if (axisOnScreen.LengthSquared < 0.001f) return;
                axisOnScreen.Normalize();

                float movementMagnitude = Vector2.Dot(mouseDelta, axisOnScreen);
                float distance = (camera.Position - gizmoPos).Length;
                float scaleFactor = distance * 0.002f;
                movementMagnitude *= scaleFactor;

                if (Globals.CurrentTool == EditorTool.Move)
                {
                    Vector3 translation = axisVector * movementMagnitude;
                    objectControl.ApplyGizmoTranslation(translation);
                }
                else if (Globals.CurrentTool == EditorTool.Rotate)
                {
                    float angleDeg = movementMagnitude * 2f;
                    objectControl.ApplyGizmoRotation(angleDeg, draggedGizmoAxis);
                }
            }
            //2D drag (translate only)
            else if (Globals.CurrentTool == EditorTool.Move && (draggedGizmoAxis == Gizmo.GizmoAxis.XY || draggedGizmoAxis == Gizmo.GizmoAxis.YZ || draggedGizmoAxis == Gizmo.GizmoAxis.XZ)){
                Vector3 axis1 = Vector3.Zero, axis2 = Vector3.Zero;
                switch (draggedGizmoAxis)
                {
                    case Gizmo.GizmoAxis.XY: axis1 = Vector3.UnitX; axis2 = Vector3.UnitY; break;
                    case Gizmo.GizmoAxis.YZ: axis1 = Vector3.UnitY; axis2 = Vector3.UnitZ; break;
                    case Gizmo.GizmoAxis.XZ: axis1 = Vector3.UnitX; axis2 = Vector3.UnitZ; break;
                }

                axis1 = Vector3.TransformVector(axis1, objectRotation);
                axis2 = Vector3.TransformVector(axis2, objectRotation);

                var gizmoPos_screen_nullable = Camera.Project(gizmoPos, ProjMatrix, camMtx, glControl.ClientRectangle);
                var axis1_end_screen_nullable = Camera.Project(gizmoPos + axis1, ProjMatrix, camMtx, glControl.ClientRectangle);
                var axis2_end_screen_nullable = Camera.Project(gizmoPos + axis2, ProjMatrix, camMtx, glControl.ClientRectangle);

                if (!gizmoPos_screen_nullable.HasValue || !axis1_end_screen_nullable.HasValue || !axis2_end_screen_nullable.HasValue) return;

                var gizmoPos_screen = gizmoPos_screen_nullable.Value.Xy;
                Vector2 axis1_screen = (axis1_end_screen_nullable.Value.Xy - gizmoPos_screen);
                Vector2 axis2_screen = (axis2_end_screen_nullable.Value.Xy - gizmoPos_screen);

                if (axis1_screen.LengthSquared < 0.001f || axis2_screen.LengthSquared < 0.001f) return;

                axis1_screen.Normalize(); axis2_screen.Normalize();

                float move1 = Vector2.Dot(mouseDelta, axis1_screen);
                float move2 = Vector2.Dot(mouseDelta, axis2_screen);

                float distance = (camera.Position - gizmoPos).Length;
                float scaleFactor = distance * 0.002f;
                move1 *= scaleFactor;
                move2 *= scaleFactor;

                Vector3 translation = (axis1 * move1) + (axis2 * move2);
                objectControl.ApplyGizmoTranslation(translation);
            }
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingGizmo) {
                MoveGizmo(e);
            } else if (isMouseDown && e.Button == MouseButtonsRight) {
                camera.updateCameraOffsetMatrixWithMouse(isControlDown, e.X, e.Y);
                camMtx = camera.GetViewMatrix();
            }
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.resetMouseStuff();
            camera.updateCameraMatrixWithScrollWheel((int)(e.Delta * 0.5f));
            camMtx = camera.GetViewMatrix();
            camera.SaveCameraPosition();
            glControl.Invalidate();
        }

        private void GlControl_Leave(object sender, EventArgs e)
        {
            isWDown = false;
            isSDown = false;
            isADown = false;
            isDDown = false;
            isQDown = false;
            isEDown = false;
            isShiftDown = false;
            isControlDown = false;
            isMouseDown = false;
            isMouseMove = false;
            myTimer.Enabled = false;
        }

        private void GlControl_KeyUp(object sender, KeyEventArgs e)
        {
            isShiftDown = e.Shift;
            isControlDown = e.Control;
            switch (e.KeyCode)
            {
                case Keys.W: isWDown = false; break;
                case Keys.S: isSDown = false; break;
                case Keys.A: isADown = false; break;
                case Keys.D: isDDown = false; break;
                case Keys.Q: isQDown = false; break;
                case Keys.E: isEDown = false; break;
            }
            if (!isWDown && !isSDown && !isADown && !isDDown && !isMouseMove && !isShiftDown && !isEDown && !isQDown)
            {
                myTimer.Enabled = false;
            }
            if (isControlDown)
            {
                camera.SaveCameraPosition();
                camera.resetMouseStuff();
            }
        }

        private void GlControl_KeyDown(object sender, KeyEventArgs e)
        {
            isShiftDown = e.Shift;
            isControlDown = e.Control;
            switch (e.KeyCode)
            {
                case Keys.W:
                    isWDown = true;
                    myTimer.Enabled = true;
                    break;
                case Keys.S:
                    isSDown = true;
                    myTimer.Enabled = true;
                    break;
                case Keys.A:
                    isADown = true;
                    myTimer.Enabled = true;
                    break;
                case Keys.D:
                    isDDown = true;
                    myTimer.Enabled = true;
                    break;
                case Keys.Q:
                    isQDown = true;
                    myTimer.Enabled = true;
                    break;
                case Keys.E:
                    isEDown = true;
                    myTimer.Enabled = true;
                    break;
                case Keys.F:
                    if (camera.CamMode == Camera.CameraMode.ORBIT) break;

                    cameraControl.SetCameraMode(1); //switch to orbit
                    camera.ResetOrbitToSelectedObject();
                    camMtx = camera.GetViewMatrix();
                    cameraControl.SetCameraMode(0); //switch back to fly
                    glControl.Invalidate();
                    break;
            }
            if (isShiftDown)
            {
                myTimer.Enabled = true;
            }
            if (isControlDown)
            {
                camera.SaveCameraPosition();
                camera.resetMouseStuff();
            }

        }

        /// <summary>
        /// Atualiza a movimentação de wasd, e cria os "frames" da renderização.
        /// </summary>
        private void updateWASDControls(object sender, EventArgs e)
        {
            bool needsRedraw = false;

            // Camera rotation via mouse movement.
            if (isMouseDown && isMouseMove)
                needsRedraw = true;

            // Keyboard navigation (WASD, QE).
            if (isMouseDown && !isControlDown && camera.CamMode == Camera.CameraMode.FLY)
            {
                if (isWDown) { camera.updateCameraToFront(); needsRedraw = true; }
                if (isSDown) { camera.updateCameraToBack(); needsRedraw = true; }
                if (isDDown) { camera.updateCameraToRight(); needsRedraw = true; }
                if (isADown) { camera.updateCameraToLeft(); needsRedraw = true; }
                if (isQDown) { camera.updateCameraToDown(); needsRedraw = true; }
                if (isEDown) { camera.updateCameraToUp(); needsRedraw = true; }
            }

            if (needsRedraw)
            {
                camMtx = camera.GetViewMatrix();
                glControl.Invalidate();
            }
        }

        private bool theAppLoadedWell = true; //o app carregou corretamente, sem erro na versão do openGL 

        private void GlControl_Load(object sender, EventArgs e)
        {
            try
            {
                Globals.OpenGLVersion = GL.GetString(StringName.Version)?.Trim() ?? "";

                if (Globals.OpenGLVersion.StartsWith("1.")
                    || Globals.OpenGLVersion.StartsWith("2.")
                    || Globals.OpenGLVersion.StartsWith("3.0")
                    || Globals.OpenGLVersion.StartsWith("3.1")
                    || Globals.OpenGLVersion.StartsWith("3.2")
                    )
                {
                    SplashScreen.Container?.Close?.Invoke();
                    this.TopMost = true;
                    MessageBox.Show(
                        "Error: You have an outdated version of OpenGL, which is not supported by this program." +
                        " The program will now exit.\n\n" +
                        "OpenGL version: [" + Globals.OpenGLVersion + "]\n",
                        "OpenGL version error:",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    theAppLoadedWell = false;
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                SplashScreen.Container?.Close?.Invoke();
                this.TopMost = true;
                MessageBox.Show(
                      "Error: " +
                      ex.Message,
                      "Error detecting OpenGL version:",
                      MessageBoxButtons.OK,
                     MessageBoxIcon.Error);
                theAppLoadedWell = false;
                this.Close();
                return;
            }

            if (theAppLoadedWell)
            {
                SplashScreen.Container?.SetProgress(110);

                GL.Viewport(0, 0, glControl.Width, glControl.Height);
                GL.ClearColor(Globals.SkyColor);

                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Texture2D);
                GL.LineWidth(1.5f);

                DataShader.StartLoad();
                Utils.StartLoadObjsModels();

                gizmo = new Gizmo();

                glControl.SwapBuffers();

                SplashScreen.Container?.Close?.Invoke();

                this.Activate();

                //force maximize
                if (Globals.BackupConfigs.MaximizeEditorOnStartup)
                    this.WindowState = FormWindowState.Maximized;
            }

        }


        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            if (RenderSelectViewer)
            {
                NewAgeTheRender.TheRender.AllRender(ref camMtx, ref ProjMatrix, camera.Position, camera.SelectedObjPosY(), gizmo, camera, Globals.CurrentTool, Globals.CurrentGizmoSpace, true); // este é da seleção
            }
            else
            {
                NewAgeTheRender.TheRender.AllRender(ref camMtx, ref ProjMatrix, camera.Position, camera.SelectedObjPosY(), gizmo, camera, Globals.CurrentTool, Globals.CurrentGizmoSpace); // rederiza todos os objetos do GL;
            }
            glControl.SwapBuffers();
        }

        bool RenderSelectViewer = false;
        private void toolStripMenuItemRenderSelectViewer_Click(object sender, EventArgs e)
        {
            RenderSelectViewer = !RenderSelectViewer;
            glControl.Invalidate();
        }

        #endregion


        #region botões do menu edit

        private void toolStripMenuItemAddNewObj_Click(object sender, EventArgs e)
        {
            addNewObject();
        }

        private void addNewObject()
        {
            AddNewObjForm form = new AddNewObjForm();
            form.OnButtonOk_Click += OnButtonOk_Click;
            form.TreeViewDisableDrawNode += TreeViewDisableDrawNode;
            form.TreeViewEnableDrawNode += TreeViewEnableDrawNode;
            form.ShowDialog();
        }

        private void OnButtonOk_Click()
        {
            UpdateTreeViewObjs();
            UpdatePropertyGrid();
            UpdateTreeViewNodes();
            UpdateGL();
        }

        private void toolStripMenuItemDeleteSelectedObj_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Lang.GetText(eLang.DeleteObjDialog), Lang.GetText(eLang.DeleteObjWarning), MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                foreach (Object3D item in treeViewObjs.SelectedNodes.Values)
                {
                    if (item.Group == GroupType.ETS
                        || item.Group == GroupType.DSE
                        || item.Group == GroupType.FSE
                        || item.Group == GroupType.SAR
                        || item.Group == GroupType.EAR
                        || item.Group == GroupType.ESE
                        || item.Group == GroupType.EMI
                        || item.Group == GroupType.QUAD_CUSTOM
                        || item.Group == GroupType.LIT_ENTRYS
                        || item.Group == GroupType.LIT_GROUPS
                        || item.Group == GroupType.EFF_EffectEntry
                        || item.Group == GroupType.EFF_Table0
                        || item.Group == GroupType.EFF_Table1
                        || item.Group == GroupType.EFF_Table2
                        || item.Group == GroupType.EFF_Table3
                        || item.Group == GroupType.EFF_Table4
                        || item.Group == GroupType.EFF_Table6
                        || item.Group == GroupType.EFF_Table9
                        || item.Group == GroupType.EFF_Table7_Effect_0
                        || item.Group == GroupType.EFF_Table8_Effect_1
                        )
                    {
                        var parent = item.Parent;

                        if (parent is Editor.Class.Interfaces.INodeChangeAmount nodeGroup)
                        {
                            item.Remove();
                            nodeGroup.ChangeAmountMethods.RemoveLineID(item.ObjLineRef);
                        }

                        if (parent is Editor.Class.Interfaces.IChangeAmountIndexFix nodeIndexFix)
                        {
                            nodeIndexFix.OnDeleteNode();
                        }
                    }
                    else if (item.Group == GroupType.ITA || item.Group == GroupType.AEV)
                    {
                        DataBase.Extras.RemoveObj(item.ObjLineRef, Utils.GroupTypeToSpecialFileFormat(item.Group));
                        var ChangeAmountMethods = ((SpecialNodeGroup)item.Parent).ChangeAmountMethods;
                        item.Remove();
                        ChangeAmountMethods.RemoveLineID(item.ObjLineRef);
                    }
                }
                TreeViewUpdateSelectedsClear();
                UpdateTreeViewNodes();
                glControl.Invalidate();
            }
        }

        private void toolStripMenuItemMoveUp_Click(object sender, EventArgs e)
        {
            var ordernedSelectedNodes = treeViewObjs.SelectedNodes.Values.OrderBy(n => n.Index);
            foreach (Object3D item in ordernedSelectedNodes)
            {
                if (item.Group == GroupType.ETS
                    || item.Group == GroupType.ITA
                    || item.Group == GroupType.AEV
                    || item.Group == GroupType.DSE
                    || item.Group == GroupType.FSE
                    || item.Group == GroupType.SAR
                    || item.Group == GroupType.EAR
                    || item.Group == GroupType.ESE
                    || item.Group == GroupType.EMI
                    || item.Group == GroupType.QUAD_CUSTOM
                    || item.Group == GroupType.LIT_ENTRYS
                    || item.Group == GroupType.LIT_GROUPS
                    || item.Group == GroupType.EFF_EffectEntry
                    || item.Group == GroupType.EFF_Table0
                    || item.Group == GroupType.EFF_Table1
                    || item.Group == GroupType.EFF_Table2
                    || item.Group == GroupType.EFF_Table3
                    || item.Group == GroupType.EFF_Table4
                    || item.Group == GroupType.EFF_Table6
                    || item.Group == GroupType.EFF_Table9
                    || item.Group == GroupType.EFF_Table7_Effect_0
                    || item.Group == GroupType.EFF_Table8_Effect_1
                    )
                {
                    int index = item.Index;
                    if (index > 0)
                    {
                        var Parent = item.Parent;
                        item.Remove();
                        Parent.Nodes.Insert(index - 1, item);

                        if (Parent is Editor.Class.Interfaces.IChangeAmountIndexFix nodeIndexFix)
                        {
                            nodeIndexFix.OnMoveNode();
                            UpdatePropertyGrid();
                        }
                    }
                }
            }
        }

        private void toolStripMenuItemMoveDown_Click(object sender, EventArgs e)
        {
            var invSelectedNodes = treeViewObjs.SelectedNodes.Values.OrderByDescending(n => n.Index);
            foreach (Object3D item in invSelectedNodes)
            {
                if (item.Group == GroupType.ETS
                    || item.Group == GroupType.ITA
                    || item.Group == GroupType.AEV
                    || item.Group == GroupType.DSE
                    || item.Group == GroupType.FSE
                    || item.Group == GroupType.SAR
                    || item.Group == GroupType.EAR
                    || item.Group == GroupType.ESE
                    || item.Group == GroupType.EMI
                    || item.Group == GroupType.QUAD_CUSTOM
                    || item.Group == GroupType.LIT_ENTRYS
                    || item.Group == GroupType.LIT_GROUPS
                    || item.Group == GroupType.EFF_EffectEntry
                    || item.Group == GroupType.EFF_Table0
                    || item.Group == GroupType.EFF_Table1
                    || item.Group == GroupType.EFF_Table2
                    || item.Group == GroupType.EFF_Table3
                    || item.Group == GroupType.EFF_Table4
                    || item.Group == GroupType.EFF_Table6
                    || item.Group == GroupType.EFF_Table9
                    || item.Group == GroupType.EFF_Table7_Effect_0
                    || item.Group == GroupType.EFF_Table8_Effect_1
                    )
                {
                    int index = item.Index;
                    var Parent = item.Parent;
                    if (index < Parent.GetNodeCount(false) - 1)
                    {
                        item.Remove();
                        Parent.Nodes.Insert(index + 1, item);

                        if (Parent is Editor.Class.Interfaces.IChangeAmountIndexFix nodeIndexFix)
                        {
                            nodeIndexFix.OnMoveNode();
                            UpdatePropertyGrid();
                        }
                    }
                }
            }
        }


        private void toolStripMenuItemSearch_Click(object sender, EventArgs e)
        {
            var selectedObj = propertyGridObjs.SelectedObject;
            if (selectedObj is EnemyProperty enemy)
            {
                SearchForm search = new SearchForm(ListBoxProperty.EnemiesList.Values.ToArray(), new UshortObjForListBox(enemy.ReturnUshortFirstSearchSelect(), ""));
                search.Search += enemy.Searched;
                search.ShowDialog();
            }
            else if (selectedObj is EtcModelProperty etcModel)
            {
                SearchForm search = new SearchForm(ListBoxProperty.EtcmodelsList.Values.ToArray(), new UshortObjForListBox(etcModel.ReturnUshortFirstSearchSelect(), ""));
                search.Search += etcModel.Searched;
                search.ShowDialog();
            }
            else if (selectedObj is SpecialProperty special)
            {
                var specialType = special.GetSpecialType();
                if (specialType == SpecialType.T03_Items || specialType == SpecialType.T11_ItemDependentEvents)
                {
                    SearchForm search = new SearchForm(ListBoxProperty.ItemsList.Values.ToArray(), new UshortObjForListBox(special.ReturnUshortFirstSearchSelect(), ""));
                    search.Search += special.Searched;
                    search.ShowDialog();
                }
            }
            else if (selectedObj is QuadCustomProperty quad)
            {
                SearchForm search = new SearchForm(ListBoxProperty.QuadCustomModelIDList.Values.ToArray(), new UintObjForListBox(quad.ReturnUshortFirstSearchSelect(), ""));
                search.Search += quad.Searched;
                search.ShowDialog();
            }

        }


        #endregion

        #region searchbar logic
        public static void SetPlaceholder(Control control, string text)
        {
            if (control is TextBox)
            {
                SendMessage(control.Handle, EM_SETCUEBANNER, 0, text);
            }
        }

        #endregion

        #region Botoes do menu

        private void SelectRoom_onLoadButtonClick(object sender, EventArgs e)
        {
            if (sender is string == false)
            {
                string text = Lang.GetText(eLang.SelectedRoom) + ": " + sender.ToString();
                if (text.Length > 100)
                {
                    text = text.Substring(0, 100);
                    text += "...";
                }
                currentRoomLabelToggle(true, text);
            }
            else
            {
                currentRoomLabelToggle(false);
            }

            if (Globals.AutoDefinedRoom)
            {
                if (DataBase.SelectedRoom != null)
                {
                    toolStripTextBoxDefinedRoom.Text = DataBase.SelectedRoom.GetRoomId().ToString("X4");
                }
                else
                {
                    toolStripTextBoxDefinedRoom.Text = "0000";
                }
            }
        }

        private void SelectRoomWindow()
        {
            SelectRoomForm selectRoom = new SelectRoomForm();

            if (Editor.Forms.SelectRoomForm.isFirstTimeLoading) Editor.Console.Log($"Loading JSON room lists for the first time...");

            selectRoom.onLoadButtonClick += SelectRoom_onLoadButtonClick;
            selectRoom.ShowDialog();
            glControl.Invalidate();
        }

        private void toolStripMenuItemSelectRoom_Click(object sender, EventArgs e)
        {
            SelectRoomWindow();
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItemCredits_Click(object sender, EventArgs e)
        {
            CreditsForm form = new CreditsForm();
            form.ShowDialog();
        }

        private void toolStripMenuItemOptions_Click(object sender, EventArgs e)
        {
            OptionsForm form = new OptionsForm();
            form.OnOKButtonClick += OptionsForm_OnOKButtonClick;
            form.OnOKButtonClick += UpdateTreeViewObjs;
            form.OnOKButtonClick += UpdateTreeViewNodes;
            form.OnOKButtonClick += UpdatePropertyGrid;
            form.ShowDialog();
            glControl.Invalidate();
        }

        private void OptionsForm_OnOKButtonClick()
        {
            TreeViewUpdateSelectedsClear();
        }

        private void toolStripMenuItemCameraMenu_Click(object sender, EventArgs e)
        {
            CameraForm cameraForm = new CameraForm(ref camera, UpdateGL, UpdateCameraMatrix);
            cameraForm.ShowDialog();
        }

        #endregion


        #region botoes do menu view

        private void toolStripMenuItemHideFileEFF_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideFileEFF.Checked = !toolStripMenuItemHideFileEFF.Checked;
            Globals.RenderFileEFFBLOB = !toolStripMenuItemHideFileEFF.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideFileLIT_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideFileLIT.Checked = !toolStripMenuItemHideFileLIT.Checked;
            Globals.RenderFileLIT = !toolStripMenuItemHideFileLIT.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideRoomModel_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideRoomModel.Checked = !toolStripMenuItemHideRoomModel.Checked;
            Globals.RenderRoom = !toolStripMenuItemHideRoomModel.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideEnemyESL_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideEnemyESL.Checked = !toolStripMenuItemHideEnemyESL.Checked;
            Globals.RenderEnemyESL = !toolStripMenuItemHideEnemyESL.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideEtcmodelETS_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideEtcmodelETS.Checked = !toolStripMenuItemHideEtcmodelETS.Checked;
            Globals.RenderEtcmodelETS = !toolStripMenuItemHideEtcmodelETS.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideItemsITA_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideItemsITA.Checked = !toolStripMenuItemHideItemsITA.Checked;
            Globals.RenderItemsITA = !toolStripMenuItemHideItemsITA.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideEventsAEV_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideEventsAEV.Checked = !toolStripMenuItemHideEventsAEV.Checked;
            Globals.RenderEventsAEV = !toolStripMenuItemHideEventsAEV.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }


        private void toolStripMenuItemHideFileFSE_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideFileFSE.Checked = !toolStripMenuItemHideFileFSE.Checked;
            Globals.RenderFileFSE = !toolStripMenuItemHideFileFSE.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideFileSAR_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideFileSAR.Checked = !toolStripMenuItemHideFileSAR.Checked;
            Globals.RenderFileSAR = !toolStripMenuItemHideFileSAR.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideFileEAR_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideFileEAR.Checked = !toolStripMenuItemHideFileEAR.Checked;
            Globals.RenderFileEAR = !toolStripMenuItemHideFileEAR.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideFileESE_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideFileESE.Checked = !toolStripMenuItemHideFileESE.Checked;
            Globals.RenderFileESE = !toolStripMenuItemHideFileESE.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideFileEMI_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideFileEMI.Checked = !toolStripMenuItemHideFileEMI.Checked;
            Globals.RenderFileEMI = !toolStripMenuItemHideFileEMI.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideQuadCustom_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideQuadCustom.Checked = !toolStripMenuItemHideQuadCustom.Checked;
            Globals.RenderFileQuadCustom = !toolStripMenuItemHideQuadCustom.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }


        private void toolStripMenuItemHideDesabledEnemy_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideDesabledEnemy.Checked = !toolStripMenuItemHideDesabledEnemy.Checked;
            Globals.RenderDisabledEnemy = !toolStripMenuItemHideDesabledEnemy.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripTextBoxDefinedRoom_TextChanged(object sender, EventArgs e)
        {
            Globals.RenderEnemyFromDefinedRoom = ushort.Parse(toolStripTextBoxDefinedRoom.Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripTextBoxDefinedRoom_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar)
                || e.KeyChar == 'A'
                || e.KeyChar == 'B'
                || e.KeyChar == 'C'
                || e.KeyChar == 'D'
                || e.KeyChar == 'E'
                || e.KeyChar == 'F'
                || e.KeyChar == 'a'
                || e.KeyChar == 'b'
                || e.KeyChar == 'c'
                || e.KeyChar == 'd'
                || e.KeyChar == 'e'
                || e.KeyChar == 'f'
                )
            {
                if (toolStripTextBoxDefinedRoom.SelectionStart < toolStripTextBoxDefinedRoom.TextLength)
                {
                    int CacheSelectionStart = toolStripTextBoxDefinedRoom.SelectionStart;
                    StringBuilder sb = new StringBuilder(toolStripTextBoxDefinedRoom.Text);
                    sb[toolStripTextBoxDefinedRoom.SelectionStart] = e.KeyChar;
                    toolStripTextBoxDefinedRoom.Text = sb.ToString();
                    toolStripTextBoxDefinedRoom.SelectionStart = CacheSelectionStart + 1;
                }
            }
            e.Handled = true;
        }


        private void toolStripMenuItemShowOnlyDefinedRoom_Click(object sender, EventArgs e)
        {
            toolStripMenuItemShowOnlyDefinedRoom.Checked = !toolStripMenuItemShowOnlyDefinedRoom.Checked;
            Globals.RenderDontShowOnlyDefinedRoom = !toolStripMenuItemShowOnlyDefinedRoom.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemAutoDefineRoom_Click(object sender, EventArgs e)
        {
            toolStripMenuItemAutoDefineRoom.Checked = !toolStripMenuItemAutoDefineRoom.Checked;
            Globals.AutoDefinedRoom = toolStripMenuItemAutoDefineRoom.Checked;
        }

        private void toolStripMenuItemHideItemTriggerZone_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideItemTriggerZone.Checked = !toolStripMenuItemHideItemTriggerZone.Checked;
            Globals.RenderItemTriggerZone = !toolStripMenuItemHideItemTriggerZone.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideItemTriggerRadius_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideItemTriggerRadius.Checked = !toolStripMenuItemHideItemTriggerRadius.Checked;
            Globals.RenderItemTriggerRadius = !toolStripMenuItemHideItemTriggerRadius.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }


        private void toolStripMenuItemItemPositionAtAssociatedObjectLocation_Click(object sender, EventArgs e)
        {
            toolStripMenuItemItemPositionAtAssociatedObjectLocation.Checked = !toolStripMenuItemItemPositionAtAssociatedObjectLocation.Checked;
            Globals.RenderItemPositionAtAssociatedObjectLocation = toolStripMenuItemItemPositionAtAssociatedObjectLocation.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideExtraObjs_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideExtraObjs.Checked = !toolStripMenuItemHideExtraObjs.Checked;
            Globals.RenderExtraObjs = !toolStripMenuItemHideExtraObjs.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideSpecialTriggerZone_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideSpecialTriggerZone.Checked = !toolStripMenuItemHideSpecialTriggerZone.Checked;
            Globals.RenderSpecialTriggerZone = !toolStripMenuItemHideSpecialTriggerZone.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemUseMoreSpecialColors_Click(object sender, EventArgs e)
        {
            toolStripMenuItemUseMoreSpecialColors.Checked = !toolStripMenuItemUseMoreSpecialColors.Checked;
            Globals.UseMoreSpecialColors = toolStripMenuItemUseMoreSpecialColors.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemUseCustomColors_Click(object sender, EventArgs e)
        {
            toolStripMenuItemUseCustomColors.Checked = !toolStripMenuItemUseCustomColors.Checked;
            Globals.UseMoreQuadCustomColors = toolStripMenuItemUseCustomColors.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }


        private void toolStripMenuItemEtcModelUseScale_Click(object sender, EventArgs e)
        {
            toolStripMenuItemEtcModelUseScale.Checked = !toolStripMenuItemEtcModelUseScale.Checked;
            Globals.RenderEtcmodelUsingScale = toolStripMenuItemEtcModelUseScale.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideExtraExceptWarpDoor_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideExtraExceptWarpDoor.Checked = !toolStripMenuItemHideExtraExceptWarpDoor.Checked;
            Globals.HideExtraExceptWarpDoor = toolStripMenuItemHideExtraExceptWarpDoor.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideOnlyWarpDoor_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideOnlyWarpDoor.Checked = !toolStripMenuItemHideOnlyWarpDoor.Checked;
            Globals.RenderExtraWarpDoor = !toolStripMenuItemHideOnlyWarpDoor.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemNodeDisplayNameInHex_Click(object sender, EventArgs e)
        {
            toolStripMenuItemNodeDisplayNameInHex.Checked = !toolStripMenuItemNodeDisplayNameInHex.Checked;
            Globals.TreeNodeRenderHexValues = toolStripMenuItemNodeDisplayNameInHex.Checked;
            TreeViewDisableDrawNode();
            if (Globals.TreeNodeRenderHexValues)
            {
                treeViewObjs.Font = Globals.TreeNodeFontHex;
            }
            else
            {
                treeViewObjs.Font = Globals.TreeNodeFontText;
            }
            TreeViewEnableDrawNode();
            treeViewObjs.Refresh();
        }

        private void toolStripMenuItemRefresh_Click(object sender, EventArgs e)
        {
            glControl.Invalidate();
            treeViewObjs.Refresh();
            propertyGridObjs.Refresh();
            glControl.Update(); // Needed after calling propertyGridObjs.Refresh();
        }

        private void toolStripMenuItemResetCamera_Click(object sender, EventArgs e)
        {
            cameraMove.ResetCamera();
        }


        private void toolStripMenuItemHideSideMenu_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItemHideLateralMenu.Checked) // fazer reaparecer
            {
                splitContainerMain.Panel1.Enabled = true;
                splitContainerMain.Panel1Collapsed = false;

                toolStripMenuItemHideLateralMenu.Checked = false;
            }
            else //fazer esconder
            {
                splitContainerMain.Panel1Collapsed = true;
                splitContainerMain.Panel1.Enabled = false;

                toolStripMenuItemHideLateralMenu.Checked = true;
            }
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideBottomMenu_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItemHideBottomMenu.Checked) // fazer reaparecer
            {
                glControls_old.Enabled = true;
                //glControls_old.Collapsed = false;

                toolStripMenuItemHideBottomMenu.Checked = false;
            }
            else //fazer esconder
            {
                glControls_old.Enabled = false;
                //glControls_old.Collapsed = true;

                toolStripMenuItemHideBottomMenu.Checked = true;
            }

            glControl.Invalidate();
        }

        //------------
        private void toolStripMenuItemRoomHideTextures_Click(object sender, EventArgs e)
        {
            toolStripMenuItemRoomHideTextures.Checked = !toolStripMenuItemRoomHideTextures.Checked;
            NewAgeTheRender.RoomSelectedObj.RenderTextures = !NewAgeTheRender.RoomSelectedObj.RenderTextures;
            glControl.Invalidate();
        }

        private void toolStripMenuItemRoomWireframe_Click(object sender, EventArgs e)
        {
            toolStripMenuItemRoomWireframe.Checked = !toolStripMenuItemRoomWireframe.Checked;
            NewAgeTheRender.RoomSelectedObj.RenderWireframe = !NewAgeTheRender.RoomSelectedObj.RenderWireframe;
            glControl.Invalidate();
        }

        private void toolStripMenuItemRoomRenderNormals_Click(object sender, EventArgs e)
        {
            toolStripMenuItemRoomRenderNormals.Checked = !toolStripMenuItemRoomRenderNormals.Checked;
            NewAgeTheRender.RoomSelectedObj.RenderNormals = !NewAgeTheRender.RoomSelectedObj.RenderNormals;
            glControl.Invalidate();
        }

        private void toolStripMenuItemRoomOnlyFrontFace_Click(object sender, EventArgs e)
        {
            toolStripMenuItemRoomOnlyFrontFace.Checked = !toolStripMenuItemRoomOnlyFrontFace.Checked;
            NewAgeTheRender.RoomSelectedObj.RenderOnlyFrontFace = !NewAgeTheRender.RoomSelectedObj.RenderOnlyFrontFace;
            glControl.Invalidate();
        }

        private void toolStripMenuItemRoomVertexColor_Click(object sender, EventArgs e)
        {
            toolStripMenuItemRoomVertexColor.Checked = !toolStripMenuItemRoomVertexColor.Checked;
            NewAgeTheRender.RoomSelectedObj.RenderVertexColor = !NewAgeTheRender.RoomSelectedObj.RenderVertexColor;
            glControl.Invalidate();
        }

        private void toolStripMenuItemRoomAlphaChannel_Click(object sender, EventArgs e)
        {
            toolStripMenuItemRoomAlphaChannel.Checked = !toolStripMenuItemRoomAlphaChannel.Checked;
            NewAgeTheRender.RoomSelectedObj.RenderAlphaChannel = !NewAgeTheRender.RoomSelectedObj.RenderAlphaChannel;
            glControl.Invalidate();
        }

        private void toolStripMenuItemModelsHideTextures_Click(object sender, EventArgs e)
        {
            toolStripMenuItemModelsHideTextures.Checked = !toolStripMenuItemModelsHideTextures.Checked;
            NewAgeTheRender.ObjModel3D.RenderTextures = !NewAgeTheRender.ObjModel3D.RenderTextures;
            glControl.Invalidate();
        }

        private void toolStripMenuItemModelsWireframe_Click(object sender, EventArgs e)
        {
            toolStripMenuItemModelsWireframe.Checked = !toolStripMenuItemModelsWireframe.Checked;
            NewAgeTheRender.ObjModel3D.RenderWireframe = !NewAgeTheRender.ObjModel3D.RenderWireframe;
            glControl.Invalidate();
        }

        private void toolStripMenuItemModelsRenderNormals_Click(object sender, EventArgs e)
        {
            toolStripMenuItemModelsRenderNormals.Checked = !toolStripMenuItemModelsRenderNormals.Checked;
            NewAgeTheRender.ObjModel3D.RenderNormals = !NewAgeTheRender.ObjModel3D.RenderNormals;
            glControl.Invalidate();
        }

        private void toolStripMenuItemModelsOnlyFrontFace_Click(object sender, EventArgs e)
        {
            toolStripMenuItemModelsOnlyFrontFace.Checked = !toolStripMenuItemModelsOnlyFrontFace.Checked;
            NewAgeTheRender.ObjModel3D.RenderOnlyFrontFace = !NewAgeTheRender.ObjModel3D.RenderOnlyFrontFace;
            glControl.Invalidate();
        }

        private void toolStripMenuItemModelsVertexColor_Click(object sender, EventArgs e)
        {
            toolStripMenuItemModelsVertexColor.Checked = !toolStripMenuItemModelsVertexColor.Checked;
            NewAgeTheRender.ObjModel3D.RenderVertexColor = !NewAgeTheRender.ObjModel3D.RenderVertexColor;
            glControl.Invalidate();
        }

        private void toolStripMenuItemModelsAlphaChannel_Click(object sender, EventArgs e)
        {
            toolStripMenuItemModelsAlphaChannel.Checked = !toolStripMenuItemModelsAlphaChannel.Checked;
            NewAgeTheRender.ObjModel3D.RenderAlphaChannel = !NewAgeTheRender.ObjModel3D.RenderAlphaChannel;
            glControl.Invalidate();
        }

        private void toolStripMenuItemRoomTextureNearestLinear_Click(object sender, EventArgs e)
        {
            NewAgeTheRender.RoomSelectedObj.LoadTextureLinear = !NewAgeTheRender.RoomSelectedObj.LoadTextureLinear;

            toolStripMenuItemRoomTextureNearestLinear.Text =
                NewAgeTheRender.RoomSelectedObj.LoadTextureLinear ?
                Lang.GetText(eLang.toolStripMenuItemRoomTextureIsLinear) :
                Lang.GetText(eLang.toolStripMenuItemRoomTextureIsNearest);

            DataBase.SelectedRoom?.ChangeTextureType();

            glControl.Invalidate();
        }

        private void toolStripMenuItemModelsTextureNearestLinear_Click(object sender, EventArgs e)
        {
            NewAgeTheRender.ObjModel3D.LoadTextureLinear = !NewAgeTheRender.ObjModel3D.LoadTextureLinear;

            toolStripMenuItemModelsTextureNearestLinear.Text =
              NewAgeTheRender.ObjModel3D.LoadTextureLinear ?
              Lang.GetText(eLang.toolStripMenuItemModelsTextureIsLinear) :
              Lang.GetText(eLang.toolStripMenuItemModelsTextureIsNearest);

            Utils.ChangeTextureTypeFromModels();

            glControl.Invalidate();
        }

        private void toolStripMenuItemShowOnlySelectedGroup_Click(object sender, EventArgs e)
        {
            toolStripMenuItemShowOnlySelectedGroup.Checked = !toolStripMenuItemShowOnlySelectedGroup.Checked;
            Globals.LIT_ShowOnlySelectedGroup = toolStripMenuItemShowOnlySelectedGroup.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemSelectedGroupUp_Click(object sender, EventArgs e)
        {
            var value = int.Parse(toolStripTextBoxSelectedGroupValue.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            value++;
            if (value > 999)
            {
                value = 999;
            }
            toolStripTextBoxSelectedGroupValue.Text = value.ToString("D3");
        }

        private void toolStripMenuItemSelectedGroupDown_Click(object sender, EventArgs e)
        {
            var value = int.Parse(toolStripTextBoxSelectedGroupValue.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            value--;
            if (value < 0)
            {
                value = 0;
            }
            toolStripTextBoxSelectedGroupValue.Text = value.ToString("D3");
        }

        private void toolStripMenuItemEnableLightColor_Click(object sender, EventArgs e)
        {
            toolStripMenuItemEnableLightColor.Checked = !toolStripMenuItemEnableLightColor.Checked;
            Globals.LIT_EnableLightColor = toolStripMenuItemEnableLightColor.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripTextBoxSelectedGroupValue_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                if (toolStripTextBoxSelectedGroupValue.SelectionStart < toolStripTextBoxSelectedGroupValue.TextLength)
                {
                    int CacheSelectionStart = toolStripTextBoxSelectedGroupValue.SelectionStart;
                    StringBuilder sb = new StringBuilder(toolStripTextBoxSelectedGroupValue.Text);
                    sb[toolStripTextBoxSelectedGroupValue.SelectionStart] = e.KeyChar;
                    toolStripTextBoxSelectedGroupValue.Text = sb.ToString();
                    toolStripTextBoxSelectedGroupValue.SelectionStart = CacheSelectionStart + 1;
                }
            }
            e.Handled = true;
        }

        private void toolStripTextBoxSelectedGroupValue_TextChanged(object sender, EventArgs e)
        {
            Globals.LIT_SelectedGroup = ushort.Parse(toolStripTextBoxSelectedGroupValue.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripTextBoxSelectedGroupValue_EFF_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                if (toolStripTextBoxSelectedGroupValue_EFF.SelectionStart < toolStripTextBoxSelectedGroupValue_EFF.TextLength)
                {
                    int CacheSelectionStart = toolStripTextBoxSelectedGroupValue_EFF.SelectionStart;
                    StringBuilder sb = new StringBuilder(toolStripTextBoxSelectedGroupValue_EFF.Text);
                    sb[toolStripTextBoxSelectedGroupValue_EFF.SelectionStart] = e.KeyChar;
                    toolStripTextBoxSelectedGroupValue_EFF.Text = sb.ToString();
                    toolStripTextBoxSelectedGroupValue_EFF.SelectionStart = CacheSelectionStart + 1;
                }
            }
            e.Handled = true;
        }

        private void toolStripTextBoxSelectedGroupValue_EFF_TextChanged(object sender, EventArgs e)
        {
            Globals.EFF_SelectedGroup = ushort.Parse(toolStripTextBoxSelectedGroupValue_EFF.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemShowOnlySelectedGroup_EFF_Click(object sender, EventArgs e)
        {
            toolStripMenuItemShowOnlySelectedGroup_EFF.Checked = !toolStripMenuItemShowOnlySelectedGroup_EFF.Checked;
            Globals.EFF_ShowOnlySelectedGroup = toolStripMenuItemShowOnlySelectedGroup_EFF.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemSelectedGroupUp_EFF_Click(object sender, EventArgs e)
        {
            var value = int.Parse(toolStripTextBoxSelectedGroupValue_EFF.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            value++;
            if (value > 999)
            {
                value = 999;
            }
            toolStripTextBoxSelectedGroupValue_EFF.Text = value.ToString("D3");
        }

        private void toolStripMenuItemSelectedGroupDown_EFF_Click(object sender, EventArgs e)
        {
            var value = int.Parse(toolStripTextBoxSelectedGroupValue_EFF.Text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            value--;
            if (value < 0)
            {
                value = 0;
            }
            toolStripTextBoxSelectedGroupValue_EFF.Text = value.ToString("D3");
        }

        private void toolStripMenuItemHideTable7_EFF_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideTable7_EFF.Checked = !toolStripMenuItemHideTable7_EFF.Checked;
            Globals.EFF_RenderTable7 = !toolStripMenuItemHideTable7_EFF.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideTable8_EFF_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideTable8_EFF.Checked = !toolStripMenuItemHideTable8_EFF.Checked;
            Globals.EFF_RenderTable8 = !toolStripMenuItemHideTable8_EFF.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemHideTable9_EFF_Click(object sender, EventArgs e)
        {
            toolStripMenuItemHideTable9_EFF.Checked = !toolStripMenuItemHideTable9_EFF.Checked;
            Globals.EFF_RenderTable9 = !toolStripMenuItemHideTable9_EFF.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        private void toolStripMenuItemDisableGroupPositionEFF_Click(object sender, EventArgs e)
        {
            toolStripMenuItemDisableGroupPositionEFF.Checked = !toolStripMenuItemDisableGroupPositionEFF.Checked;
            Globals.EFF_Use_Group_Position = !toolStripMenuItemDisableGroupPositionEFF.Checked;
            treeViewObjs.Refresh();
            glControl.Invalidate();
        }

        #endregion


        #region propertyGridObjs and TreeViewObjs

        private IObject3D getSelectedObject()
        {
            if (DataBase.LastSelectNode is IObject3D node)
            {
                return node;
            }
            return null;
        }

        private void UpdateGL()
        {
            glControl.Invalidate();
        }

        private void UpdateCameraMatrix()
        {
            camMtx = camera.GetViewMatrix();
        }

        private void UpdatePropertyGrid()
        {
            propertyGridObjs.Refresh();
            glControl.Update(); // Needed after calling propertyGridObjs.Refresh();
        }

        public void UpdateProjectionMatrix()
        {
            if (glControl == null && glControl.IsHandleCreated) return;

            ProjMatrix = ReturnNewProjMatrix();
            glControl.Invalidate();
        }


        private void UpdateTreeViewObjs()
        {
            treeViewObjs.Refresh();
        }

        private void UpdateOrbitCamera()
        {
            if (camera.isOrbitCamera())
            {
                camera.UpdateCameraOrbitOnChangeValue();
                camMtx = camera.GetViewMatrix();
            }
        }


        public void TreeViewUpdateSelectedsClear()
        {
            treeViewObjs.SelectedNodesClearNoRedraw();
            SetObjectToPropertyGrid(none);
            UpdateAllMoveControls();
            objectMove.UpdateSelection();
            treeViewObjs.Refresh();
            propertyGridObjs.Refresh();
        }

        public void TreeViewDisableDrawNode()
        {
            treeViewObjs.Enabled = false;
            //treeViewObjs.Visible = false;
            treeViewObjs.DisableDrawNode();
            //propertyGridObjs.Visible = false;
        }

        public void TreeViewEnableDrawNode()
        {
            treeViewObjs.EnableDrawNode();
            //treeViewObjs.Visible = true;
            treeViewObjs.Enabled = true;
            //propertyGridObjs.Visible = true;
        }

        private void SetObjectToPropertyGrid(object obj)
        {
            propertyGridOriginalObj = obj;
            UpdatePropertyGridDisplay();
        }

        #region propertygrid filter

        private void propertyGrid_searchField_TextChanged(object sender, EventArgs e)
        {
            UpdatePropertyGridDisplay();
        }

        #endregion

        #region treeview filter (this is not working well for child items, mostly for root - TODO)

        private void treeView_searchField_TextChanged(object sender, EventArgs e)
        {
            if (treeViewObjs.SelectedNode != null)
            {
                treeViewObjs.SelectedNode = null;
            }

            treeViewObjs.BeginUpdate();
            ShowAllNodes();

            string filter = treeView_searchField.Text;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                FilterNodesRecursive(treeViewObjs.Nodes, filter.ToLowerInvariant());
            }

            treeViewObjs.EndUpdate();
        }

        private void FilterNodesRecursive(TreeNodeCollection collection, string filter)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                var node = collection[i];
                FilterNodesRecursive(node.Nodes, filter);
                bool selfMatches = node.Text.ToLowerInvariant().Contains(filter);
                bool hasVisibleChildren = node.Nodes.Count > 0;

                if (!selfMatches && !hasVisibleChildren)
                {
                    _hiddenNodes[node] = (node.Parent, node.Index);
                    collection.RemoveAt(i);
                }
            }
        }

        private void ShowAllNodes() {
            if (_hiddenNodes.Count == 0)
            {
                return;
            }
            foreach (var entry in _hiddenNodes.OrderBy(kvp => kvp.Value.Index))
            {
                var nodeToRestore = entry.Key;
                var parent = entry.Value.Parent;
                var index = entry.Value.Index;

                if (parent == null)
                {
                    treeViewObjs.Nodes.Insert(index, nodeToRestore);
                }
                else
                {
                    parent.Nodes.Insert(index, nodeToRestore);
                }
            }
            _hiddenNodes.Clear();
        }
        #endregion

        private void treeViewObjs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            bool OldLastNodeIsNull = !(DataBase.LastSelectNode is Object3D);
            //Console.WriteLine(e.Node);
            //Console.WriteLine(treeViewObjs.SelectedNodes.Count);
            if (e.Node == null || e.Node.Parent == null || treeViewObjs.SelectedNodes.Count == 0)
            {
                SetObjectToPropertyGrid(none);
                DataBase.LastSelectNode = null;
            }
            else if (treeViewObjs.SelectedNodes.Count == 1 && e.Node is Object3D node)
            {
                DataBase.LastSelectNode = node;
                SetObjectToPropertyGrid(node.Property);
            }
            else if (treeViewObjs.SelectedNodes.Count > 1)
            {
                DataBase.LastSelectNode = treeViewObjs.SelectedNodes.Last().Value;

                MultiSelectProperty p = new MultiSelectProperty(updateMethods);
                int count = p.LoadContent(treeViewObjs.SelectedNodes.Values.ToList());
                if (count != 0)
                {
                    SetObjectToPropertyGrid(p);
                }
                else
                {
                    SetObjectToPropertyGrid(none);
                }
            }
            else
            {
                SetObjectToPropertyGrid(none);
                DataBase.LastSelectNode = null;
            }
            if (camera.isOrbitCamera())
            {
                if (OldLastNodeIsNull)
                {
                    camera.ResetOrbitToSelectedObject();
                }
                camera.UpdateCameraOrbitOnChangeObj();
                camMtx = camera.GetViewMatrix();
            }
            objectMove.UpdateSelection(); //legacy objectmove
            UpdateAllMoveControls();
            glControl.Invalidate();
        }

        #endregion


        #region Gerenciamento de arquivos //new

        private void toolStripMenuItemNewESL_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            FileManager.NewFileESL();
            Globals.FilePathESL = null;
            TreeViewEnableDrawNode();
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewETS_2007_PS2_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileETS(Re4Version.V2007PS2);
            Globals.FilePathETS = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewETS_UHD_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileETS(Re4Version.UHD);
            Globals.FilePathETS = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewITA_2007_PS2_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileITA(Re4Version.V2007PS2);
            Globals.FilePathITA = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewITA_UHD_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileITA(Re4Version.UHD);
            Globals.FilePathITA = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewAEV_2007_PS2_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileAEV(Re4Version.V2007PS2);
            Globals.FilePathAEV = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewAEV_UHD_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileAEV(Re4Version.UHD);
            Globals.FilePathAEV = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewDSE_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileDSE();
            Globals.FilePathDSE = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewFSE_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileFSE();
            Globals.FilePathFSE = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewSAR_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileSAR();
            Globals.FilePathSAR = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewEAR_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileEAR();
            Globals.FilePathEAR = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewEMI_2007_PS2_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileEMI(Re4Version.V2007PS2);
            Globals.FilePathEMI = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewESE_2007_PS2_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileESE(Re4Version.V2007PS2);
            Globals.FilePathESE = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewEMI_UHD_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileEMI(Re4Version.UHD);
            Globals.FilePathEMI = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewESE_UHD_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileESE(Re4Version.UHD);
            Globals.FilePathESE = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewQuadCustom_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileQuadCustom();
            Globals.FilePathQuadCustom = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewLIT_2007_PS2_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileLIT(Re4Version.V2007PS2);
            Globals.FilePathLIT = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewLIT_UHD_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileLIT(Re4Version.UHD);
            Globals.FilePathLIT = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewITA_PS4_NS_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileITA(Re4Version.UHD, true);
            Globals.FilePathITA = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewAEV_PS4_NS_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileAEV(Re4Version.UHD, true);
            Globals.FilePathAEV = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewEFFBLOB_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileEFFBLOB(Endianness.LittleEndian);
            Globals.FilePathEFFBLOB = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewEFFBLOBBIG_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileEFFBLOB(Endianness.BigEndian);
            Globals.FilePathEFFBLOB = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewCAM_BIG_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileCAM(IsRe4Version.BIG_ENDIAN);
            Globals.FilePathCAM = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewCAM_2007_PS2_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileCAM(IsRe4Version.V2007PS2);
            Globals.FilePathCAM = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewCAM_UHD_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileCAM(IsRe4Version.UHD);
            Globals.FilePathCAM = null;
            glControl.Invalidate();
        }

        private void toolStripMenuItemNewCAM_PS4NS_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            FileManager.NewFileCAM(IsRe4Version.PS4NS);
            Globals.FilePathCAM = null;
            glControl.Invalidate();
        }

        #endregion

        #region OPEN

        private bool OpenIsUHD = false;
        private bool OpenIsPs4Ns_Adapted = false;
        private IsRe4Version OpenIsRe4Version = IsRe4Version.NULL;

        private void OpenFile(CancelEventArgs e, OpenFileDialog dialog, Func<FileInfo, FileStream, bool> loadAction, Action clearAction, string filePathGlobal, bool isCamFile = false)
        {
            FileInfo fileInfo = null;
            FileStream fileStream = null;
            try
            {
                fileInfo = new FileInfo(dialog.FileName);

                if (fileInfo.Length == 0)
                {
                    MessageBox.Show(Lang.GetText(eLang.MessageBoxFile0MB), Lang.GetText(eLang.MessageBoxWarningTitle), MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }

                //specific size validation (16MB for most, 4 bytes for DSE/LIT)
                if (fileInfo.Length > 0x1000000)
                {
                    MessageBox.Show(Lang.GetText(eLang.MessageBoxFile16MB), Lang.GetText(eLang.MessageBoxWarningTitle), MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }

                //minimum size validation (16 bytes for majority, 4 for DSE/LIT)
                int minSize = 16;
                if (filePathGlobal.EndsWith(".DSE", StringComparison.OrdinalIgnoreCase) ||
                    filePathGlobal.EndsWith(".LIT", StringComparison.OrdinalIgnoreCase))
                {
                    minSize = 4;
                }

                if (!isCamFile && fileInfo.Length < minSize)
                {
                    if (minSize == 4) MessageBox.Show(Lang.GetText(eLang.MessageBoxFile4Bytes), Lang.GetText(eLang.MessageBoxWarningTitle), MessageBoxButtons.OK);
                    else MessageBox.Show(Lang.GetText(eLang.MessageBoxFile16Bytes), Lang.GetText(eLang.MessageBoxWarningTitle), MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }

                fileStream = fileInfo.OpenRead();

                TreeViewUpdateSelectedsClear();
                TreeViewDisableDrawNode();

                if (loadAction(fileInfo, fileStream))
                {
                    // Atualiza o Globals.FilePath no método de chamada (FileOk)
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                clearAction();
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
            }
            finally
            {
                fileStream?.Close();
                glControl.Invalidate();
                TreeViewEnableDrawNode();
                UpdateTreeViewNodes();
                dialog.FileName = null;
            }
        }

        private void toolStripMenuItemOpenESL_Click(object sender, EventArgs e)
        {
            openFileDialogESL.ShowDialog();
        }
        private void toolStripMenuItemOpenETS_2007_PS2_Click(object sender, EventArgs e)
        {
            OpenIsUHD = false;
            openFileDialogETS.ShowDialog();
        }
        private void toolStripMenuItemOpenETS_UHD_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            openFileDialogETS.ShowDialog();
        }
        private void toolStripMenuItemOpenITA_2007_PS2_Click(object sender, EventArgs e)
        {
            OpenIsUHD = false;
            OpenIsPs4Ns_Adapted = false;
            openFileDialogITA.ShowDialog();
        }
        private void toolStripMenuItemOpenITA_UHD_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            OpenIsPs4Ns_Adapted = false;
            openFileDialogITA.ShowDialog();
        }
        private void toolStripMenuItemOpenAEV_2007_PS2_Click(object sender, EventArgs e)
        {
            OpenIsUHD = false;
            OpenIsPs4Ns_Adapted = false;
            openFileDialogAEV.ShowDialog();
        }
        private void toolStripMenuItemOpenAEV_UHD_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            OpenIsPs4Ns_Adapted = false;
            openFileDialogAEV.ShowDialog();
        }
        private void toolStripMenuItemOpenITA_PS4_NS_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            OpenIsPs4Ns_Adapted = true;
            openFileDialogITA.ShowDialog();
        }
        private void toolStripMenuItemOpenAEV_PS4_NS_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            OpenIsPs4Ns_Adapted = true;
            openFileDialogAEV.ShowDialog();
        }
        private void toolStripMenuItemOpenDSE_Click(object sender, EventArgs e)
        {
            openFileDialogDSE.ShowDialog();
        }
        private void toolStripMenuItemOpenFSE_Click(object sender, EventArgs e)
        {
            openFileDialogFSE.ShowDialog();
        }
        private void toolStripMenuItemOpenSAR_Click(object sender, EventArgs e)
        {
            openFileDialogSAR.ShowDialog();
        }
        private void toolStripMenuItemOpenEAR_Click(object sender, EventArgs e)
        {
            openFileDialogEAR.ShowDialog();
        }
        private void toolStripMenuItemOpenEMI_2007_PS2_Click(object sender, EventArgs e)
        {
            OpenIsUHD = false;
            openFileDialogEMI.ShowDialog();
        }
        private void toolStripMenuItemOpenESE_2007_PS2_Click(object sender, EventArgs e)
        {
            OpenIsUHD = false;
            openFileDialogESE.ShowDialog();
        }
        private void toolStripMenuItemOpenEMI_UHD_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            openFileDialogEMI.ShowDialog();
        }
        private void toolStripMenuItemOpenESE_UHD_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            openFileDialogESE.ShowDialog();
        }
        private void toolStripMenuItemOpenQuadCustom_Click(object sender, EventArgs e)
        {
            openFileDialogQuadCustom.ShowDialog();
        }

        private void toolStripMenuItemOpenLIT_2007_PS2_Click(object sender, EventArgs e)
        {
            OpenIsUHD = false;
            openFileDialogLIT.ShowDialog();
        }

        private void toolStripMenuItemOpenLIT_UHD_Click(object sender, EventArgs e)
        {
            OpenIsUHD = true;
            openFileDialogLIT.ShowDialog();
        }

        private void toolStripMenuItemOpenEFFBLOB_Click(object sender, EventArgs e)
        {
            openFileDialogEFFBLOB.ShowDialog();
        }

        private void toolStripMenuItemOpenEFFBLOBBIG_Click(object sender, EventArgs e)
        {
            openFileDialogEFFBLOBBIG.ShowDialog();
        }

        private void toolStripMenuItemOpenCAM_BIG_Click(object sender, EventArgs e)
        {
            OpenIsRe4Version = IsRe4Version.BIG_ENDIAN;
            openFileDialogCAM.ShowDialog();
        }

        private void toolStripMenuItemOpenCAM_2007_PS2_Click(object sender, EventArgs e)
        {
            OpenIsRe4Version = IsRe4Version.V2007PS2;
            openFileDialogCAM.ShowDialog();
        }

        private void toolStripMenuItemOpenCAM_UHD_Click(object sender, EventArgs e)
        {
            OpenIsRe4Version = IsRe4Version.UHD;
            openFileDialogCAM.ShowDialog();
        }

        private void toolStripMenuItemOpenCAM_PS4NS_Click(object sender, EventArgs e)
        {
            OpenIsRe4Version = IsRe4Version.PS4NS;
            openFileDialogCAM.ShowDialog();
        }

        private void openFileDialogETS_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogETS, (fileInfo, fileStream) => {
                if (OpenIsUHD)
                {
                    FileManager.LoadFileETS_UHD(fileStream, fileInfo);
                }
                else
                {
                    FileManager.LoadFileETS_2007_PS2(fileStream, fileInfo);
                }
                Globals.FilePathETS = openFileDialogETS.FileName;
                return true;
            }, FileManager.ClearETS, openFileDialogETS.FileName);
        }

        private void openFileDialogITA_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogITA, (fileInfo, fileStream) => {
                if (OpenIsPs4Ns_Adapted)
                {
                    FileManager.LoadFileITA_PS4_NS(fileStream, fileInfo);
                }
                else if (OpenIsUHD)
                {
                    FileManager.LoadFileITA_UHD(fileStream, fileInfo);
                }
                else
                {
                    FileManager.LoadFileITA_2007_PS2(fileStream, fileInfo);
                }
                Globals.FilePathITA = openFileDialogITA.FileName;
                return true;
            }, FileManager.ClearITA, openFileDialogITA.FileName);
        }

        private void openFileDialogAEV_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogAEV, (fileInfo, fileStream) => {
                if (OpenIsPs4Ns_Adapted)
                {
                    FileManager.LoadFileAEV_PS4_NS(fileStream, fileInfo);
                }
                else if (OpenIsUHD)
                {
                    FileManager.LoadFileAEV_UHD(fileStream, fileInfo);
                }
                else
                {
                    FileManager.LoadFileAEV_2007_PS2(fileStream, fileInfo);
                }
                Globals.FilePathAEV = openFileDialogAEV.FileName;
                return true;
            }, FileManager.ClearAEV, openFileDialogAEV.FileName);
        }

        private void openFileDialogDSE_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogDSE, (fileInfo, fileStream) => {
                FileManager.LoadFileDSE(fileStream, fileInfo);
                Globals.FilePathDSE = openFileDialogDSE.FileName;
                return true;
            }, FileManager.ClearDSE, openFileDialogDSE.FileName);
        }

        private void openFileDialogFSE_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogFSE, (fileInfo, fileStream) => {
                FileManager.LoadFileFSE(fileStream, fileInfo);
                Globals.FilePathFSE = openFileDialogFSE.FileName;
                return true;
            }, FileManager.ClearFSE, openFileDialogFSE.FileName);
        }

        private void openFileDialogSAR_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogSAR, (fileInfo, fileStream) => {
                FileManager.LoadFileSAR(fileStream, fileInfo);
                Globals.FilePathSAR = openFileDialogSAR.FileName;
                return true;
            }, FileManager.ClearSAR, openFileDialogSAR.FileName);
        }

        private void openFileDialogEAR_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogEAR, (fileInfo, fileStream) => {
                FileManager.LoadFileEAR(fileStream, fileInfo);
                Globals.FilePathEAR = openFileDialogEAR.FileName;
                return true;
            }, FileManager.ClearEAR, openFileDialogEAR.FileName);
        }

        private void openFileDialogEMI_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogEMI, (fileInfo, fileStream) => {
                if (OpenIsUHD)
                {
                    FileManager.LoadFileEMI_UHD(fileStream, fileInfo);
                }
                else
                {
                    FileManager.LoadFileEMI_2007_PS2(fileStream, fileInfo);
                }
                Globals.FilePathEMI = openFileDialogEMI.FileName;
                return true;
            }, FileManager.ClearEMI, openFileDialogEMI.FileName);
        }

        private void openFileDialogESE_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogESE, (fileInfo, fileStream) => {
                if (OpenIsUHD)
                {
                    FileManager.LoadFileESE_UHD(fileStream, fileInfo);
                }
                else
                {
                    FileManager.LoadFileESE_2007_PS2(fileStream, fileInfo);
                }
                Globals.FilePathESE = openFileDialogESE.FileName;
                return true;
            }, FileManager.ClearESE, openFileDialogESE.FileName);
        }

        private void openFileDialogQuadCustom_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogQuadCustom, (fileInfo, fileStream) => {
                FileManager.LoadFileQuadCustom(fileStream, fileInfo);
                Globals.FilePathQuadCustom = openFileDialogQuadCustom.FileName;
                return true;
            }, FileManager.ClearQuadCustom, openFileDialogQuadCustom.FileName);
        }

        private void openFileDialogLIT_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogLIT, (fileInfo, fileStream) => {
                if (OpenIsUHD)
                {
                    FileManager.LoadFileLIT_UHD(fileStream, fileInfo);
                }
                else
                {
                    FileManager.LoadFileLIT_2007_PS2(fileStream, fileInfo);
                }
                Globals.FilePathLIT = openFileDialogLIT.FileName;
                return true;
            }, FileManager.ClearLIT, openFileDialogLIT.FileName);
        }

        private void openFileDialogEFFBLOB_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogEFFBLOB, (fileInfo, fileStream) => {
                FileManager.LoadFileEFFBLOB(fileStream, Endianness.LittleEndian);
                Globals.FilePathEFFBLOB = openFileDialogEFFBLOB.FileName;
                return true;
            }, FileManager.ClearEFFBLOB, openFileDialogEFFBLOB.FileName);
        }

        private void openFileDialogEFFBLOBBIG_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogEFFBLOBBIG, (fileInfo, fileStream) => {
                FileManager.LoadFileEFFBLOB(fileStream, Endianness.BigEndian);
                Globals.FilePathEFFBLOB = openFileDialogEFFBLOBBIG.FileName;
                return true;
            }, FileManager.ClearEFFBLOB, openFileDialogEFFBLOBBIG.FileName);
        }

        private void openFileDialogCAM_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogCAM, (fileInfo, fileStream) => {
                FileManager.LoadFileCAM(fileStream, OpenIsRe4Version);
                Globals.FilePathCAM = openFileDialogCAM.FileName;
                return true;
            }, FileManager.ClearCAM, openFileDialogCAM.FileName, true); //true for isCam
        }

        private void openFileDialogESL_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(e, openFileDialogESL, (fileInfo, fileStream) => {
                FileManager.LoadFileESL(fileStream, fileInfo);
                Globals.FilePathESL = openFileDialogESL.FileName;
                return true;
            }, FileManager.ClearESL, openFileDialogESL.FileName);
        }

        #endregion

        #region Gerenciamento de arquivos //Clear

        private void toolStripMenuItemClear_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItemClearESL.Enabled = DataBase.FileESL != null;
            toolStripMenuItemClearETS.Enabled = DataBase.FileETS != null;
            toolStripMenuItemClearITA.Enabled = DataBase.FileITA != null;
            toolStripMenuItemClearAEV.Enabled = DataBase.FileAEV != null;
            toolStripMenuItemClearDSE.Enabled = DataBase.FileDSE != null;
            toolStripMenuItemClearFSE.Enabled = DataBase.FileFSE != null;
            toolStripMenuItemClearSAR.Enabled = DataBase.FileSAR != null;
            toolStripMenuItemClearEAR.Enabled = DataBase.FileEAR != null;
            toolStripMenuItemClearEMI.Enabled = DataBase.FileEMI != null;
            toolStripMenuItemClearESE.Enabled = DataBase.FileESE != null;
            toolStripMenuItemClearLIT.Enabled = DataBase.FileLIT != null;
            toolStripMenuItemClearEFFBLOB.Enabled = DataBase.FileEFF != null;
            toolStripMenuItemClearQuadCustom.Enabled = DataBase.FileQuadCustom != null;
        }

        private void toolStripMenuItemClearESL_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearESL();
            Globals.FilePathESL = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearETS_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearETS();
            Globals.FilePathETS = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearITA_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearITA();
            Globals.FilePathITA = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearAEV_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearAEV();
            Globals.FilePathAEV = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearDSE_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearDSE();
            Globals.FilePathDSE = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearFSE_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearFSE();
            Globals.FilePathFSE = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearSAR_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearSAR();
            Globals.FilePathSAR = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearEAR_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearEAR();
            Globals.FilePathEAR = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearEMI_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearEMI();
            Globals.FilePathEMI = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearESE_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearESE();
            Globals.FilePathESE = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearQuadCustom_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearQuadCustom();
            Globals.FilePathQuadCustom = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearLIT_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearLIT();
            Globals.FilePathLIT = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        private void toolStripMenuItemClearEFFBLOB_Click(object sender, EventArgs e)
        {
            TreeViewUpdateSelectedsClear();
            TreeViewDisableDrawNode();
            UpdateTreeViewNodes();
            FileManager.ClearEFFBLOB();
            Globals.FilePathEFFBLOB = null;
            glControl.Invalidate();
            TreeViewEnableDrawNode();
        }

        #endregion

        #region Gerenciamento de arquivos //Save As..

        private void toolStripMenuItemSaveAs_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItemSaveAsESL.Enabled = DataBase.FileESL != null;
            toolStripMenuItemSaveAsETS.Enabled = DataBase.FileETS != null;
            toolStripMenuItemSaveAsITA.Enabled = DataBase.FileITA != null;
            toolStripMenuItemSaveAsAEV.Enabled = DataBase.FileAEV != null;
            toolStripMenuItemSaveAsDSE.Enabled = DataBase.FileDSE != null;
            toolStripMenuItemSaveAsFSE.Enabled = DataBase.FileFSE != null;
            toolStripMenuItemSaveAsSAR.Enabled = DataBase.FileSAR != null;
            toolStripMenuItemSaveAsEAR.Enabled = DataBase.FileEAR != null;
            toolStripMenuItemSaveAsEMI.Enabled = DataBase.FileEMI != null;
            toolStripMenuItemSaveAsESE.Enabled = DataBase.FileESE != null;
            toolStripMenuItemSaveAsLIT.Enabled = DataBase.FileLIT != null;
            toolStripMenuItemSaveAsEFFBLOB.Enabled = DataBase.FileEFF != null;
            toolStripMenuItemSaveAsQuadCustom.Enabled = DataBase.FileQuadCustom != null;

            if (DataBase.FileETS != null && DataBase.FileETS.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveAsETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsETS_2007_PS2);
            }
            else if (DataBase.FileETS != null && DataBase.FileETS.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveAsETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsETS_UHD);
            }
            else
            {
                toolStripMenuItemSaveAsETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsETS);
            }

            if (DataBase.FileITA != null && DataBase.FileITA.IsPs4Ns_Adapted)
            {
                toolStripMenuItemSaveAsITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsITA_PS4_NS);
            }
            else if (DataBase.FileITA != null && DataBase.FileITA.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveAsITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsITA_2007_PS2);
            }
            else if (DataBase.FileITA != null && DataBase.FileITA.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveAsITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsITA_UHD);
            }
            else
            {
                toolStripMenuItemSaveAsITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsITA);
            }

            if (DataBase.FileAEV != null && DataBase.FileAEV.IsPs4Ns_Adapted)
            {
                toolStripMenuItemSaveAsAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsAEV_PS4_NS);
            }
            else if (DataBase.FileAEV != null && DataBase.FileAEV.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveAsAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsAEV_2007_PS2);
            }
            else if (DataBase.FileAEV != null && DataBase.FileAEV.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveAsAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsAEV_UHD);
            }
            else
            {
                toolStripMenuItemSaveAsAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsAEV);
            }

            if (DataBase.FileEMI != null && DataBase.FileEMI.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveAsEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEMI_2007_PS2);
            }
            else if (DataBase.FileEMI != null && DataBase.FileEMI.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveAsEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEMI_UHD);
            }
            else
            {
                toolStripMenuItemSaveAsEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEMI);
            }

            if (DataBase.FileESE != null && DataBase.FileESE.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveAsESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsESE_2007_PS2);
            }
            else if (DataBase.FileESE != null && DataBase.FileESE.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveAsESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsESE_UHD);
            }
            else
            {
                toolStripMenuItemSaveAsESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsESE);
            }

            if (DataBase.FileLIT != null && DataBase.FileLIT.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveAsLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsLIT_2007_PS2);
            }
            else if (DataBase.FileLIT != null && DataBase.FileLIT.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveAsLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsLIT_UHD);
            }
            else
            {
                toolStripMenuItemSaveAsLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsLIT);
            }

            if (DataBase.FileEFF != null && DataBase.FileEFF.Endian == Endianness.LittleEndian)
            {
                toolStripMenuItemSaveAsEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEFFBLOB_LittleEndian);
            }
            else if (DataBase.FileEFF != null && DataBase.FileEFF.Endian == Endianness.BigEndian)
            {
                toolStripMenuItemSaveAsEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEFFBLOB_BigEndian);
            }
            else
            {
                toolStripMenuItemSaveAsEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEFFBLOB);
            }

        }

        private void toolStripMenuItemSaveAsESL_Click(object sender, EventArgs e)
        {
            saveFileDialogESL.FileName = Globals.FilePathESL;
            saveFileDialogESL.ShowDialog();
        }

        private void toolStripMenuItemSaveAsETS_Click(object sender, EventArgs e)
        {
            saveFileDialogETS.FileName = Globals.FilePathETS;
            saveFileDialogETS.ShowDialog();
        }

        private void toolStripMenuItemSaveAsITA_Click(object sender, EventArgs e)
        {
            saveFileDialogITA.FileName = Globals.FilePathITA;
            saveFileDialogITA.ShowDialog();
        }

        private void toolStripMenuItemSaveAsAEV_Click(object sender, EventArgs e)
        {
            saveFileDialogAEV.FileName = Globals.FilePathAEV;
            saveFileDialogAEV.ShowDialog();
        }

        private void toolStripMenuItemSaveAsDSE_Click(object sender, EventArgs e)
        {
            saveFileDialogDSE.FileName = Globals.FilePathDSE;
            saveFileDialogDSE.ShowDialog();
        }

        private void toolStripMenuItemSaveAsFSE_Click(object sender, EventArgs e)
        {
            saveFileDialogFSE.FileName = Globals.FilePathFSE;
            saveFileDialogFSE.ShowDialog();
        }

        private void toolStripMenuItemSaveAsSAR_Click(object sender, EventArgs e)
        {
            saveFileDialogSAR.FileName = Globals.FilePathSAR;
            saveFileDialogSAR.ShowDialog();
        }

        private void toolStripMenuItemSaveAsEAR_Click(object sender, EventArgs e)
        {
            saveFileDialogEAR.FileName = Globals.FilePathEAR;
            saveFileDialogEAR.ShowDialog();
        }

        private void toolStripMenuItemSaveAsEMI_Click(object sender, EventArgs e)
        {
            saveFileDialogEMI.FileName = Globals.FilePathEMI;
            saveFileDialogEMI.ShowDialog();
        }

        private void toolStripMenuItemSaveAsESE_Click(object sender, EventArgs e)
        {
            saveFileDialogESE.FileName = Globals.FilePathESE;
            saveFileDialogESE.ShowDialog();
        }

        private void toolStripMenuItemSaveAsQuadCustom_Click(object sender, EventArgs e)
        {
            saveFileDialogQuadCustom.FileName = Globals.FilePathQuadCustom;
            saveFileDialogQuadCustom.ShowDialog();
        }

        private void toolStripMenuItemSaveAsLIT_Click(object sender, EventArgs e)
        {
            saveFileDialogLIT.FileName = Globals.FilePathLIT;
            saveFileDialogLIT.ShowDialog();
        }

        private void toolStripMenuItemSaveAsEFFBLOB_Click(object sender, EventArgs e)
        {
            if (DataBase.FileEFF.Endian == Endianness.LittleEndian)
            {
                saveFileDialogEFFBLOB.FileName = Globals.FilePathEFFBLOB;
                saveFileDialogEFFBLOB.ShowDialog();
            }
            else
            {
                saveFileDialogEFFBLOBBIG.FileName = Globals.FilePathEFFBLOB;
                saveFileDialogEFFBLOBBIG.ShowDialog();
            }
        }

        private void saveFileDialogESL_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogESL.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileESL(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathESL = saveFileDialogESL.FileName;
                    saveFileDialogESL.FileName = null;
                }
            }
        }

        private void saveFileDialogETS_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogETS.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileETS(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathETS = saveFileDialogETS.FileName;
                    saveFileDialogETS.FileName = null;
                }
            }
        }

        private void saveFileDialogITA_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogITA.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileITA(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathITA = saveFileDialogITA.FileName;
                    saveFileDialogITA.FileName = null;
                }
            }
        }

        private void saveFileDialogAEV_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogAEV.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileAEV(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathAEV = saveFileDialogAEV.FileName;
                    saveFileDialogAEV.FileName = null;
                }
            }
        }

        private void saveFileDialogDSE_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogDSE.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileDSE(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathDSE = saveFileDialogDSE.FileName;
                    saveFileDialogDSE.FileName = null;
                }
            }
        }

        private void saveFileDialogFSE_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogFSE.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileFSE(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathFSE = saveFileDialogFSE.FileName;
                    saveFileDialogFSE.FileName = null;
                }
            }
        }

        private void saveFileDialogSAR_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogSAR.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {

                    FileManager.SaveFileSAR(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathSAR = saveFileDialogSAR.FileName;
                    saveFileDialogSAR.FileName = null;
                }

            }
        }

        private void saveFileDialogEAR_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogEAR.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileEAR(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathEAR = saveFileDialogEAR.FileName;
                    saveFileDialogEAR.FileName = null;
                }
            }
        }

        private void saveFileDialogEMI_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogEMI.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileEMI(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathEMI = saveFileDialogEMI.FileName;
                    saveFileDialogEMI.FileName = null;
                }
            }
        }

        private void saveFileDialogESE_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogESE.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileESE(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathESE = saveFileDialogESE.FileName;
                    saveFileDialogESE.FileName = null;
                }
            }
        }

        private void saveFileDialogQuadCustom_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogQuadCustom.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileQuadCustom(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathQuadCustom = saveFileDialogQuadCustom.FileName;
                    saveFileDialogQuadCustom.FileName = null;
                }
            }
        }

        private void saveFileDialogLIT_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogLIT.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileLIT(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathLIT = saveFileDialogLIT.FileName;
                    saveFileDialogLIT.FileName = null;
                }
            }
        }



        private void saveFileDialogEFFBLOB_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogEFFBLOB.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileEFFBLOB(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathEFFBLOB = saveFileDialogEFFBLOB.FileName;
                    saveFileDialogEFFBLOB.FileName = null;
                }
            }
        }

        private void saveFileDialogEFFBLOBBIG_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file;
            FileStream stream;
            try
            {
                file = new FileInfo(saveFileDialogEFFBLOBBIG.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                try
                {
                    FileManager.SaveFileEFFBLOB(stream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                    return;
                }
                finally
                {
                    stream.Close();
                    Globals.FilePathEFFBLOB = saveFileDialogEFFBLOBBIG.FileName;
                    saveFileDialogEFFBLOBBIG.FileName = null;
                }
            }
        }

        private void saveFileDialogCAM_FileOk(object sender, CancelEventArgs e)
        {

        }

        #endregion

        #region Gerenciamento de arquivos //Save

        private void toolStripMenuItemSave_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItemSaveESL.Enabled = DataBase.FileESL != null;
            toolStripMenuItemSaveETS.Enabled = DataBase.FileETS != null;
            toolStripMenuItemSaveITA.Enabled = DataBase.FileITA != null;
            toolStripMenuItemSaveAEV.Enabled = DataBase.FileAEV != null;
            toolStripMenuItemSaveDSE.Enabled = DataBase.FileDSE != null;
            toolStripMenuItemSaveFSE.Enabled = DataBase.FileFSE != null;
            toolStripMenuItemSaveSAR.Enabled = DataBase.FileSAR != null;
            toolStripMenuItemSaveEAR.Enabled = DataBase.FileEAR != null;
            toolStripMenuItemSaveEMI.Enabled = DataBase.FileEMI != null;
            toolStripMenuItemSaveESE.Enabled = DataBase.FileESE != null;
            toolStripMenuItemSaveLIT.Enabled = DataBase.FileLIT != null;
            toolStripMenuItemSaveQuadCustom.Enabled = DataBase.FileQuadCustom != null;
            toolStripMenuItemSaveEFFBLOB.Enabled = DataBase.FileEFF != null;

            if (DataBase.FileETS != null && DataBase.FileETS.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveETS_2007_PS2);
            }
            else if (DataBase.FileETS != null && DataBase.FileETS.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveETS_UHD);
            }
            else
            {
                toolStripMenuItemSaveETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveETS);
            }

            if (DataBase.FileITA != null && DataBase.FileITA.IsPs4Ns_Adapted)
            {
                toolStripMenuItemSaveITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveITA_PS4_NS);
            }
            else if (DataBase.FileITA != null && DataBase.FileITA.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveITA_2007_PS2);
            }
            else if (DataBase.FileITA != null && DataBase.FileITA.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveITA_UHD);
            }
            else
            {
                toolStripMenuItemSaveITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveITA);
            }

            if (DataBase.FileAEV != null && DataBase.FileAEV.IsPs4Ns_Adapted)
            {
                toolStripMenuItemSaveAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAEV_PS4_NS);
            }
            else if (DataBase.FileAEV != null && DataBase.FileAEV.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAEV_2007_PS2);
            }
            else if (DataBase.FileAEV != null && DataBase.FileAEV.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAEV_UHD);
            }
            else
            {
                toolStripMenuItemSaveAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAEV);
            }


            if (DataBase.FileEMI != null && DataBase.FileEMI.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveEMI_2007_PS2);
            }
            else if (DataBase.FileEMI != null && DataBase.FileEMI.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveEMI_UHD);
            }
            else
            {
                toolStripMenuItemSaveEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveEMI);
            }

            if (DataBase.FileESE != null && DataBase.FileESE.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveESE_2007_PS2);
            }
            else if (DataBase.FileESE != null && DataBase.FileESE.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveESE_UHD);
            }
            else
            {
                toolStripMenuItemSaveESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveESE);
            }

            if (DataBase.FileLIT != null && DataBase.FileLIT.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveLIT_2007_PS2);
            }
            else if (DataBase.FileLIT != null && DataBase.FileLIT.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveLIT_UHD);
            }
            else
            {
                toolStripMenuItemSaveLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveLIT);
            }

            if (DataBase.FileEFF != null && DataBase.FileEFF.Endian == Endianness.LittleEndian)
            {
                toolStripMenuItemSaveEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveEFFBLOB_LittleEndian);
            }
            else if (DataBase.FileEFF != null && DataBase.FileEFF.Endian == Endianness.BigEndian)
            {
                toolStripMenuItemSaveEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveEFFBLOB_BigEndian);
            }
            else
            {
                toolStripMenuItemSaveEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveEFFBLOB);
            }

        }

        #region EXPORT
        private void ExportFile(Func<object> getFileAction, ref string filePath, SaveFileDialog saveDialog, Action<FileStream> saveAction)
        {
            if (getFileAction() == null)
            {
                return;
            }

            string path = filePath;
            bool pathIsValid = !string.IsNullOrEmpty(path);

            if (!pathIsValid)
            {
                saveDialog.FileName = path;
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    path = saveDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                using (FileStream stream = new FileInfo(path).Create())
                {
                    saveAction(stream);
                }
                filePath = path;
                Editor.Console.Log($"File exported successfully: {Path.GetFileName(path)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
            }
        }


        private void ExportAllModifiedRoomFiles()
        {
            Editor.Console.Log("Exporting all modified room files...");

            if (DataBase.FileETS != null && !string.IsNullOrEmpty(Globals.FilePathETS)) { ExportETS(); }
            if (DataBase.FileITA != null && !string.IsNullOrEmpty(Globals.FilePathITA)) { ExportITA(); }
            if (DataBase.FileAEV != null && !string.IsNullOrEmpty(Globals.FilePathAEV)) { ExportAEV(); }
            if (DataBase.FileDSE != null && !string.IsNullOrEmpty(Globals.FilePathDSE)) { ExportDSE(); }
            if (DataBase.FileFSE != null && !string.IsNullOrEmpty(Globals.FilePathFSE)) { ExportFSE(); }
            if (DataBase.FileSAR != null && !string.IsNullOrEmpty(Globals.FilePathSAR)) { ExportSAR(); }
            if (DataBase.FileEAR != null && !string.IsNullOrEmpty(Globals.FilePathEAR)) { ExportEAR(); }
            if (DataBase.FileEMI != null && !string.IsNullOrEmpty(Globals.FilePathEMI)) { ExportEMI(); }
            if (DataBase.FileESE != null && !string.IsNullOrEmpty(Globals.FilePathESE)) { ExportESE(); }
            if (DataBase.FileLIT != null && !string.IsNullOrEmpty(Globals.FilePathLIT)) { ExportLIT(); }
            if (DataBase.FileEFF != null && !string.IsNullOrEmpty(Globals.FilePathEFFBLOB)) { ExportEFFBLOB(); }
            //ESL/quadcustom are not room specific, maybe later we can add smt to detect if esl/quad is related to current room
        }

        private void ExportESL() => ExportFile(() => DataBase.FileESL, ref Globals.FilePathESL, saveFileDialogESL, FileManager.SaveFileESL);
        private void ExportETS() => ExportFile(() => DataBase.FileETS, ref Globals.FilePathETS, saveFileDialogETS, FileManager.SaveFileETS);
        private void ExportITA() => ExportFile(() => DataBase.FileITA, ref Globals.FilePathITA, saveFileDialogITA, FileManager.SaveFileITA);
        private void ExportAEV() => ExportFile(() => DataBase.FileAEV, ref Globals.FilePathAEV, saveFileDialogAEV, FileManager.SaveFileAEV);
        private void ExportDSE() => ExportFile(() => DataBase.FileDSE, ref Globals.FilePathDSE, saveFileDialogDSE, FileManager.SaveFileDSE);
        private void ExportFSE() => ExportFile(() => DataBase.FileFSE, ref Globals.FilePathFSE, saveFileDialogFSE, FileManager.SaveFileFSE);
        private void ExportSAR() => ExportFile(() => DataBase.FileSAR, ref Globals.FilePathSAR, saveFileDialogSAR, FileManager.SaveFileSAR);
        private void ExportEAR() => ExportFile(() => DataBase.FileEAR, ref Globals.FilePathEAR, saveFileDialogEAR, FileManager.SaveFileEAR);
        private void ExportEMI() => ExportFile(() => DataBase.FileEMI, ref Globals.FilePathEMI, saveFileDialogEMI, FileManager.SaveFileEMI);
        private void ExportESE() => ExportFile(() => DataBase.FileESE, ref Globals.FilePathESE, saveFileDialogESE, FileManager.SaveFileESE);
        private void ExportLIT() => ExportFile(() => DataBase.FileLIT, ref Globals.FilePathLIT, saveFileDialogLIT, FileManager.SaveFileLIT);
        private void ExportQuadCustom() => ExportFile(() => DataBase.FileQuadCustom, ref Globals.FilePathQuadCustom, saveFileDialogQuadCustom, FileManager.SaveFileQuadCustom);
        private void ExportEFFBLOB()
        {
            if (DataBase.FileEFF == null) return;
            if (DataBase.FileEFF.Endian == Endianness.LittleEndian)
            {
                ExportFile(() => DataBase.FileEFF, ref Globals.FilePathEFFBLOB, saveFileDialogEFFBLOB, FileManager.SaveFileEFFBLOB);
            }
            else
            {
                ExportFile(() => DataBase.FileEFF, ref Globals.FilePathEFFBLOB, saveFileDialogEFFBLOBBIG, FileManager.SaveFileEFFBLOB);
            }
        }

        private void toolStripMenuItemSaveESL_Click(object sender, EventArgs e)
        {
            ExportESL();
        }

        private void toolStripMenuItemSaveETS_Click(object sender, EventArgs e)
        {
            ExportETS();
        }

        private void toolStripMenuItemSaveITA_Click(object sender, EventArgs e)
        {
            ExportITA();
        }

        private void toolStripMenuItemSaveAEV_Click(object sender, EventArgs e)
        {
            ExportAEV();
        }

        private void toolStripMenuItemSaveDSE_Click(object sender, EventArgs e)
        {
            ExportDSE();
        }

        private void toolStripMenuItemSaveFSE_Click(object sender, EventArgs e)
        {
            ExportFSE();
        }

        private void toolStripMenuItemSaveSAR_Click(object sender, EventArgs e)
        {
            ExportSAR();
        }

        private void toolStripMenuItemSaveEAR_Click(object sender, EventArgs e)
        {
            ExportEAR();
        }

        private void toolStripMenuItemSaveEMI_Click(object sender, EventArgs e)
        {
            ExportEMI();
        }

        private void toolStripMenuItemSaveESE_Click(object sender, EventArgs e)
        {
            ExportESE();
        }

        private void toolStripMenuItemSaveQuadCustom_Click(object sender, EventArgs e)
        {
            ExportQuadCustom();
        }

        private void toolStripMenuItemSaveLIT_Click(object sender, EventArgs e)
        {
            ExportLIT();
        }

        private void toolStripMenuItemSaveEFFBLOB_Click(object sender, EventArgs e)
        {
            ExportEFFBLOB();
        }

        #endregion


        private void toolStripMenuItemSaveDirectories_DropDownOpening(object sender, EventArgs e)
        {
            if (Path.GetExtension(Globals.FilePathEFFBLOB ?? "").ToUpperInvariant() == ".EFFBLOBBIG")
            {
                toolStripMenuItemDirectory_EFFBLOB.Text = Lang.GetText(eLang.DirectoryEFFBLOBBIG) + " " + (Globals.FilePathEFFBLOB ?? "");
            }
            else
            {
                toolStripMenuItemDirectory_EFFBLOB.Text = Lang.GetText(eLang.DirectoryEFFBLOB) + " " + (Globals.FilePathEFFBLOB ?? "");
            }
            toolStripMenuItemDirectory_ESL.Text = Lang.GetText(eLang.DirectoryESL) + " " + (Globals.FilePathESL ?? "");
            toolStripMenuItemDirectory_ETS.Text = Lang.GetText(eLang.DirectoryETS) + " " + (Globals.FilePathETS ?? "");
            toolStripMenuItemDirectory_ITA.Text = Lang.GetText(eLang.DirectoryITA) + " " + (Globals.FilePathITA ?? "");
            toolStripMenuItemDirectory_AEV.Text = Lang.GetText(eLang.DirectoryAEV) + " " + (Globals.FilePathAEV ?? "");
            toolStripMenuItemDirectory_DSE.Text = Lang.GetText(eLang.DirectoryDSE) + " " + (Globals.FilePathDSE ?? "");
            toolStripMenuItemDirectory_FSE.Text = Lang.GetText(eLang.DirectoryFSE) + " " + (Globals.FilePathFSE ?? "");
            toolStripMenuItemDirectory_SAR.Text = Lang.GetText(eLang.DirectorySAR) + " " + (Globals.FilePathSAR ?? "");
            toolStripMenuItemDirectory_EAR.Text = Lang.GetText(eLang.DirectoryEAR) + " " + (Globals.FilePathEAR ?? "");
            toolStripMenuItemDirectory_EMI.Text = Lang.GetText(eLang.DirectoryEMI) + " " + (Globals.FilePathEMI ?? "");
            toolStripMenuItemDirectory_ESE.Text = Lang.GetText(eLang.DirectoryESE) + " " + (Globals.FilePathESE ?? "");
            toolStripMenuItemDirectory_LIT.Text = Lang.GetText(eLang.DirectoryLIT) + " " + (Globals.FilePathLIT ?? "");
            toolStripMenuItemDirectory_QuadCustom.Text = Lang.GetText(eLang.DirectoryQuadCustom) + " " + (Globals.FilePathQuadCustom ?? "");
        }

        #endregion

        #region Gerenciamento de arquivos //Save Convert

        private void toolStripMenuItemSaveConverter_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuItemSaveConverterETS.Enabled = DataBase.FileETS != null;
            toolStripMenuItemSaveConverterITA.Enabled = DataBase.FileITA != null && DataBase.FileITA.IsPs4Ns_Adapted == false;
            toolStripMenuItemSaveConverterAEV.Enabled = DataBase.FileAEV != null && DataBase.FileAEV.IsPs4Ns_Adapted == false;
            toolStripMenuItemSaveConverterEMI.Enabled = DataBase.FileEMI != null;
            toolStripMenuItemSaveConverterESE.Enabled = DataBase.FileESE != null;

            if (DataBase.FileETS != null && DataBase.FileETS.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveConverterETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterETS_UHD);
            }
            else if (DataBase.FileETS != null && DataBase.FileETS.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveConverterETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterETS_2007_PS2);
            }
            else
            {
                toolStripMenuItemSaveConverterETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterETS);
            }

            if (DataBase.FileITA != null && DataBase.FileITA.GetRe4Version == Re4Version.V2007PS2 && DataBase.FileITA.IsPs4Ns_Adapted == false)
            {
                toolStripMenuItemSaveConverterITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterITA_UHD);
            }
            else if (DataBase.FileITA != null && DataBase.FileITA.GetRe4Version == Re4Version.UHD && DataBase.FileITA.IsPs4Ns_Adapted == false)
            {
                toolStripMenuItemSaveConverterITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterITA_2007_PS2);
            }
            else
            {
                toolStripMenuItemSaveConverterITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterITA);
            }

            if (DataBase.FileAEV != null && DataBase.FileAEV.GetRe4Version == Re4Version.V2007PS2 && DataBase.FileAEV.IsPs4Ns_Adapted == false)
            {
                toolStripMenuItemSaveConverterAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterAEV_UHD);
            }
            else if (DataBase.FileAEV != null && DataBase.FileAEV.GetRe4Version == Re4Version.UHD && DataBase.FileAEV.IsPs4Ns_Adapted == false)
            {
                toolStripMenuItemSaveConverterAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterAEV_2007_PS2);
            }
            else
            {
                toolStripMenuItemSaveConverterAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterAEV);
            }

            if (DataBase.FileEMI != null && DataBase.FileEMI.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveConverterEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterEMI_UHD);
            }
            else if (DataBase.FileEMI != null && DataBase.FileEMI.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveConverterEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterEMI_2007_PS2);
            }
            else
            {
                toolStripMenuItemSaveConverterEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterEMI);
            }

            if (DataBase.FileESE != null && DataBase.FileESE.GetRe4Version == Re4Version.V2007PS2)
            {
                toolStripMenuItemSaveConverterESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterESE_UHD);
            }
            else if (DataBase.FileESE != null && DataBase.FileESE.GetRe4Version == Re4Version.UHD)
            {
                toolStripMenuItemSaveConverterESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterESE_2007_PS2);
            }
            else
            {
                toolStripMenuItemSaveConverterESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterESE);
            }
        }

        private void toolStripMenuItemSaveConverterETS_Click(object sender, EventArgs e)
        {
            saveFileDialogConvertETS.FileName = null;
            saveFileDialogConvertETS.ShowDialog();
        }

        private void toolStripMenuItemSaveConverterITA_Click(object sender, EventArgs e)
        {
            saveFileDialogConvertITA.FileName = null;
            saveFileDialogConvertITA.ShowDialog();
        }

        private void toolStripMenuItemSaveConverterAEV_Click(object sender, EventArgs e)
        {
            saveFileDialogConvertAEV.FileName = null;
            saveFileDialogConvertAEV.ShowDialog();
        }

        private void toolStripMenuItemSaveConverterEMI_Click(object sender, EventArgs e)
        {
            saveFileDialogConvertEMI.FileName = null;
            saveFileDialogConvertEMI.ShowDialog();
        }

        private void toolStripMenuItemSaveConverterESE_Click(object sender, EventArgs e)
        {
            saveFileDialogConvertESE.FileName = null;
            saveFileDialogConvertESE.ShowDialog();
        }

        private void saveFileDialogConvertETS_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file = null;
            FileStream stream = null;
            try
            {
                file = new FileInfo(saveFileDialogConvertETS.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                FileManager.SaveConvertFileETS(stream);
                stream.Close();
            }
        }

        private void saveFileDialogConvertITA_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file = null;
            FileStream stream = null;
            try
            {
                file = new FileInfo(saveFileDialogConvertITA.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                FileManager.SaveConvertFileITA(stream);
                stream.Close();
            }
        }

        private void saveFileDialogConvertAEV_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file = null;
            FileStream stream = null;
            try
            {
                file = new FileInfo(saveFileDialogConvertAEV.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                FileManager.SaveConvertFileAEV(stream);
                stream.Close();
            }
        }

        private void saveFileDialogConvertEMI_FileOk(object sender, CancelEventArgs e)
        {
            FileInfo file = null;
            FileStream stream = null;
            try
            {
                file = new FileInfo(saveFileDialogConvertEMI.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                FileManager.SaveConvertFileEMI(stream);
                stream.Close();
            }
        }

        private void saveFileDialogConvertESE_FileOk(object sender, CancelEventArgs e)
        {

            FileInfo file = null;
            FileStream stream = null;
            try
            {
                file = new FileInfo(saveFileDialogConvertESE.FileName);
                stream = file.Create();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.GetText(eLang.MessageBoxErrorTitle), MessageBoxButtons.OK);
                e.Cancel = true;
                return;
            }

            if (file != null && stream != null)
            {
                FileManager.SaveConvertFileESE(stream);
                stream.Close();
            }
        }

        #region MAIN TOOLSTRIP

        private void PopulatePreferredVerDropdown() {
            toolstrip_preferredVer.Text = Globals.BackupConfigs.PreferredVersion.ToString();

            foreach (EditorRe4Ver version in Enum.GetValues(typeof(EditorRe4Ver))) {
                ToolStripMenuItem item = new ToolStripMenuItem(version.ToString());
                item.Tag = version; // Store the enum value itself
                item.Click += PreferredVer_Click;
                toolstrip_preferredVer.DropDownItems.Add(item);
            }
        }

        private void PreferredVer_Click(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem clickedItem && clickedItem.Tag is Editor.Class.Enums.EditorRe4Ver selectedVersion) {
                PreferredVerSet((int)selectedVersion);
            }
        }

        private void PreferredVerSet(int index) {
            Globals.PreferredVersion = (EditorRe4Ver)index;
            Globals.BackupConfigs.PreferredVersion = (EditorRe4Ver)index;
            toolstrip_preferredVer.Text = Globals.BackupConfigs.PreferredVersion.ToString();
        }

        private void toolstrip_playButton_Click(object sender, EventArgs e)
        {
            if (Globals.PreferredVersion == EditorRe4Ver.None) //NO VERSION SELECTED
            {
                Editor.Console.Error("No current preferred version selected. Set one in \"Settings > General\".");
            }
            if (Globals.PreferredVersion == EditorRe4Ver.UHD) //UHD LAUNCH
            {
                const string processName = "bio4";
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0)
                {
                    // If the process is already running, focus it.
                    IntPtr handle = processes[0].MainWindowHandle;
                    ShowWindow(handle, SW_RESTORE); // Restore if minimized.
                    SetForegroundWindow(handle);    // Bring to the front.
                    Editor.Console.Log("bio4.exe already running, focusing on game window.");
                }
                else
                {
                    // If not running, launch the game.
                    string filePath = Globals.DirectoryUHDRE4 + @"\Bin32\bio4.exe";

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            Process.Start(filePath);
                            Editor.Console.Log("Launching bio4.exe...");
                        }
                        catch (Exception ex)
                        {
                            Editor.Console.Error(ex.Message);
                        }
                    }
                    else
                    {
                        string warningLog = "The game executable path could not be found. Check the path in \"options > setup\" or change the current version.";
                        Editor.Console.Warning(warningLog);
                    }
                }
            }
            if (Globals.PreferredVersion == EditorRe4Ver.SourceNext2007)
            {
            }
        }

        #endregion

        #region Project Management

        private void toolstrip_saveEnv_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentProjectPath))
            {
                //force saveas
                SaveProjectAs();
            }
            else
            {
                SaveProject(currentProjectPath);
            }
        }

        private void toolstrip_openEnv_Click(object sender, EventArgs e)
        {
            //ask to save before opening a new proj
            if (isProjectEmpty() == false)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before opening a new project?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    toolstrip_saveEnv_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "QuadX Project (*.quadx)|*.quadx";
                openFileDialog.Title = "Open QuadX Project";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadProject(openFileDialog.FileName);
                }
            }
        }

        private void toolstrip_newEnv_Click(object sender, EventArgs e)
        {
            ResetEditorState();
            currentProjectPath = null;
            UpdateFormTitle();
        }


        private void LoadProject(string path)
        {
            try
            {
                ResetEditorState();

                string json = File.ReadAllText(path);
                var project = JsonConvert.DeserializeObject<ProjectFile>(json);

                if (project == null)throw new Exception("Failed to deserialize project file. It may be corrupt or in an old format.");

                ProjectManager.LoadProjectData(project);

                //load room logic here

                buildTreeView();

                currentProjectPath = path;
                UpdateFormTitle();
                UpdateTreeViewNodes();
                UpdateGL();
                Editor.Console.Log("Project loaded from: " + Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading project: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetEditorState();
                currentProjectPath = null;
                UpdateFormTitle();
            }
        }

        private void SaveProject(string path)
        {
            try
            {
                var project = new ProjectFile();

                project.FileESL = DataBase.FileESL;
                project.FileETS = DataBase.FileETS;
                project.FileITA = DataBase.FileITA;
                project.FileAEV = DataBase.FileAEV;
                project.FileDSE = DataBase.FileDSE;
                project.FileEMI = DataBase.FileEMI;
                project.FileSAR = DataBase.FileSAR;
                project.FileEAR = DataBase.FileEAR;
                project.FileESE = DataBase.FileESE;
                project.FileFSE = DataBase.FileFSE;
                project.FileLIT = DataBase.FileLIT;
                project.FileQuadCustom = DataBase.FileQuadCustom;
                project.FileEFF = DataBase.FileEFF;


                if (DataBase.SelectedRoom != null)
                    project.SelectedRoomID = DataBase.SelectedRoom.GetRoomId();


                //serialize the project object to JSON and write to file
                string json = JsonConvert.SerializeObject(project, Formatting.Indented);
                File.WriteAllText(path, json);

                currentProjectPath = path;
                UpdateFormTitle();
                Editor.Console.Log("Project saved to: " + Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving project: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveProjectAs()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "QuadX Project (*.quadx)|*.quadx";
                saveFileDialog.Title = "Save QuadX Project";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveProject(saveFileDialog.FileName);
                }
            }
        }

        private bool isProjectEmpty()
        {
            bool isEmpty = 
            DataBase.FileESL == null &&
            DataBase.FileETS == null &&
            DataBase.FileITA == null &&
            DataBase.FileAEV == null &&
            DataBase.FileDSE == null &&
            DataBase.FileFSE == null &&
            DataBase.FileSAR == null &&
            DataBase.FileEAR == null &&
            DataBase.FileEMI == null &&
            DataBase.FileESE == null && 
            DataBase.FileLIT == null &&
            DataBase.FileEFF == null &&
            DataBase.FileQuadCustom == null && 
            DataBase.SelectedRoom == null;

            return isEmpty;
        }

        #endregion

        private void treeView_addButton_Click(object sender, EventArgs e)
        {
            addNewObject();
        }

        #region viewport toolstrip
        private void viewportTools_tool_move_Click(object sender, EventArgs e)
        {
            selectTool(0);
        }

        private void viewportTools_tool_rotate_Click(object sender, EventArgs e)
        {
            selectTool(1);
        }

        private void selectTool(int tool = 0)
        {
            //uncheck all
            viewportTools_tool_move.Checked = false;
            viewportTools_tool_move.CheckState = CheckState.Unchecked;
            viewportTools_tool_rotate.Checked = false;
            viewportTools_tool_rotate.CheckState = CheckState.Unchecked;

            //select new tool
            if (tool == 0)
            {
                Globals.CurrentTool = EditorTool.Move;
                viewportTools_tool_move.Checked = true;
                viewportTools_tool_move.CheckState = CheckState.Checked;
                glControl.Invalidate(); // Redraw to show the correct gizmo
            }
            else if ( tool == 1)
            {
                Globals.CurrentTool = EditorTool.Rotate;
                viewportTools_tool_rotate.Checked = true;
                viewportTools_tool_rotate.CheckState = CheckState.Checked;
                glControl.Invalidate(); // Redraw to show the correct gizmo
            }
        }

        private void viewportTools_gizmospace_Click(object sender, EventArgs e)
        {
            toggleGizmospace();
        }

        private void toggleGizmospace()
        {
            //toggle between world/local
            if (Globals.CurrentGizmoSpace == GizmoSpace.World)
            {
                selectGizmospace(true);
            }
            else
            {
                selectGizmospace(false);
            }
        }

        private void selectGizmospace(bool localSpace)
        {
            if (localSpace == true){
                Globals.CurrentGizmoSpace = GizmoSpace.Local;
                viewportTools_gizmospace.Text = "Local";
            }else{
                Globals.CurrentGizmoSpace = GizmoSpace.World;
                viewportTools_gizmospace.Text = "World";
            }

            glControl.Invalidate();
        }

        private SkyComboBox gizmoMoveMode;
        private ToolStripControlHost gizmoMoveModeHost;
        private void GenerateViewportUtility()
        {
            //gizmo control
            ViewportGizmoDropdown objectControlDropdown = new ViewportGizmoDropdown(ref camera, objectControl, gizmo);
            objectControlDropdown.Margin = Padding.Empty;
            objectControlDropdown.Padding = Padding.Empty;

            ToolStripControlHost gizmoHost = new ToolStripControlHost(objectControlDropdown)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = objectControlDropdown.Size
            };

            ToolStripDropDown gizmoHostedDropDown = new ToolStripDropDown
            {
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoClose = true,
                AutoSize = false
            };
            gizmoHostedDropDown.Items.Add(gizmoHost);
            gizmoHostedDropDown.Size = objectControlDropdown.Size;

            //gizmo dropdown button
            ToolStripDropDownButton gizmoDropdown = new ToolStripDropDownButton("Gizmo");
            gizmoDropdown.Alignment = ToolStripItemAlignment.Right;
            gizmoDropdown.DisplayStyle = ToolStripItemDisplayStyle.Image;
            gizmoDropdown.Image = Properties.Resources.gizmo;
            gizmoDropdown.ToolTipText = "Gizmo Controls";
            gizmoDropdown.Width = 32;
            gizmoDropdown.AutoSize = false;

            //gizmo click event
            gizmoDropdown.Click += (s, e) => {
                var button = (ToolStripDropDownButton)s;
                ToolStrip parent = button.GetCurrentParent();
                Point screenPos = parent.PointToScreen(new Point(button.Bounds.Left, button.Bounds.Bottom));
                gizmoHostedDropDown.Show(screenPos);
            };

            viewportToolstrip.Items.Add(gizmoDropdown);

            //camera control
            ViewportCameraDropdown cameraControlDropdown = new ViewportCameraDropdown(ref camera, cameraControl);
            cameraControlDropdown.Margin = Padding.Empty;
            cameraControlDropdown.Padding = Padding.Empty;

            ToolStripControlHost cameraHost = new ToolStripControlHost(cameraControlDropdown)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = cameraControlDropdown.Size
            };

            ToolStripDropDown cameraHostedDropDown = new ToolStripDropDown
            {
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoClose = true,
                AutoSize = false
            };
            cameraHostedDropDown.Items.Add(cameraHost);
            cameraHostedDropDown.Size = cameraControlDropdown.Size;

            //camera dropdown
            ToolStripDropDownButton cameraDropdown = new ToolStripDropDownButton("Camera");
            cameraDropdown.Alignment = ToolStripItemAlignment.Right;
            cameraDropdown.DisplayStyle = ToolStripItemDisplayStyle.Image;
            cameraDropdown.Image = Properties.Resources.cam;
            cameraDropdown.ToolTipText = "Camera Controls";
            cameraDropdown.Width = 32;
            cameraDropdown.AutoSize = false;

            //camera click
            cameraDropdown.Click += (s, e) =>
            {
                cameraControlDropdown.UpdateLanguage();
                var button = (ToolStripDropDownButton)s;
                ToolStrip parent = button.GetCurrentParent();
                Point screenPos = parent.PointToScreen(new Point(button.Bounds.Left, button.Bounds.Bottom));
                cameraHostedDropDown.Show(screenPos);
            };

            viewportToolstrip.Items.Add(cameraDropdown);


            //combobox
            gizmoMoveMode = new SkyComboBox
            {
                BackColor = SystemColors.Control,
                ForeColor = SystemColors.GrayText,
                ListForeColor = SystemColors.GrayText,
                Width = viewportToolstrip.Width / 4,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor = Cursors.Default,
                ItemHeight = 16,
                IntegralHeight = false,
                TriangleColorA = Color.Black,
                TriangleColorB = Color.Black
            };
            gizmoMoveMode.SelectedIndexChanged += gizmoMoveMode_SelectedIndexChanged;

            gizmoMoveModeHost = new ToolStripControlHost(gizmoMoveMode)
            {
                Alignment = ToolStripItemAlignment.Right,
                Margin = new System.Windows.Forms.Padding(0, 0, 5, 0)
            };

            if (Globals.BackupConfigs.SelectedTheme != EditorTheme.Light) ThemeManager.ApplyTheme(gizmoMoveMode);

            viewportToolstrip.Items.Add(gizmoMoveModeHost);
        }

        private void gizmoMoveMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //keep selection updated in legacy glcontrols
            if (gizmoMoveMode.SelectedItem is MoveObjTypeObjForListBox obj){
                objectControl.SetSelectedMoveMode(obj.ID);
                objectMove.UpdateSelection();
            }

            if (gizmoMoveMode.SelectedItem is ManipulationTarget target)
            {
                objectControl.SetSelectedManipulationTarget(target);
            }
        }

        private void UpdateAllMoveControls()
        {
            var availableTargets = objectControl.GetManipulationTargets();

            gizmoMoveMode.SelectedIndexChanged -= gizmoMoveMode_SelectedIndexChanged;
            gizmoMoveMode.DataSource = null;
            gizmoMoveMode.DataSource = availableTargets;
            gizmoMoveMode.DisplayMember = "DisplayName";

            //reselect the previous target if it still exists
            var currentTarget = objectControl.SelectedTarget;
            int indexToSelect = -1;
            if (currentTarget != null)
            {
                indexToSelect = availableTargets.FindIndex(t => t.Type == currentTarget.Type);
            }

            //if previous target doest exist in the new list, it will return "-1"
            if (indexToSelect == -1){
                indexToSelect = 0;
            }

            gizmoMoveMode.SelectedIndex = indexToSelect;

            //update the objectcontrol state with the target we just selected
            if (availableTargets.Count > 0 && indexToSelect >= 0){
                objectControl.SetSelectedManipulationTarget(availableTargets[indexToSelect]);
            }

            gizmoMoveMode.SelectedIndexChanged += gizmoMoveMode_SelectedIndexChanged;

            //legacy
            objectMove.UpdateSelection();
        }


        #endregion

        private void runSetupWizardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupWizard form = new SetupWizard();
            form.ShowDialog();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEditorState();
            //create new project here
        }

        private void openRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectRoomWindow();
        }

        private void toolstrip_openRoom_Click(object sender, EventArgs e)
        {
            SelectRoomWindow();
        }

        private void toolstrip_clearRoom_Click(object sender, EventArgs e)
        {
            if (DataBase.SelectedRoom == null) return;

            DialogResult result = MessageBox.Show(
                "Are you sure clear current room?",
                "Clear room?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes) { 
                Editor.Console.Log($"Clearing previously loaded room: {DataBase.SelectedRoom.GetRoomModel().Description} (ID: {DataBase.SelectedRoom.GetRoomModel().HexID})");
                currentRoomLabelToggle(false);
                DataBase.SelectedRoom.ClearGL();
                DataBase.SelectedRoom = null;
                GC.Collect();
                UpdateGL();
            }
        }

        private void clearEverythingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearAllObjects();
            UpdateGL();
        }

        #region tool buttons

        private void repackRoom_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                $"Do you want to repack room {currentRoomLabel.Text} with all existent files?",
                "Repack Room",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ExportAllModifiedRoomFiles();
                ExternalToolManager.RepackCurrentRoomUdas();
            }
        }
        #endregion


#endregion

        #region open root folders

        private void openEditorRoot_Click(object sender, EventArgs e){
            string path = AppContext.BaseDirectory;
            OpenExplorer(path, "Editor Root");
        }

        private void openRE4root_Click(object sender, EventArgs e){
            string path = GetRe4RootPath();
            OpenExplorer(path, "RE4 Game Root");
        }

        private void openRoomRoot_Click(object sender, EventArgs e)
        {
            if (DataBase.SelectedRoom == null) {
                Editor.Console.Warning("Selected room is null.");
                return;
            }

            string path = Utils.GetCurrentRoomDirectory();
            OpenExplorer(path, "Room Root");
        }

        private string GetRe4RootPath(){
            switch (Globals.PreferredVersion){
                case Editor.Class.Enums.EditorRe4Ver.UHD:
                    return Globals.DirectoryUHDRE4;
                case Editor.Class.Enums.EditorRe4Ver.SourceNext2007:
                    return Globals.Directory2007RE4;
                case Editor.Class.Enums.EditorRe4Ver.PS2:
                    return Globals.DirectoryPS2RE4;
                case Editor.Class.Enums.EditorRe4Ver.PS4NS:
                    return Globals.DirectoryPS4NSRE4;
                default:
                    return null;
            }
        }

        private void OpenExplorer(string path, string locationName){
            if (string.IsNullOrEmpty(path)){
                Editor.Console.Log(locationName + " directory is not set. Please check settings.");
                return;
            }
            if (!Directory.Exists(path)){
                Editor.Console.Log(locationName + " directory does not exist or the path is invalid: " + path);
                return;
            }

            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                Editor.Console.Error("Failed to open File Explorer for " + locationName + ". Error: " + ex.Message);
            }
        }

        private void unpackAllRoomsUHDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool deleteLFS = false;

            if (Globals.BackupConfigs.PreferredVersion == EditorRe4Ver.UHD || Globals.BackupConfigs.PreferredVersion == EditorRe4Ver.PS4NS)
            {
                var result = MessageBox.Show(
                    "Would you like to delete original '.lfs' compressed files and keep only new uncompressed '.udas'?\n\n(Keeping original will significantly fill disk space)",
                    "Delete uncompressed .lfs files?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    deleteLFS = true;
            }

            ExternalToolManager.UnpackAllRoomsUdas(deleteLFS);
        }


        private void unpackAllTexturesUHDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
            "Choose the unpack mode:\n\nYes = ImagePackHD\nNo = ImagePack (SD)",
            "Select Unpack Mode",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

            string imagepackType = null;

            if (result == DialogResult.Yes){
                imagepackType = "ImagePackHD";
            }else if (result == DialogResult.No){
                imagepackType = "ImagePack";
            }
            ExternalToolManager.UnpackAllPacks(imagepackType);
        }

        #endregion

        #region Property Grid Toolstrip
        private void propertyGridButton_Categorized_Click(object sender, EventArgs e)
        {
            propertyGridButton_Categorized.Checked = true;
            propertyGridButton_Alphabetical.Checked = false;

            UpdatePropertyGridDisplay();
        }

        private void propertyGridButton_Alphabetical_Click(object sender, EventArgs e)
        {
            propertyGridButton_Alphabetical.Checked = true;
            propertyGridButton_Categorized.Checked = false;

            UpdatePropertyGridDisplay();
        }
        private void propertyGridButton_ShowAll_Click(object sender, EventArgs e)
        {
            propertyGridButton_ShowAll.Checked = !propertyGridButton_ShowAll.Checked;
            UpdatePropertyGridDisplay();
        }


        private void UpdatePropertyGridDisplay()
        {
            if (propertyGridOriginalObj == null){
                propertyGridObjs.SelectedObject = null;
                return;
            }

            object objectToDisplay = propertyGridOriginalObj;

            //filter curated properties
            if (!propertyGridButton_ShowAll.Checked && !(objectToDisplay is NoneProperty))
            {
                objectToDisplay = new PropertyFilterDescriptor(objectToDisplay);
            }

            //order filter
            propertyGridObjs.PropertySort = propertyGridButton_Alphabetical.Checked
                ? PropertySort.Alphabetical
                : PropertySort.Categorized;

            //search field text filter
            string filterText = propertyGrid_searchField.Text;
            if (!string.IsNullOrWhiteSpace(filterText))
            {
                propertyGridObjs.SelectedObject = new PropertyFilter(objectToDisplay, filterText);
            }
            else
            {
                propertyGridObjs.SelectedObject = objectToDisplay;
            }
        }

        private void propertyGridObjs_PropertySortChanged(object sender, EventArgs e)
        {
            var sort = propertyGridObjs.PropertySort;
            propertyGridButton_Categorized.Checked = (sort == PropertySort.Categorized || sort == PropertySort.CategorizedAlphabetical);
            propertyGridButton_Alphabetical.Checked = (sort == PropertySort.Alphabetical);
        }

        private void propertyGridObjs_Enter(object sender, EventArgs e)
        {
            InPropertyGrid = true;
        }

        private void propertyGridObjs_Leave(object sender, EventArgs e)
        {
            InPropertyGrid = false;
        }

        private void propertyGridObjs_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGridObjs.Refresh();
            treeViewObjs.Refresh();
        }

        private void support3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWebsite("https://jaderlink.github.io/Donate/");
        }


        #endregion

        #region MainForm events/ metodos

        public void OpenWebsite(string url){
            try{
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Editor.Console.Error($"Failed to open website: {ex.Message}");
            }
        }

        private void splitContainerRight_Panel2_Resize(object sender, EventArgs e)
        {
            int painel2Width = glControls_old.Width;
            int quite = painel2Width / 2;
        }

        public void ApplyTheme()
        {
            if (Globals.BackupConfigs.SelectedTheme == EditorTheme.Light){
                return;
            }


            ThemeManager.ApplyTheme(this);
            ThemeManager.ApplyTheme(menuStripMenu);
            ThemeManager.ApplyTheme(toolStripEditor);
            ThemeManager.ApplyTheme(viewportToolstrip);
            ThemeManager.ApplyTheme(editor);
            ThemeManager.ApplyTheme(treeViewLabel);
            ThemeManager.ApplyTheme(treeView_searchBar);
            ThemeManager.ApplyTheme(treeView_searchField);
            ThemeManager.ApplyTheme(treeViewObjs);
            ThemeManager.ApplyTheme(propertyGridLabel);
            ThemeManager.ApplyTheme(propertyGrid_searchBar);
            ThemeManager.ApplyTheme(propertyGrid_searchField);
            ThemeManager.ApplyTheme(propertyGridObjs);
            ThemeManager.ApplyTheme(splitContainerMain);
            ThemeManager.ApplyTheme(glViewport);
            ThemeManager.ApplyTheme(splitContainerRight);
            ThemeManager.ApplyTheme(splitContainerLeft);
            ThemeManager.ApplyTheme(utilityPanel);
            ThemeManager.ApplyTheme(consoleBox);
            ThemeManager.ApplyTheme(cameraMove);
            ThemeManager.ApplyTheme(objectMove);
            ThemeManager.ApplyTheme(glControls_old);

            //specific coloring
            ColorPalette palette = ThemeManager.GetCurrentPalette();

            //remove toolstrip weird white line
            toolStripEditor.Renderer = new ThemeManager.CustomToolStripRenderer(palette);
            viewportToolstrip.Renderer = new ThemeManager.CustomToolStripRenderer(palette);
            treeView_addToolstrip.Renderer = new ThemeManager.CustomToolStripRenderer(palette);
            toolStrip_propertyGrid.Renderer = new ThemeManager.CustomToolStripRenderer(palette);
            viewportToolstrip.BackColor = palette.Border;
            splitContainerLeft.BackColor = palette.Border;
            treeView_searchBar.BackColor = palette.BackgroundDarker;
            treeView_searchField.BackColor = palette.BackgroundDarker;
            propertyGrid_searchBar.BackColor = palette.BackgroundDarker;
            propertyGrid_searchField.BackColor = palette.BackgroundDarker;
            toolStripContainer1.TopToolStripPanel.BackColor = palette.BackgroundDarker;
            Globals.NodeColorEntry = palette.Text;
        }


        private void StartUpdateTranslation()
        {
            // menu principal
            toolStripMenuItemFile.Text = Lang.GetText(eLang.toolStripMenuItemFile);
            toolStripMenuItemEdit.Text = Lang.GetText(eLang.toolStripMenuItemEdit);
            toolStripMenuItemView.Text = Lang.GetText(eLang.toolStripMenuItemView);
            toolStripMenuItemMisc.Text = Lang.GetText(eLang.toolStripMenuItemMisc);
            //submenu File
            toolStripMenuItemNewFile.Text = Lang.GetText(eLang.toolStripMenuItemNewFile);
            toolStripMenuItemOpen.Text = Lang.GetText(eLang.toolStripMenuItemOpen);
            toolStripMenuItemSave.Text = Lang.GetText(eLang.toolStripMenuItemSave);
            toolStripMenuItemSaveAs.Text = Lang.GetText(eLang.toolStripMenuItemSaveAs);
            toolStripMenuItemSaveConverter.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverter);
            toolStripMenuItemClear.Text = Lang.GetText(eLang.toolStripMenuItemClear);
            toolStripMenuItemClose.Text = Lang.GetText(eLang.toolStripMenuItemClose);
            
            // subsubmenu New
            toolStripMenuItemNewESL.Text = Lang.GetText(eLang.toolStripMenuItemNewESL);
            toolStripMenuItemNewETS_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemNewETS_2007_PS2);
            toolStripMenuItemNewITA_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemNewITA_2007_PS2);
            toolStripMenuItemNewAEV_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemNewAEV_2007_PS2);
            toolStripMenuItemNewETS_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemNewETS_UHD_PS4NS);
            toolStripMenuItemNewITA_UHD.Text = Lang.GetText(eLang.toolStripMenuItemNewITA_UHD);
            toolStripMenuItemNewAEV_UHD.Text = Lang.GetText(eLang.toolStripMenuItemNewAEV_UHD);
            toolStripMenuItemNewDSE.Text = Lang.GetText(eLang.toolStripMenuItemNewDSE);
            toolStripMenuItemNewFSE.Text = Lang.GetText(eLang.toolStripMenuItemNewFSE);
            toolStripMenuItemNewSAR.Text = Lang.GetText(eLang.toolStripMenuItemNewSAR);
            toolStripMenuItemNewEAR.Text = Lang.GetText(eLang.toolStripMenuItemNewEAR);
            toolStripMenuItemNewEMI_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemNewEMI_2007_PS2);
            toolStripMenuItemNewESE_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemNewESE_2007_PS2);
            toolStripMenuItemNewEMI_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemNewEMI_UHD_PS4NS);
            toolStripMenuItemNewESE_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemNewESE_UHD_PS4NS);
            toolStripMenuItemNewQuadCustom.Text = Lang.GetText(eLang.toolStripMenuItemNewQuadCustom);
            toolStripMenuItemNewITA_PS4_NS.Text = Lang.GetText(eLang.toolStripMenuItemNewITA_PS4_NS);
            toolStripMenuItemNewAEV_PS4_NS.Text = Lang.GetText(eLang.toolStripMenuItemNewAEV_PS4_NS);
            toolStripMenuItemNewLIT_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemNewLIT_2007_PS2);
            toolStripMenuItemNewLIT_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemNewLIT_UHD_PS4NS);
            toolStripMenuItemNewEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemNewEFFBLOB);
            toolStripMenuItemNewBigEndianFiles.Text = Lang.GetText(eLang.toolStripMenuItemNewBigEndianFiles);
            toolStripMenuItemNewEFFBLOBBIG.Text = Lang.GetText(eLang.toolStripMenuItemNewEFFBLOBBIG);
            // subsubmenu Open
            toolStripMenuItemOpenESL.Text = Lang.GetText(eLang.toolStripMenuItemOpenESL);
            toolStripMenuItemOpenETS_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemOpenETS_2007_PS2);
            toolStripMenuItemOpenITA_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemOpenITA_2007_PS2);
            toolStripMenuItemOpenAEV_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemOpenAEV_2007_PS2);
            toolStripMenuItemOpenETS_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemOpenETS_UHD_PS4NS);
            toolStripMenuItemOpenITA_UHD.Text = Lang.GetText(eLang.toolStripMenuItemOpenITA_UHD);
            toolStripMenuItemOpenAEV_UHD.Text = Lang.GetText(eLang.toolStripMenuItemOpenAEV_UHD);
            toolStripMenuItemOpenDSE.Text = Lang.GetText(eLang.toolStripMenuItemOpenDSE);
            toolStripMenuItemOpenFSE.Text = Lang.GetText(eLang.toolStripMenuItemOpenFSE);
            toolStripMenuItemOpenSAR.Text = Lang.GetText(eLang.toolStripMenuItemOpenSAR);
            toolStripMenuItemOpenEAR.Text = Lang.GetText(eLang.toolStripMenuItemOpenEAR);
            toolStripMenuItemOpenEMI_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemOpenEMI_2007_PS2);
            toolStripMenuItemOpenESE_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemOpenESE_2007_PS2);
            toolStripMenuItemOpenEMI_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemOpenEMI_UHD_PS4NS);
            toolStripMenuItemOpenESE_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemOpenESE_UHD_PS4NS);
            toolStripMenuItemOpenQuadCustom.Text = Lang.GetText(eLang.toolStripMenuItemOpenQuadCustom);
            toolStripMenuItemOpenITA_PS4_NS.Text = Lang.GetText(eLang.toolStripMenuItemOpenITA_PS4_NS);
            toolStripMenuItemOpenAEV_PS4_NS.Text = Lang.GetText(eLang.toolStripMenuItemOpenAEV_PS4_NS);
            toolStripMenuItemOpenLIT_2007_PS2.Text = Lang.GetText(eLang.toolStripMenuItemOpenLIT_2007_PS2);
            toolStripMenuItemOpenLIT_UHD_PS4NS.Text = Lang.GetText(eLang.toolStripMenuItemOpenLIT_UHD_PS4NS);
            toolStripMenuItemOpenEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemOpenEFFBLOB);
            toolStripMenuItemOpenBigEndianFiles.Text = Lang.GetText(eLang.toolStripMenuItemOpenBigEndianFiles);
            toolStripMenuItemOpenEFFBLOBBIG.Text = Lang.GetText(eLang.toolStripMenuItemOpenEFFBLOBBIG);           
            // subsubmenu Save
            toolStripMenuItemSaveESL.Text = Lang.GetText(eLang.toolStripMenuItemSaveESL);
            toolStripMenuItemSaveETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveETS);
            toolStripMenuItemSaveITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveITA);
            toolStripMenuItemSaveAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAEV);
            toolStripMenuItemSaveEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveEMI);
            toolStripMenuItemSaveESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveESE);
            toolStripMenuItemSaveDSE.Text = Lang.GetText(eLang.toolStripMenuItemSaveDSE);
            toolStripMenuItemSaveFSE.Text = Lang.GetText(eLang.toolStripMenuItemSaveFSE);
            toolStripMenuItemSaveSAR.Text = Lang.GetText(eLang.toolStripMenuItemSaveSAR);
            toolStripMenuItemSaveEAR.Text = Lang.GetText(eLang.toolStripMenuItemSaveEAR);
            toolStripMenuItemSaveLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveLIT);
            toolStripMenuItemSaveEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveEFFBLOB);
            toolStripMenuItemSaveQuadCustom.Text = Lang.GetText(eLang.toolStripMenuItemSaveQuadCustom);
            toolStripMenuItemSaveDirectories.Text = Lang.GetText(eLang.toolStripMenuItemSaveDirectories);
            // subsubmenu Save As...
            toolStripMenuItemSaveAsESL.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsESL);
            toolStripMenuItemSaveAsETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsETS);
            toolStripMenuItemSaveAsITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsITA);
            toolStripMenuItemSaveAsAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsAEV);
            toolStripMenuItemSaveAsEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEMI);
            toolStripMenuItemSaveAsESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsESE);
            toolStripMenuItemSaveAsDSE.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsDSE);
            toolStripMenuItemSaveAsFSE.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsFSE);
            toolStripMenuItemSaveAsSAR.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsSAR);
            toolStripMenuItemSaveAsEAR.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEAR);
            toolStripMenuItemSaveAsLIT.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsLIT);
            toolStripMenuItemSaveAsEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsEFFBLOB);
            toolStripMenuItemSaveAsQuadCustom.Text = Lang.GetText(eLang.toolStripMenuItemSaveAsQuadCustom);
            // subsubmenu Save As (Convert)
            toolStripMenuItemSaveConverterETS.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterETS);
            toolStripMenuItemSaveConverterITA.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterITA);
            toolStripMenuItemSaveConverterAEV.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterAEV);
            toolStripMenuItemSaveConverterEMI.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterEMI);
            toolStripMenuItemSaveConverterESE.Text = Lang.GetText(eLang.toolStripMenuItemSaveConverterESE);
            // subsubmenu Clear
            toolStripMenuItemClearESL.Text = Lang.GetText(eLang.toolStripMenuItemClearESL);
            toolStripMenuItemClearETS.Text = Lang.GetText(eLang.toolStripMenuItemClearETS);
            toolStripMenuItemClearITA.Text = Lang.GetText(eLang.toolStripMenuItemClearITA);
            toolStripMenuItemClearAEV.Text = Lang.GetText(eLang.toolStripMenuItemClearAEV);
            toolStripMenuItemClearDSE.Text = Lang.GetText(eLang.toolStripMenuItemClearDSE);
            toolStripMenuItemClearFSE.Text = Lang.GetText(eLang.toolStripMenuItemClearFSE);
            toolStripMenuItemClearSAR.Text = Lang.GetText(eLang.toolStripMenuItemClearSAR);
            toolStripMenuItemClearEAR.Text = Lang.GetText(eLang.toolStripMenuItemClearEAR);
            toolStripMenuItemClearEMI.Text = Lang.GetText(eLang.toolStripMenuItemClearEMI);
            toolStripMenuItemClearESE.Text = Lang.GetText(eLang.toolStripMenuItemClearESE);
            toolStripMenuItemClearLIT.Text = Lang.GetText(eLang.toolStripMenuItemClearLIT);
            toolStripMenuItemClearEFFBLOB.Text = Lang.GetText(eLang.toolStripMenuItemClearEFFBLOB);
            toolStripMenuItemClearQuadCustom.Text = Lang.GetText(eLang.toolStripMenuItemClearQuadCustom);

            // sub menu edit
            toolStripMenuItemAddNewObj.Text = Lang.GetText(eLang.toolStripMenuItemAddNewObj);
            toolStripMenuItemDeleteSelectedObj.Text = Lang.GetText(eLang.toolStripMenuItemDeleteSelectedObj);
            toolStripMenuItemMoveUp.Text = Lang.GetText(eLang.toolStripMenuItemMoveUp);
            toolStripMenuItemMoveDown.Text = Lang.GetText(eLang.toolStripMenuItemMoveDown);
            toolStripMenuItemSearch.Text = Lang.GetText(eLang.toolStripMenuItemSearch);

            // sub menu Misc
            toolStripMenuItemOptions.Text = Lang.GetText(eLang.toolStripMenuItemOptions);
            toolStripMenuItemCredits.Text = Lang.GetText(eLang.toolStripMenuItemCredits);

            // sub menu View
            toolStripMenuItemSubMenuHide.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuHide);
            toolStripMenuItemSubMenuRoom.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuRoom);
            toolStripMenuItemSubMenuModels.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuModels);
            toolStripMenuItemSubMenuEnemy.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuEnemy);
            toolStripMenuItemSubMenuItem.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuItem);
            toolStripMenuItemSubMenuSpecial.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuSpecial);
            toolStripMenuItemSubMenuEtcModel.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuEtcModel);
            toolStripMenuItemSubMenuLight.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuLight);
            toolStripMenuItemSubMenuEffect.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuEffect);
            toolStripMenuItemNodeDisplayNameInHex.Text = Lang.GetText(eLang.toolStripMenuItemNodeDisplayNameInHex);
            toolStripMenuItemCameraMenu.Text = Lang.GetText(eLang.toolStripMenuItemCameraMenu);
            toolStripMenuItemResetCamera.Text = Lang.GetText(eLang.toolStripMenuItemResetCamera);
            toolStripMenuItemRefresh.Text = Lang.GetText(eLang.toolStripMenuItemRefresh);

            //sub menu hide
            toolStripMenuItemHideRoomModel.Text = Lang.GetText(eLang.toolStripMenuItemHideRoomModel);
            toolStripMenuItemHideEnemyESL.Text = Lang.GetText(eLang.toolStripMenuItemHideEnemyESL);
            toolStripMenuItemHideEtcmodelETS.Text = Lang.GetText(eLang.toolStripMenuItemHideEtcmodelETS);
            toolStripMenuItemHideItemsITA.Text = Lang.GetText(eLang.toolStripMenuItemHideItemsITA);
            toolStripMenuItemHideEventsAEV.Text = Lang.GetText(eLang.toolStripMenuItemHideEventsAEV);
            toolStripMenuItemHideLateralMenu.Text = Lang.GetText(eLang.toolStripMenuItemHideLateralMenu);
            toolStripMenuItemHideBottomMenu.Text = Lang.GetText(eLang.toolStripMenuItemHideBottomMenu);
            toolStripMenuItemHideFileFSE.Text = Lang.GetText(eLang.toolStripMenuItemHideFileFSE);
            toolStripMenuItemHideFileSAR.Text = Lang.GetText(eLang.toolStripMenuItemHideFileSAR);
            toolStripMenuItemHideFileEAR.Text = Lang.GetText(eLang.toolStripMenuItemHideFileEAR);
            toolStripMenuItemHideFileESE.Text = Lang.GetText(eLang.toolStripMenuItemHideFileESE);
            toolStripMenuItemHideFileEMI.Text = Lang.GetText(eLang.toolStripMenuItemHideFileEMI);
            toolStripMenuItemHideFileLIT.Text = Lang.GetText(eLang.toolStripMenuItemHideFileLIT);
            toolStripMenuItemHideFileEFF.Text = Lang.GetText(eLang.toolStripMenuItemHideFileEFF);
            toolStripMenuItemHideQuadCustom.Text = Lang.GetText(eLang.toolStripMenuItemHideQuadCustom);

            // sub menus de view
            toolStripMenuItemHideDesabledEnemy.Text = Lang.GetText(eLang.toolStripMenuItemHideDesabledEnemy);
            toolStripMenuItemShowOnlyDefinedRoom.Text = Lang.GetText(eLang.toolStripMenuItemShowOnlyDefinedRoom);
            toolStripMenuItemAutoDefineRoom.Text = Lang.GetText(eLang.toolStripMenuItemAutoDefineRoom);
            toolStripMenuItemItemPositionAtAssociatedObjectLocation.Text = Lang.GetText(eLang.toolStripMenuItemItemPositionAtAssociatedObjectLocation);
            toolStripMenuItemHideItemTriggerZone.Text = Lang.GetText(eLang.toolStripMenuItemHideItemTriggerZone);
            toolStripMenuItemHideItemTriggerRadius.Text = Lang.GetText(eLang.toolStripMenuItemHideItemTriggerRadius);
            toolStripMenuItemHideSpecialTriggerZone.Text = Lang.GetText(eLang.toolStripMenuItemHideSpecialTriggerZone);
            toolStripMenuItemHideExtraObjs.Text = Lang.GetText(eLang.toolStripMenuItemHideExtraObjs);
            toolStripMenuItemHideOnlyWarpDoor.Text = Lang.GetText(eLang.toolStripMenuItemHideOnlyWarpDoor);
            toolStripMenuItemHideExtraExceptWarpDoor.Text = Lang.GetText(eLang.toolStripMenuItemHideExtraExceptWarpDoor);
            toolStripMenuItemUseMoreSpecialColors.Text = Lang.GetText(eLang.toolStripMenuItemUseMoreSpecialColors);
            toolStripMenuItemEtcModelUseScale.Text = Lang.GetText(eLang.toolStripMenuItemEtcModelUseScale);
            toolStripMenuItemSubMenuQuadCustom.Text = Lang.GetText(eLang.toolStripMenuItemSubMenuQuadCustom);
            toolStripMenuItemUseCustomColors.Text = Lang.GetText(eLang.toolStripMenuItemUseCustomColors);
            toolStripMenuItemShowOnlySelectedGroup.Text = Lang.GetText(eLang.toolStripMenuItemShowOnlySelectedGroup);
            toolStripMenuItemSelectedGroupUp.Text = Lang.GetText(eLang.toolStripMenuItemSelectedGroupUp);
            toolStripMenuItemSelectedGroupDown.Text = Lang.GetText(eLang.toolStripMenuItemSelectedGroupDown);
            toolStripMenuItemEnableLightColor.Text = Lang.GetText(eLang.toolStripMenuItemEnableLightColor);
            toolStripMenuItemShowOnlySelectedGroup_EFF.Text = Lang.GetText(eLang.toolStripMenuItemShowOnlySelectedGroup_EFF);
            toolStripMenuItemSelectedGroupUp_EFF.Text = Lang.GetText(eLang.toolStripMenuItemSelectedGroupUp_EFF);
            toolStripMenuItemSelectedGroupDown_EFF.Text = Lang.GetText(eLang.toolStripMenuItemSelectedGroupDown_EFF);
            toolStripMenuItemHideTable7_EFF.Text = Lang.GetText(eLang.toolStripMenuItemHideTable7_EFF);
            toolStripMenuItemHideTable8_EFF.Text = Lang.GetText(eLang.toolStripMenuItemHideTable8_EFF);
            toolStripMenuItemHideTable9_EFF.Text = Lang.GetText(eLang.toolStripMenuItemHideTable9_EFF);
            toolStripMenuItemDisableGroupPositionEFF.Text = Lang.GetText(eLang.toolStripMenuItemDisableGroupPositionEFF);

            //sub menu de view room and model
            toolStripMenuItemModelsHideTextures.Text = Lang.GetText(eLang.toolStripMenuItemModelsHideTextures);
            toolStripMenuItemModelsWireframe.Text = Lang.GetText(eLang.toolStripMenuItemModelsWireframe);
            toolStripMenuItemModelsRenderNormals.Text = Lang.GetText(eLang.toolStripMenuItemModelsRenderNormals);
            toolStripMenuItemModelsOnlyFrontFace.Text = Lang.GetText(eLang.toolStripMenuItemModelsOnlyFrontFace);
            toolStripMenuItemModelsVertexColor.Text = Lang.GetText(eLang.toolStripMenuItemModelsVertexColor);
            toolStripMenuItemModelsAlphaChannel.Text = Lang.GetText(eLang.toolStripMenuItemModelsAlphaChannel);
            toolStripMenuItemRoomHideTextures.Text = Lang.GetText(eLang.toolStripMenuItemRoomHideTextures);
            toolStripMenuItemRoomWireframe.Text = Lang.GetText(eLang.toolStripMenuItemRoomWireframe);
            toolStripMenuItemRoomRenderNormals.Text = Lang.GetText(eLang.toolStripMenuItemRoomRenderNormals);
            toolStripMenuItemRoomOnlyFrontFace.Text = Lang.GetText(eLang.toolStripMenuItemRoomOnlyFrontFace);
            toolStripMenuItemRoomVertexColor.Text = Lang.GetText(eLang.toolStripMenuItemRoomVertexColor);
            toolStripMenuItemRoomAlphaChannel.Text = Lang.GetText(eLang.toolStripMenuItemRoomAlphaChannel);
            toolStripMenuItemRoomTextureNearestLinear.Text = Lang.GetText(eLang.toolStripMenuItemRoomTextureIsLinear);
            toolStripMenuItemModelsTextureNearestLinear.Text = Lang.GetText(eLang.toolStripMenuItemModelsTextureIsLinear);


            //save and open windows
            openFileDialogAEV.Title = Lang.GetText(eLang.openFileDialogAEV);
            openFileDialogESL.Title = Lang.GetText(eLang.openFileDialogESL);
            openFileDialogETS.Title = Lang.GetText(eLang.openFileDialogETS);
            openFileDialogITA.Title = Lang.GetText(eLang.openFileDialogITA);
            openFileDialogDSE.Title = Lang.GetText(eLang.openFileDialogDSE);
            openFileDialogFSE.Title = Lang.GetText(eLang.openFileDialogFSE);
            openFileDialogSAR.Title = Lang.GetText(eLang.openFileDialogSAR);
            openFileDialogEAR.Title = Lang.GetText(eLang.openFileDialogEAR);
            openFileDialogEMI.Title = Lang.GetText(eLang.openFileDialogEMI);
            openFileDialogESE.Title = Lang.GetText(eLang.openFileDialogESE);
            openFileDialogLIT.Title = Lang.GetText(eLang.openFileDialogLIT);
            openFileDialogEFFBLOB.Title = Lang.GetText(eLang.openFileDialogEFFBLOB);
            openFileDialogEFFBLOBBIG.Title = Lang.GetText(eLang.openFileDialogEFFBLOBBIG);
            openFileDialogQuadCustom.Title = Lang.GetText(eLang.openFileDialogQuadCustom);

            saveFileDialogConvertAEV.Title = Lang.GetText(eLang.saveFileDialogConvertAEV);
            saveFileDialogConvertETS.Title = Lang.GetText(eLang.saveFileDialogConvertETS);
            saveFileDialogConvertITA.Title = Lang.GetText(eLang.saveFileDialogConvertITA);
            saveFileDialogConvertEMI.Title = Lang.GetText(eLang.saveFileDialogConvertEMI);
            saveFileDialogConvertESE.Title = Lang.GetText(eLang.saveFileDialogConvertESE);

            saveFileDialogAEV.Title = Lang.GetText(eLang.saveFileDialogAEV);
            saveFileDialogESL.Title = Lang.GetText(eLang.saveFileDialogESL);
            saveFileDialogETS.Title = Lang.GetText(eLang.saveFileDialogETS);
            saveFileDialogITA.Title = Lang.GetText(eLang.saveFileDialogITA);
            saveFileDialogDSE.Title = Lang.GetText(eLang.saveFileDialogDSE);
            saveFileDialogFSE.Title = Lang.GetText(eLang.saveFileDialogFSE);
            saveFileDialogSAR.Title = Lang.GetText(eLang.saveFileDialogSAR);
            saveFileDialogEAR.Title = Lang.GetText(eLang.saveFileDialogEAR);
            saveFileDialogEMI.Title = Lang.GetText(eLang.saveFileDialogEMI);
            saveFileDialogESE.Title = Lang.GetText(eLang.saveFileDialogESE);
            saveFileDialogLIT.Title = Lang.GetText(eLang.saveFileDialogLIT);
            saveFileDialogEFFBLOB.Title = Lang.GetText(eLang.saveFileDialogEFFBLOB);
            saveFileDialogEFFBLOBBIG.Title = Lang.GetText(eLang.saveFileDialogEFFBLOBBIG);
            saveFileDialogQuadCustom.Title = Lang.GetText(eLang.saveFileDialogQuadCustom);

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (theAppLoadedWell)
            {
                if (isProjectEmpty() == false) {

                    DialogResult result = MessageBox.Show(
                        "You may have unsaved changes. Are you sure you want to exit?",
                        "Exit Application?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {

                }

                DataBase.ItemsModels?.ClearGL();
                DataBase.EtcModels?.ClearGL();
                DataBase.EnemiesModels?.ClearGL();
                DataBase.InternalModels?.ClearGL();
                DataBase.QuadCustomModels?.ClearGL();
                DataBase.SelectedRoom?.ClearGL();
                DataShader.EndUnload();
            }
        }


        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // entrada de teclas para açoes especiais
            cameraMove.isControlDown = e.Control;

            if (!isMouseDown){
                //not manipulating viewport shortcuts
                switch (e.KeyCode){
                    case Keys.W:
                        selectTool(0); //Select Move Tool
                        break;
                    case Keys.E:
                        selectTool(1); //Select Rotate Tool
                        break;
                }
            }
            else
            {
                //while manipualting viewport shortcuts
            }

            #region usado em propery
            // proibe a estrada de caracteres que não vão nos campos de numeros
            if (InPropertyGrid && propertyGridObjs.SelectedGridItem != null && propertyGridObjs.SelectedGridItem.PropertyDescriptor != null)
            {

                if (propertyGridObjs.SelectedGridItem.PropertyDescriptor.Attributes.Contains(new DecNumberAttribute()))
                {

                    e.SuppressKeyPress = true;
                    if (KeysCheck.KeyIsNum(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Control)
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Alt || e.Shift || e.KeyCode == Keys.Alt)
                    {
                        e.SuppressKeyPress = true;
                    }
                    if (KeysCheck.KeyIsEssential(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }

                }

                if (propertyGridObjs.SelectedGridItem.PropertyDescriptor.Attributes.Contains(new DecNegativeNumberAttribute()))
                {

                    e.SuppressKeyPress = true;
                    if (KeysCheck.KeyIsNum(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (KeysCheck.KeyIsMinus(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Control)
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Alt || e.Shift || e.KeyCode == Keys.Alt)
                    {
                        e.SuppressKeyPress = true;
                    }
                    if (KeysCheck.KeyIsEssential(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }

                }

                if (propertyGridObjs.SelectedGridItem.PropertyDescriptor.Attributes.Contains(new HexNumberAttribute()))
                {

                    e.SuppressKeyPress = true;
                    if (KeysCheck.KeyIsNum(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Shift)
                    {
                        e.SuppressKeyPress = true;
                    }
                    if (KeysCheck.KeyIsHex(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Control)
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Alt || e.KeyCode == Keys.Alt)
                    {
                        e.SuppressKeyPress = true;
                    }
                    if (KeysCheck.KeyIsEssential(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }

                }

                if (propertyGridObjs.SelectedGridItem.PropertyDescriptor.Attributes.Contains(new FloatNumberAttribute()))
                {

                    e.SuppressKeyPress = true;
                    if (KeysCheck.KeyIsNum(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (KeysCheck.KeyIsMinus(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (KeysCheck.KeyIsCommaDot(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (KeysCheck.KeyIsOnlyDot(e.KeyValue))
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Control)
                    {
                        e.SuppressKeyPress = false;
                    }
                    if (e.Alt || e.Shift || e.KeyCode == Keys.Alt)
                    {
                        e.SuppressKeyPress = true;
                    }
                    if (KeysCheck.KeyIsEssential(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                }

                if (propertyGridObjs.SelectedGridItem.PropertyDescriptor.Attributes.Contains(new NoKeyAttribute()))
                {
                    e.SuppressKeyPress = true;
                    if (KeysCheck.KeyIsEssentialNoKey(e.KeyCode))
                    {
                        e.SuppressKeyPress = false;
                    }
                }
            }

            #endregion
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            cameraMove.isControlDown = e.Control;
        }

        #endregion

    }
}
