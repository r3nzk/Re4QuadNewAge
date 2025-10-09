using System;
using System.Drawing;
using System.Windows.Forms;

namespace Re4QuadExtremeEditor.Editor
{
    public static class Console
    {
        private static RichTextBox outputControl;

        private enum LogLevel { Info, Warning, Error }

        //colors
        private static readonly Color InfoColor = ColorTranslator.FromHtml("#adadad");
        private static readonly Color WarningColor = ColorTranslator.FromHtml("#c2b34f");
        private static readonly Color ErrorColor = ColorTranslator.FromHtml("#c26161");

        public static void RegisterOutputControl(RichTextBox richTextBox)
        {
            outputControl = richTextBox;
            outputControl.ForeColor = InfoColor;
            outputControl.Font = new Font("Consolas", 9.75F);
            outputControl.Clear();
        }

        public static void Clear(){
            if (outputControl == null)
                return;


            outputControl.Clear();
        }

        public static void Log(string message) => LogInternal(message, LogLevel.Info);
        public static void Warning(string message) => LogInternal(message, LogLevel.Warning);
        public static void Error(string message) => LogInternal(message, LogLevel.Error);

        private static void LogInternal(string message, LogLevel level){
            if (outputControl == null)
                return;

            if (outputControl.InvokeRequired){
                outputControl.Invoke(new Action(() => LogInternal(message, level)));
                return;
            }

            outputControl.SelectionStart = outputControl.TextLength;
            outputControl.SelectionLength = 0;

            Color messageColor;
            switch (level){
                case LogLevel.Warning:
                    messageColor = WarningColor;
                    break;
                case LogLevel.Error:
                    messageColor = ErrorColor;
                    break;
                default:
                    messageColor = InfoColor;
                    break;
            }

            outputControl.SelectionColor = messageColor;

            string formattedMessage = $"[{DateTime.Now.ToString("HH:mm:ss")}] {message}\n";
            outputControl.SelectedText = formattedMessage;

            // Reset color and scroll to the end.
            outputControl.SelectionColor = InfoColor;
            outputControl.ScrollToCaret();
        }
    }
}
