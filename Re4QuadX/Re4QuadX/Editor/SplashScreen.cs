using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Re4QuadX.Editor.Forms;

namespace Re4QuadX.Editor
{
    public static class SplashScreen
    {
        public static SplashScreenContainer Container { get; set; }
        private static void SplashScreenShow()
        {
            Application.Run(new SplashScreenForm(Container));
        }

        public static void StartSplashScreen()
        {
            Container = new SplashScreenContainer();
            System.Threading.Thread threadSplashScreen = new System.Threading.Thread(SplashScreenShow);
            threadSplashScreen.SetApartmentState(System.Threading.ApartmentState.STA);
            threadSplashScreen.Start();
        }
    }

    public class SplashScreenContainer
    {
        public Action Close;
        public Action ReleasedToClose;
        public bool FormIsClosed = false;
        public Action<int> SetProgress;
        public Action<string> SetStatusText;
    }
}
