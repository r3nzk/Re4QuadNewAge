using Re4QuadX.Editor.Class;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Re4QuadX.Editor
{
    public static class Console
    {
        private static RichTextBox outputControl;

        private static readonly List<(string message, LogLevel level)> earlyLogCache = new List<(string, LogLevel)>();
        private enum LogLevel { Info, Warning, Error }

        //colors
        private static Color InfoColor;
        private static Color WarningColor;
        private static Color ErrorColor;

        public static void RegisterOutputControl(RichTextBox richTextBox)
        {
            outputControl = richTextBox;

            var palette = ThemeManager.GetCurrentPalette();
            InfoColor = palette.ConsoleInfo;
            WarningColor = palette.ConsoleWarning;
            ErrorColor = palette.ConsoleError;

            outputControl.Font = new Font("Consolas", 9.75F);
            outputControl.Clear();

            if (!Globals.firstBoot)
                FlushEarlyLogs();
        }

        public static void FlushEarlyLogs()
        {
            if (outputControl == null || earlyLogCache.Count == 0)
            {
                return;
            }

            foreach (var log in earlyLogCache)
            {
                LogInternal(log.message, log.level, isFromCache: true);
            }
            earlyLogCache.Clear();
        }

        public static void Clear(){
            if (outputControl == null)
                return;

            outputControl.Clear();
        }

        public static void Log(string message) => LogInternal(message, LogLevel.Info);
        public static void Warning(string message) => LogInternal(message, LogLevel.Warning);
        public static void Error(string message) => LogInternal(message, LogLevel.Error);

        private static void LogInternal(string message, LogLevel level, bool isFromCache = false) {
            if (outputControl == null){
                earlyLogCache.Add((message, level));
                return;
            }

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
