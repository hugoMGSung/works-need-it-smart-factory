using DataService;
using HMIControl;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SCADATest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        readonly double x1 = SystemParameters.PrimaryScreenWidth;  // 화면 전체 너비
        readonly double y1 = SystemParameters.PrimaryScreenHeight;   // 화면 전체 높이

        private List<TagNodeHandle> _valueChangedList;
        private Dictionary<string, ContentControl> _dict = new Dictionary<string, ContentControl>();

        public class CustomPrincipal : IPrincipal
        {
            public IIdentity Identity { get; private set; }

            public bool IsInRole(string role)
            {
                throw new NotImplementedException();
            }

            public CustomPrincipal(string userId)
            {
                Identity = new GenericIdentity(userId);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        // 윈도우 로드
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.Principal = new CustomPrincipal("System");

            if (App.Principal != null)
            {
                TxtUser.Text = $"현재사용자: {App.Principal.Identity.Name} / 권한: {App.Principal}";
            }

            #region  기본 인터페이스 표시 디버깅 完!

            if (Tag != null && !string.IsNullOrEmpty(Tag.ToString()))
            {
                string Wintypes = Tag.ToString().TrimEnd(';');
                var control = Activator.CreateInstance(Type.GetType(Wintypes)) as ContentControl;
                if (control != null)
                {
                    ScaleControl(control);
                    control.Loaded += Control_Loaded;
                    control.Unloaded += Control_Unloaded;
                    _dict[Wintypes] = control;
                    cvs1.Child = control;
                    Title = Wintypes;
                }
            }

            #endregion

            #region 상태 표시줄 시간 표시, PLC 연결 상태 표시, PLC 감시 장치와 통신

            DispatcherTimer ShowTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };

            ShowTimer.Tick += (s, e1) =>
            {
                TxtTime.InvokeAsynchronously(delegate { TxtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); });
                LmpConn.Fill = App.Server.Drivers.Any(x => x.IsClosed) ? Brushes.Red : Brushes.Green;
            };
            ShowTimer.Start();

            #endregion

            #region 서버 바인딩

            lock (this) { _valueChangedList = this.BindingToServer(App.Server); }

            BindingTagWindow(this);
            CommandBindings.AddRange(BindingCommandHandler());
            ITag tag = App.Server["__CoreEvent"];
            if (tag != null)
            {
                tag.ValueChanged += (s, e1) =>
                {
                    if (tag != null)
                    {
                        App.Events.ReverseEnqueue($"{tag.GetTagName()} : {DateTime.Now} : {tag.ToString()}");
                        if (tag.ToString().Contains("Error:"))
                        {
                            _ = MessageBox.Show(tag.ToString(), "오류！", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                };
            }

            #endregion
        }

        // 컨트롤 로드시 처리 이벤트핸들러
        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ContentControl uie)
            {
                uie.Tag = "YES";
                BindingTagWindow(uie);
            }
        }

        // 컨트롤 언로드시 처리 이벤트핸들러
        private void Control_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ContentControl uie)
            {
                uie.Tag = "NO";
                IEnumerable<ITagWindow> windows = uie.FindChildren<ITagWindow>();
                foreach (ITagWindow item in windows)
                {
                    if (!string.IsNullOrEmpty(item.TagWindowText))
                    {
                        ((UIElement)item).RemoveHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(Item_MouseLeftButtonUp));
                    }
                }
            }
        }

        private void ScaleControl(ContentControl ctrl)
        {
            var transform = ctrl.RenderTransform as MatrixTransform;
            if (transform != null && !double.IsNaN(ctrl.Width) && !double.IsNaN(ctrl.Height))
            {
                Matrix matrix = transform.Matrix;
                matrix.Scale(x1 / ctrl.Width, y1 / ctrl.Height);
                ctrl.RenderTransform = new MatrixTransform(matrix);
                ctrl.Width = x1;
                ctrl.Height = y1;
                Background = ctrl.Background;
            }
        }

        private void BindingTagWindow(DependencyObject container)
        {
            if (container == null)
            {
                return;
            }

            foreach (ITagWindow item in container.FindChildren<ITagWindow>())
            {
                if (!string.IsNullOrEmpty(item.TagWindowText))
                {
                    UIElement element = item as UIElement;
                    element.AddHandler(UIElement.MouseLeftButtonUpEvent,
                                       new MouseButtonEventHandler(Item_MouseLeftButtonUp));
                }
            }
        }

        private void Item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowContent(sender as ITagWindow);
            e.Handled = true;
        }

        // 컨텐츠 화면 띄우기 로직!
        private void ShowContent(ITagWindow tagWindow)
        {
            if (tagWindow == null || string.IsNullOrEmpty(tagWindow.TagWindowText))
            {
                return;
            }

            string[] windows = tagWindow.TagWindowText.TrimEnd(';').Split(';');
            foreach (string txt in windows)
            {
                if (_dict.ContainsKey(txt))
                {
                    if (_dict[txt].Tag.ToString() != "YES")
                    {
                        cvs1.Child = _dict[txt];
                    }
                    continue;
                }

                if (tagWindow.IsUnique)
                {
                    foreach (object win in App.Current.Windows)
                    {
                        if (win.ToString() == txt)
                            goto lab1;
                    }
                }

                try
                {
                    if (Activator.CreateInstance(Type.GetType(txt)) is ContentControl ctrl)
                    {
                        Window win = ctrl as Window;
                        if (win == null)
                            ScaleControl(ctrl);

                        ctrl.Loaded += Control_Loaded;
                        ctrl.Unloaded += Control_Unloaded;

                        if (win != null)
                        {
                            win.Owner = this;
                            win.ShowInTaskbar = false;
                            if (tagWindow.IsModel)
                                win.ShowDialog();
                            else
                                win.Show();
                        }
                        else
                        {
                            _dict[txt] = ctrl;
                            cvs1.Child = ctrl;
                            Title = txt;
                        }
                    }
                }
                catch (Exception e)
                {
                    App.LOGGER.Error($"예외발생, ShowContent : [{e}]");
                }

            lab1:
                continue;
            }
        }

        private CommandBindingCollection BindingCommandHandler()
        {
            CoreLib.DAService srv = App.Server;
            CommandBindingCollection CommandBindings = new CommandBindingCollection();
            return CommandBindings;
        }

        // 시작 버튼 누르면 팝업 툴바트레이 보이기
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            switch (MnuMain.Visibility)
            {
                case Visibility.Hidden:
                    MnuMain.Visibility = Visibility.Visible;
                    break;
                case Visibility.Visible:
                    MnuMain.Visibility = Visibility.Hidden;
                    break;
            }
        }

        // 프로그램 종료
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            // TODO : MetroUI로 변경할 것
            /*var result = MessageBox.Show("종료하시겠습니까？", "확인", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            App.Current.Shutdown(0);
            e.Handled = true;*/
            _ = Dispatcher.BeginInvoke(new Action(async () => await ConfirmShutdown()));
        }

        private bool _shutdown;  // 종료 여부 확인

        // 종료처리 Metro 메시지 팝업
        private async Task ConfirmShutdown()
        {
            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("프로그램 종료",
                                                     "SCADA 프로그램을 종료하시겠습니까?",
                                                     MessageDialogStyle.AffirmativeAndNegative, mySettings);

            this._shutdown = result == MessageDialogResult.Affirmative;

            if (this._shutdown)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
