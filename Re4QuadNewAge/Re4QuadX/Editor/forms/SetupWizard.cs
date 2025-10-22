using Re4QuadX.Editor.JSON;
using ReaLTaiizor.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            pages = new List<System.Windows.Forms.Panel> { page1, page2, page3, page4, page5 };
            pageLabels = new List<Label> { pageLabel1, pageLabel2, pageLabel3, pageLabel4, pageLabel5 };
        }

        private void SetupWizard_Load(object sender, EventArgs e)
        {
            //get current theme from config file and apply to dropdown, we save it as initial index to detect if changed later (require restart)
            initialThemeIndex = (int)Globals.BackupConfigs.SelectedTheme;
            themeDropdown.SelectedIndex = initialThemeIndex;

            NavigateToPage(0);
        }

        ///handle all visual updates when moving between wizard pages.
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

        ///updates the font style of the step labels to indicate the current page.
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

        ///enables/disables navigation buttons and updates the next button text.
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

        private void themeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.BackupConfigs.SelectedTheme = (Editor.Class.EditorTheme)themeDropdown.SelectedIndex;
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}