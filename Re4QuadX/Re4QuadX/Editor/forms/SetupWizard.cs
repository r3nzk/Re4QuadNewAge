using Re4QuadX.Editor.Class;
using Re4QuadX.Editor.Class.Enums;
using Re4QuadX.Editor.JSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Re4QuadX.Editor.forms
{
    public partial class SetupWizard : Form
    {
        //navigation
        private int currentPageIndex = 0;
        private List<System.Windows.Forms.Panel> pages;
        private List<Label> pageLabels;

        private int initialThemeIndex;
        private bool wizardFinished = false;

        public SetupWizard()
        {
            InitializeComponent();

            //initialize pages
            pages = new List<System.Windows.Forms.Panel> { page1, page2, page3, page4, page5 }; // Assuming you meant page4 and page5
            pageLabels = new List<Label> { pageLabel1, pageLabel2, pageLabel3, pageLabel4, pageLabel5 };
        }

        private void SetupWizard_Load(object sender, EventArgs e)
        {
            //get current theme from config file and apply to dropdown, we save it as initial index to detect if changed later (require restart)
            initialThemeIndex = (int)Globals.BackupConfigs.SelectedTheme;
            themeDropdown.SelectedIndex = initialThemeIndex;
            checkBoxCheckUpdates.Checked = Globals.checkUpdates;

            //get current directories
            textBoxDirectory2007RE4.Text = Globals.BackupConfigs.Directory2007RE4;
            textBoxDirectoryPS2RE4.Text = Globals.BackupConfigs.DirectoryPS2RE4;
            textBoxDirectoryUHDRE4.Text = Globals.BackupConfigs.DirectoryUHDRE4;
            textBoxDirectoryPS4NSRE4.Text = Globals.BackupConfigs.DirectoryPS4NSRE4;

            //get current tool
            toolTextbox_udas.Text = Globals.BackupConfigs.ToolPathUDAS;
            toolTextbox_lfs.Text = Globals.BackupConfigs.ToolPathLFS;
            toolTextbox_pack.Text = Globals.BackupConfigs.ToolPathPACK;
            toolTextbox_gca.Text = Globals.BackupConfigs.ToolPathGCA;

            //get current xfile
            textbox_xfile.Text = Globals.BackupConfigs.DirectoryXFILE;

            preferredVerComboBox.SelectedIndex = (int)Globals.PreferredVersion;

            NavigateToPage(0);
        }

        //handle all visual updates when moving between wizard pages.
        private void NavigateToPage(int index)
        {
            //bundary check
            if (index < 0 || index >= pages.Count)
            {
                return;
            }

            currentPageIndex = index;

            //hide all pages
            foreach (var page in pages)
            {
                page.Visible = false;
            }

            //Show only the current page
            pages[currentPageIndex].Visible = true;

            UpdateStepLabels();
            UpdateButtonStates();
        }

        //updates the font style of the step labels to indicate the current page.
        private void UpdateStepLabels()
        {
            for (int i = 0; i < pageLabels.Count; i++)
            {
                if (i == currentPageIndex)
                {
                    pageLabels[i].Font = new Font(pageLabels[i].Font, FontStyle.Bold);
                }
                else
                {
                    pageLabels[i].Font = new Font(pageLabels[i].Font, FontStyle.Regular);
                }
            }
        }

        //enables/disables navigation buttons and updates the next button text.
        private void UpdateButtonStates()
        {
            backButton.Enabled = (currentPageIndex > 0);

            if (currentPageIndex == pages.Count - 1)
            {
                nextButton.Text = "Finish";
            }
            else
            {
                nextButton.Text = "Next";
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (currentPageIndex < pages.Count - 1)
            {
                NavigateToPage(currentPageIndex + 1);
            }
            else
            {
                //we are on the last page, so the button acts as "Finish".
                HandleWizardCompletion();
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            NavigateToPage(currentPageIndex - 1);
        }

        private void HandleWizardCompletion()
        {
            Globals.BackupConfigs.PreferredVersion = (EditorRe4Ver)preferredVerComboBox.SelectedIndex;
            MainForm.instance.PreferredVerSet(preferredVerComboBox.SelectedIndex);
            //apply unchanged settings
            ConfigsFile.writeConfigsFile(Consts.ConfigsFileDirectory, Globals.BackupConfigs);

            //check if any setting was changed that requires a restart.
            bool themeChanged = themeDropdown.SelectedIndex != initialThemeIndex;

            //add other checks here if needed in the future
            bool needsRestart = themeChanged;

            if (needsRestart)
            {
                AskForRestart();
            }
            else
            {
                wizardFinished = true;
                Close();
            }
        }

        private void AskForRestart()
        {
            string message = "Some of the settings you changed require the application to restart. Do you want to restart now?";
            string caption = "Restart Required";

            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            wizardFinished = true;
            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
            else
            {
                Close();
            }
        }

        #region gettting started
        private void themeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.BackupConfigs.SelectedTheme = (Editor.Class.EditorTheme)themeDropdown.SelectedIndex;
        }

        private void checkBoxCheckUpdates_CheckedChanged(object sender, EventArgs e)
        {
            Globals.checkUpdates = checkBoxCheckUpdates.Checked;
        }
        #endregion


        private void quitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region directory assignment
        private void buttonDirectoryPS4NSRE4_Click(object sender, EventArgs e)
        {
            SelectFolder(textBoxDirectoryPS4NSRE4, "Select RE4 PS4/NS Directory", (path) => Globals.BackupConfigs.DirectoryPS4NSRE4 = path);
        }

        private void buttonDirectory2007RE4_Click(object sender, EventArgs e)
        {
            SelectFolder(textBoxDirectory2007RE4, "Select RE4 2007 Directory", (path) => Globals.BackupConfigs.Directory2007RE4 = path);
        }

        private void buttonDirectoryPS2RE4_Click(object sender, EventArgs e)
        {
            SelectFolder(textBoxDirectoryPS2RE4, "Select RE4 PS2 Directory", (path) => Globals.BackupConfigs.DirectoryPS2RE4 = path);
        }

        private void buttonDirectoryUHDRE4_Click(object sender, EventArgs e)
        {
            SelectFolder(textBoxDirectoryUHDRE4, "Select RE4 UHD Directory", (path) => Globals.BackupConfigs.DirectoryUHDRE4 = path);
        }
        private void directoriesHelp_Click(object sender, EventArgs e)
        {
            Utils.OpenLink("https://github.com/r3nzk/Re4QuadX/wiki/Setup#getting-game-paths");
        }

        #endregion

        #region automation
        private void toolButton_udas_Click(object sender, EventArgs e)
        {
            SelectFile(toolTextbox_udas, "Select UDAS Tool", (path) => Globals.BackupConfigs.ToolPathUDAS = path);
        }

        private void toolButton_lfs_Click(object sender, EventArgs e)
        {
            SelectFile(toolTextbox_lfs, "Select LFS Tool", (path) => Globals.BackupConfigs.ToolPathLFS = path);
        }

        private void toolButton_pack_Click(object sender, EventArgs e)
        {
            SelectFile(toolTextbox_pack, "Select PACK Tool", (path) => Globals.BackupConfigs.ToolPathPACK = path);
        }

        private void toolButton_gca_Click(object sender, EventArgs e)
        {
            SelectFile(toolTextbox_gca, "Select GCA Tool", (path) => Globals.BackupConfigs.ToolPathGCA = path);
        }

        //help
        private void getToolButton_Click(object sender, EventArgs e)
        {
            Utils.OpenLink("https://github.com/r3nzk/Re4QuadX/wiki/Automation-and-Tools#get-tools");
        }

        private void automationGuideButton_Click(object sender, EventArgs e)
        {
            Utils.OpenLink("https://github.com/r3nzk/Re4QuadX/wiki/Automation-and-Tools#automation-guide");
        }

        #endregion

        #region xfile
        private void xfileBrowse_Click(object sender, EventArgs e)
        {
            SelectFolder(textbox_xfile, "Select XFILE Directory", (path) => Globals.BackupConfigs.DirectoryXFILE = path);
        }

        private void xfileHelp_Click(object sender, EventArgs e)
        {
            Utils.OpenLink("https://github.com/r3nzk/Re4QuadX/wiki/Setup#getting-xfile");
        }

        #endregion

        #region Helper Methods

        private void SelectFolder(TextBox textBox, string description, Action<string> updateConfigAction)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;
                if (Directory.Exists(textBox.Text))
                {
                    dialog.SelectedPath = textBox.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string fixedPath = Utils.FixDirectory(dialog.SelectedPath);
                    textBox.Text = fixedPath;
                    updateConfigAction(fixedPath);
                }
            }
        }

        private void SelectFile(TextBox textBox, string title, Action<string> updateConfigAction)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                dialog.Title = title;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;

                if (File.Exists(textBox.Text))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(textBox.Text);
                    dialog.FileName = Path.GetFileName(textBox.Text);
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = dialog.FileName;
                    updateConfigAction(dialog.FileName);
                }
            }
        }
        #endregion

        private void preferredVerComboBox_SelectionIndexChanged(object sender, EventArgs e)
        {
            Globals.PreferredVersion = (EditorRe4Ver)preferredVerComboBox.SelectedIndex;
        }
        private async void unpackAllButton_Click(object sender, EventArgs e)
        {
            EditorRe4Ver gameVersion = Globals.PreferredVersion;

            if (gameVersion == EditorRe4Ver.PS2) return; //ps2 still not supported (i think)

            string message = $"Are you sure you want to extract all rooms for '{gameVersion}' version?\n\n This can take some time.";
            string caption = "Are you sure?";

            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No) return;

            bool deleteLFS = false;

            if (Globals.PreferredVersion == EditorRe4Ver.UHD || Globals.PreferredVersion == EditorRe4Ver.PS4NS)
            {
                var result2 = MessageBox.Show(
                    "Would you like to delete original '.lfs' compressed files and keep only new uncompressed '.udas'?\n\n(Keeping original will significantly fill disk space)",
                    "Delete uncompressed .lfs files?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result2 == DialogResult.Yes)
                    deleteLFS = true;
            }

            await ExternalToolManager.UnpackAllRoomsUdas(deleteLFS);

            if (gameVersion == EditorRe4Ver.SourceNext2007) return; //only does textures for uhd or ps4/ns

            await Task.Delay(1);

            var result3 = MessageBox.Show(
            "Choose the unpack mode:\n\nYes = ImagePackHD\nNo = ImagePack (SD)",
            "Select Unpack Mode",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

            string imagepackType = null;

            if (result == DialogResult.Yes)
            {
                imagepackType = "ImagePackHD";
            }
            else if (result == DialogResult.No)
            {
                imagepackType = "ImagePack";
            }
            ExternalToolManager.UnpackAllPacks(imagepackType);
        }
    }
}