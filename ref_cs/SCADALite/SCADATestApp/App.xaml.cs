using CoreLib;
using NLog;
using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace SCADATest
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : System.Windows.Application
    {
        // NLog 정적 인스턴스 생성
        public static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        static readonly string machine = Environment.MachineName;
        public static readonly DAService Server = new DAService();

        static IPrincipal _princ;
        public static IPrincipal Principal
        {
            get
            {
                return _princ;
            }
            set
            {
                _princ = value;
            }
        }

        static ReverseObservableQueue<string> eventLog = new ReverseObservableQueue<string>(300);
        public static ReverseObservableQueue<string> Events
        {
            get { return eventLog; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow win1 = null;
            //Screen scr1;
            //System.Drawing.Rectangle rect1;

            Current.DispatcherUnhandledException += App_DispatcherUnhandledException;
            base.OnStartup(e);

            win1 = new MainWindow();
            win1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win1.WindowState = WindowState.Maximized;
            /*scr1 = Screen.AllScreens[0];
            rect1 = scr1.WorkingArea;
            win1.Top = rect1.Top;
            win1.Left = rect1.Left;*/
            win1.Show();
            LOGGER.Info("SCADALite App 시작!");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Server.Dispose();
            ConfigCache.SaveConfig();
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LOGGER.Error(e.Exception);
            e.Handled = true;
        }
    }
}
