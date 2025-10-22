using System;
using System.Windows.Forms;

namespace Re4QuadX.Editor.Forms
{
    public partial class SplashScreenForm : Form
    {
        private SplashScreenContainer container;

        private bool BlockClose = true;

        public SplashScreenForm(SplashScreenContainer container)
        {
            this.container = container;
            this.container.Close = CloseForm;
            this.container.ReleasedToClose = ReleasedToClose;
            this.container.SetStatusText = SetStatusText;
            this.container.SetProgress = SetProgressBarValue;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void SetStatusText(string text){
            if (splashLoadingLabel.InvokeRequired){
                splashLoadingLabel.Invoke(new Action<string>(SetStatusText), text);
            }
            else{
                splashLoadingLabel.Text = text;
            }
        }

        private void SetProgressBarValue(int value){
            if (splashLoadingBar.InvokeRequired)
            {
                splashLoadingBar.Invoke(new Action<int>(SetProgressBarValue), value);
            }
            else
            {
                splashLoadingBar.Value = value;
            }
        }

        private void ReleasedToClose(){
            if (container.FormIsClosed == false){
                this.Invoke(new Action(InvokedReleasedToClose));
            }
        }

        private void InvokedReleasedToClose(){
            BlockClose = false;
        }

        private void CloseForm(){
            if (container.FormIsClosed == false){
                this.Invoke(new Action(InvokedCloseForm));
            }
        }

        private void InvokedCloseForm(){
            BlockClose = false;
            Close();
        }

        private void SplashScreenForm_FormClosed(object sender, FormClosedEventArgs e){
            container.FormIsClosed = true;
        }

        private void SplashScreenForm_FormClosing(object sender, FormClosingEventArgs e){
            if (BlockClose){
                e.Cancel = true;
            }
        }


        //----------------------
        private void To(string url)
        {
            try { System.Diagnostics.Process.Start("explorer.exe", url); } catch (Exception) { }
        }

        private void SplashScreenForm_Load(object sender, EventArgs e)
        {

        }
    }
}
