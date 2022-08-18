using System.Windows;

namespace SCADATest.Views
{
    /// <summary>
    /// StartDevice.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StartDevice : Window
    {
        public StartDevice(string dev, Point p)
        {
            InitializeComponent();
            devicename.Text += ":" + dev;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            grd.Width = 240;
            grd.Height = 70;

            btnStart.Click += new RoutedEventHandler((s, e) =>
            {
                var tag = App.Server[dev + "_Running"];
                if (tag != null) tag.Write(true);
                this.Close();
            });

            btnStop.Click += new RoutedEventHandler((s, e) =>
            {
                var tag = App.Server[dev + "_Running"];
                if (tag != null) tag.Write(false);
                this.Close();
            });

            this.Left = p.X - grd.Width / 2;
            this.Top = p.Y - grd.Height / 2 + 40;
            if (this.Left < 0) this.Left = 0;
            if (this.Top < 0) this.Top = 0;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
