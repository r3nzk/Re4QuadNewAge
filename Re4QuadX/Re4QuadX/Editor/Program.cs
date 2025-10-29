using Re4QuadX.Editor;
using Re4QuadX.Editor.Class.Enums;
using Re4QuadX.Editor.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Re4QuadX
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            SplashScreen.StartSplashScreen();

            int timeoutMs = 500;
            int sleepMs = 10;
            //added satefy loop to wait for SetStatusText action to be assigned
            while (timeoutMs > 0 && !(SplashScreen.Container?.isReady ?? false)){
                Thread.Sleep(sleepMs);
                timeoutMs -= sleepMs;
            }

            try
            {
                // A list of loading tasks to be completed.
                Action[] loadingTasks = new Action[]
                {
                    // Language and UI texts
                    () => {
                        Editor.Lang.StartAttributeTexts();
                        Editor.Lang.StartTexts();
                        //show text after loading cause... yeah, no langoutput without loading
                        SplashScreen.Container?.SetStatusText(Lang.GetText(eLang.LoadingSplashLang));
                    },
                    // Configurations
                    () => {
                        SplashScreen.Container?.SetStatusText(Lang.GetText(eLang.LoadingSplashConfig));
                        Editor.JSON.Configs.StartLoadConfigs();
                    },
                    // Utility data and lists
                    () => {
                        SplashScreen.Container?.SetStatusText(Lang.GetText(eLang.LoadingSplashUtility));
                        Editor.Utils.StartReloadDirectoryDic();
                        Editor.Utils.StartLoadObjsInfoLists();
                        Editor.Utils.StartLoadPromptMessageList();
                        Editor.Utils.StartLoadLangFile();
                        Editor.Utils.StartEnemyExtraSegmentList();
                    },
                    // Setting up UI properties
                    () => {
                        SplashScreen.Container?.SetStatusText(Lang.GetText(eLang.LoadingSplashUI));
                        Editor.Utils.StartSetListBoxsProperty();
                        Editor.Utils.StartSetListBoxsPropertybjsInfoLists();
                    },
                    // Tree view nodes
                    () => {
                        SplashScreen.Container?.SetStatusText(Lang.GetText(eLang.LoadingSplashTreeView));
                        Editor.Utils.StartCreateNodes();
                        Editor.Utils.StartExtraGroup();
                    },
                    // Final check (mainform load), this is set and we wait for mainform to finish loading (mainly open gl rendering loading)
                    () => {
                        SplashScreen.Container?.SetStatusText(Lang.GetText(eLang.LoadingSplashGraphics));
                    }
                };

                // Execute each task and update the progress bar.
                for (int i = 0; i < loadingTasks.Length; i++)
                {
                    loadingTasks[i].Invoke();
                    int progress = (int)((float)(i + 1) / loadingTasks.Length * 100);
                    SplashScreen.Container?.SetProgress(progress);
                    Thread.Sleep(25); //progress delay for each loading part, to avoid issues in lower end devices.
                }

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during loading: {ex.Message}", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SplashScreen.Container?.Close?.Invoke();
            }
        }

        // Manipulador para exceções de thread UI
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "Error in graphical interface");
        }

        // Manipulador para exceções não tratadas de threads não-UI
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                HandleException(ex, "General error");
            }
        }

        // Método para lidar com exceções
        private static void HandleException(Exception ex, string context)
        {
            MessageBox.Show($"{context}: {ex.Message}\nAn unexpected error occurred, the program may not work correctly from now on.", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}