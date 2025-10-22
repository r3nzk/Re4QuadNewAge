using NewAgeTheRender;
using PowerLib.Winform.Controls;
using Re4QuadX.Editor.Class;
using Re4QuadX.Editor.Class.CustomDelegates;
using Re4QuadX.Editor.Class.Enums;
using Re4QuadX.Editor.JSON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Re4QuadX.Editor.Forms
{
    public partial class SelectRoomForm : Form
    {
        //Since the editor freezes for a second sometimes when loading the full room list,
        //this was made to communicate to the user that the program is fetching the list from scratch and not stuck.
        //(mostly happens for while opening for the first time, that's why we avoid spamming everytime we open this form)
        public static bool isFirstTimeLoading = true;
        //searchbar placeholder
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        object lastSelected = null;

        /// <summary>
        /// evendo que acontece depois de clicar em load;
        /// </summary>
        public event EventHandler onLoadButtonClick;

        public SelectRoomForm()
        {
            if (isFirstTimeLoading) Editor.Console.Log($"Loading JSON room lists for the first time...");

            InitializeComponent();
            KeyPreview = true;

            comboBoxMainList.Items.Add(Lang.GetText(eLang.NoneRoom));
            comboBoxMainList.SelectedIndex = 0;

            var list1 = Editor.Utils.LoadRoomListJSON();
            comboBoxMainList.Items.AddRange(list1.ToArray());

            SetPlaceholder(room_searchField, "Filter: name or ID");

            if (isFirstTimeLoading){
                Editor.Console.Log($"Finished loading JSON room lists.");
                isFirstTimeLoading = false;
            }

            var roomInfoItems = comboBoxMainList.Items.OfType<RoomInfo>();
            RoomInfo targetRoomList = null;

            //verify preferred list in setetings
            if (!string.IsNullOrEmpty(Globals.PreferredRoomList)){
                targetRoomList = roomInfoItems.FirstOrDefault(x => x.RoomListObj?.JsonFileName == Globals.PreferredRoomList);
            }
            //if not found, go with last used or null
            else if (DataBase.SelectedRoom != null)
            {
                targetRoomList = roomInfoItems.FirstOrDefault(x => x.RoomListObj != null && DataBase.SelectedRoom.GetRoomListObj() != null
                    && x.RoomListObj.JsonFileName == DataBase.SelectedRoom.GetRoomListObj().JsonFileName);
            }

            if (targetRoomList != null)
            {
                comboBoxMainList.SelectedItem = targetRoomList;

                if (DataBase.SelectedRoom != null)
                {
                    if (room_listBox.Items.Contains(DataBase.SelectedRoom.GetRoomModel()))
                    {
                        room_listBox.SelectedIndex = room_listBox.Items.IndexOf(DataBase.SelectedRoom.GetRoomModel());
                    }
                }
            }

            //disable load buttons to prevent null room load
            if (lastSelected == null) {
                buttonLoad.Enabled = false;
                buttonLoadComplete.Enabled = false;
            }

            ApplyTheme();

            if (Lang.LoadedTranslation){
                StartUpdateTranslation();
            }
        }
        public static void SetPlaceholder(Control control, string text)
        {
            if (control is TextBox)
            {
                SendMessage(control.Handle, EM_SETCUEBANNER, 0, text);
            }
        }

        private void room_searchField_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (!(comboBoxMainList.SelectedItem is RoomInfo r)){
                room_listBox.Items.Clear();
                return;
            }

            var allRooms = r.RoomModelDict.Values;
            var searchText = room_searchField.Text.Trim();

            room_listBox.BeginUpdate();
            room_listBox.Items.Clear();

            if (string.IsNullOrEmpty(searchText)){ //no search filter
                room_listBox.Items.AddRange(allRooms.ToArray());
            }
            else
            {
                var filteredRooms = allRooms.Where(room =>
                    (room.Description != null && room.Description.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (room.HexID.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToArray();

                room_listBox.Items.AddRange(filteredRooms);
            }

            room_listBox.EndUpdate();
        }

        private void ApplyTheme()
        {
            if (Globals.BackupConfigs.SelectedTheme == EditorTheme.Light)
                return;

            ThemeManager.ApplyTheme(this);

            ThemeManager.ApplyTheme(buttonCancel);
            ThemeManager.ApplyTheme(buttonLoad);
            ThemeManager.ApplyTheme(buttonLoadComplete);

            ThemeManager.ApplyTheme(comboBoxMainList);
            ThemeManager.ApplyTheme(room_listBox);
            ThemeManager.ApplyTheme(room_searchField);
            ThemeManager.ApplyTheme(room_searchBar);
            ThemeManager.ApplyTheme(labelText1);

            //specific coloring
            var palette = ThemeManager.GetCurrentPalette();
            room_searchBar.BackColor = palette.BackgroundDarker;
            room_searchField.BackColor = palette.BackgroundDarker;
        }
       

        private void comboBoxMainList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMainList.SelectedItem is RoomInfo r)
            {
                room_listBox.Items.Clear();
                room_listBox.Items.AddRange(r.RoomModelDict.Values.ToArray());
                ApplyFilter();
            }
            else 
            {
                room_listBox.Items.Clear();
            }

            bool foundIt = false;

            if (lastSelected is RoomModel rm)
            {
                 var list = room_listBox.Items.OfType<RoomModel>();
                 var obj = list.Where(x => x.JsonFileName == rm.JsonFileName || x.HexID == rm.HexID).FirstOrDefault();
                if (obj != null)
                {
                    int index = room_listBox.Items.IndexOf(obj);
                    if (index > -1)
                    {
                        room_listBox.SelectedIndex = index;
                        foundIt = true;
                    }
                }
            }
        }

        private bool Enable_room_listBox_SelectedIndexChanged = true;

        private void room_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Enable_room_listBox_SelectedIndexChanged)
            {
                lastSelected = room_listBox.SelectedItem;
            }

            bool isRoomSelected = (room_listBox.SelectedItem != null);
            buttonLoad.Enabled = isRoomSelected;
            buttonLoadComplete.Enabled = isRoomSelected;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void blockInput()
        {
            buttonLoad.Enabled = false;
            buttonLoadComplete.Enabled = false;
            buttonCancel.Enabled = false;
            comboBoxMainList.Enabled = false;
            room_listBox.Enabled = false;
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            blockInput();

            LoadRoomModel();
        }

        private void buttonLoadComplete_Click(object sender, EventArgs e)
        {
            blockInput();

            LoadRoomObjects();
            LoadRoomModel();
        }
        private void buttonLoadComplete_Dropdown(object sender, EventArgs e)
        {
            var btn = sender as Control;
            advancedLoadContextMenu.Show(btn, new Point(0, btn.Height));
        }
        private void buttonLoadCompleteItem_Click(object sender, EventArgs e)
        {
            var item = sender as PowerLib.Winform.Controls.DropDownButtonItem;
        }

        private void advancedLoadContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }


        public async void LoadRoomModel()
        {
            // Clear current room if existent
            if (DataBase.SelectedRoom != null)
            {
                Editor.Console.Log($"Clearing previously loaded room: {DataBase.SelectedRoom.GetRoomModel().Description} (ID: {DataBase.SelectedRoom.GetRoomModel().HexID})");
                await Task.Delay(1); //wait 1 frame to give time to log

                //clear room
                DataBase.SelectedRoom.ClearGL();
                DataBase.SelectedRoom = null;

                GC.Collect();
            }

            // Create new room
            if (comboBoxMainList.SelectedItem is RoomInfo r && room_listBox.SelectedItem is RoomModel rm)
            {
                Editor.Console.Log($"Loading new Room: {room_listBox.SelectedItem}");
                await Task.Delay(1); //wait 1 frame to give time to log

                Cursor.Current = Cursors.WaitCursor;
                //select desired room
                DataBase.SelectedRoom = new RoomSelectedObj(rm, r.RoomListObj);

                GC.Collect();
            }

            onLoadButtonClick?.Invoke(room_listBox.SelectedItem, new EventArgs());

            Cursor.Current = Cursors.Default;
            Close();
        }

        private void LoadRoomObjects()
        {
            if (!(comboBoxMainList.SelectedItem is RoomInfo roomInfo) || !(room_listBox.SelectedItem is RoomModel roomModel) || roomInfo.RoomListObj == null){
                Editor.Console.Log("No room selected for complete load, or room list is invalid. Loading model only.");
                return;
            }

            Editor.Console.Log($"Starting complete load for room: {roomModel.Description}");

            //clear all object files present to replace with new room
            Editor.Console.Log("Clearing previously loaded object files..");
            if (Application.OpenForms["MainForm"] is MainForm main)
            {
                main.Invoke((MethodInvoker)delegate {
                    main.TreeViewUpdateSelectedsClear();
                });
            }
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

            EditorRe4Ver gameVersion = Globals.PreferredVersion;

            //get game version and base path
            string rootGamePath = "";
            bool isUhd = false;
            bool isPs4NsAdapted = false;

            switch (gameVersion)
            {
                case EditorRe4Ver.UHD:
                    rootGamePath = Globals.DirectoryUHDRE4;
                    isUhd = true;
                    break;
                case EditorRe4Ver.SourceNext2007:
                    rootGamePath = Globals.Directory2007RE4;
                    break;
                case EditorRe4Ver.PS2:
                    rootGamePath = Globals.DirectoryPS2RE4;
                    break;
                case EditorRe4Ver.PS4NS:
                    rootGamePath = Globals.DirectoryPS4NSRE4;
                    isUhd = true;
                    isPs4NsAdapted = true;
                    break;
                default:
                    Editor.Console.Warning($"The selected room list's game version '{gameVersion}' is not supported for auto-loading files.");
                    return;
            }

            //find the room directory (combine root game path + /bio4 for UHD)
            string basePath = (gameVersion == EditorRe4Ver.UHD || gameVersion == EditorRe4Ver.PS4NS)
            ? Path.Combine(rootGamePath, "BIO4")
            : rootGamePath;

            string rawFileName = Path.GetFileNameWithoutExtension(roomModel.JsonFileName);
            string roomName = rawFileName.Split('_')[0]; //gets raw room ID without extra shit (for r100 mostly)
            string roomDirectory = null;

            //search in stage folders (st0-st7)
            for (int i = 0; i <= 7; i++)
            {
                string stagePath = Path.Combine(basePath, "St" + i);
                if (!Directory.Exists(stagePath)) continue;

                string potentialPath = Path.Combine(stagePath, roomName);
                if (Directory.Exists(potentialPath))
                {
                    roomDirectory = potentialPath;
                    Editor.Console.Log($"Found room directory at: {roomDirectory}");
                    break;
                }
            }

            if (roomDirectory == null){
                Editor.Console.Error($"Could not find directory for room '{roomName}' in any 'St' folder for version '{gameVersion}'. Skippiong object file load.");
                return;
            }

            LoadAllFilesForRoom(roomDirectory, isUhd, isPs4NsAdapted);
        }

        private void LoadAllFilesForRoom(string roomPath, bool isUhd, bool isPs4Ns){
            var fileLoadActions = new Dictionary<string, Action<string, FileStream, FileInfo>>{
                /*{ ".ESL", (path, file, info) => { 
                    esl is not located inside stage folder, so later we can do some specific logic here.
                    maybe we can set a list of esl enemies based on room and them auto show defined room to desired room 
                },*/
                
                { ".ETS", (path, file, info) => { if (isUhd) FileManager.LoadFileETS_UHD(file, info); else FileManager.LoadFileETS_2007_PS2(file, info); Globals.FilePathETS = path; } },
                { ".ITA", (path, file, info) => {
                    if (isPs4Ns) FileManager.LoadFileITA_PS4_NS(file, info);
                    else if (isUhd) FileManager.LoadFileITA_UHD(file, info);
                    else FileManager.LoadFileITA_2007_PS2(file, info);
                    Globals.FilePathITA = path;
                } },
                { ".AEV", (path, file, info) => {
                    if (isPs4Ns) FileManager.LoadFileAEV_PS4_NS(file, info);
                    else if (isUhd) FileManager.LoadFileAEV_UHD(file, info);
                    else FileManager.LoadFileAEV_2007_PS2(file, info);
                    Globals.FilePathAEV = path;
                } },
                { ".DSE", (path, file, info) => { FileManager.LoadFileDSE(file, info); Globals.FilePathDSE = path; } },
                { ".FSE", (path, file, info) => { FileManager.LoadFileFSE(file, info); Globals.FilePathFSE = path; } },
                { ".SAR", (path, file, info) => { FileManager.LoadFileSAR(file, info); Globals.FilePathSAR = path; } },
                { ".EAR", (path, file, info) => { FileManager.LoadFileEAR(file, info); Globals.FilePathEAR = path; } },
                { ".EMI", (path, file, info) => { if (isUhd) FileManager.LoadFileEMI_UHD(file, info); else FileManager.LoadFileEMI_2007_PS2(file, info); Globals.FilePathEMI = path; } },
                { ".ESE", (path, file, info) => { if (isUhd) FileManager.LoadFileESE_UHD(file, info); else FileManager.LoadFileESE_2007_PS2(file, info); Globals.FilePathESE = path; } },
                { ".LIT", (path, file, info) => { if (isUhd) FileManager.LoadFileLIT_UHD(file, info); else FileManager.LoadFileLIT_2007_PS2(file, info); Globals.FilePathLIT = path; } }
            };


            foreach (var pair in fileLoadActions)
            {
                string extension = pair.Key;

                bool shouldLoad = true;
                switch (extension.ToUpperInvariant())
                {
                    //.esl is not in the context menu
                    case ".ITA": shouldLoad = includeITA.Checked; break;
                    case ".AEV": shouldLoad = includeAEV.Checked; break;
                    case ".LIT": shouldLoad = includeLIT.Checked; break;
                    case ".ETS": shouldLoad = includeETS.Checked; break;
                    case ".ESE": shouldLoad = includeESE.Checked; break;
                    case ".EMI": shouldLoad = includeEMI.Checked; break;
                    case ".DSE": shouldLoad = includeDSE.Checked; break;
                    case ".FSE": shouldLoad = includeFSE.Checked; break;
                    case ".SAR": shouldLoad = includeSAR.Checked; break;
                    case ".EAR": shouldLoad = includeEAR.Checked; break;
                }

                if (!shouldLoad){
                    continue;
                }

                string[] files = Directory.GetFiles(roomPath, "*" + extension, SearchOption.TopDirectoryOnly);

                if (files.Length > 0)
                {
                    string fileToLoad = files[0];
                    Editor.Console.Log($"Found {extension.Substring(1)} file: {Path.GetFileName(fileToLoad)}. Importing...");
                    LoadFileAction(fileToLoad, pair.Value);
                }
                else
                {
                    Editor.Console.Warning($"No {extension.Substring(1)} file found in room directory. Skipping...");
                }
            }
        }

        /// <summary>
        /// Generic wrapper to safely open a file and call the appropriate load action.
        /// </summary>
        private void LoadFileAction(string path, Action<string, FileStream, FileInfo> loadAction)
        {
            try
            {
                var fileInfo = new FileInfo(path);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    using (FileStream fileStream = fileInfo.OpenRead())
                    {
                        loadAction(path, fileStream, fileInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Editor.Console.Error($"Failed to load file {Path.GetFileName(path)}: {ex.Message}");
            }
        }


        private void SelectRoomForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void StartUpdateTranslation() 
        {
            this.Text = Lang.GetText(eLang.SelectRoomForm);
            labelText1.Text = Lang.GetText(eLang.labelSelectAList);
            buttonLoad.Text = Lang.GetText(eLang.SelectRoomButtonLoad);
            buttonCancel.Text = Lang.GetText(eLang.SelectRoomButtonCancel);
        }

        private void SelectRoomForm_Load(object sender, EventArgs e)
        {
            buttonLoadComplete.DropDown += buttonLoadComplete_Dropdown;
        }
    }
}
