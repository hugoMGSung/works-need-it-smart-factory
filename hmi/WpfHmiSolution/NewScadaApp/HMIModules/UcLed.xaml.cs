using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewScadaApp
{
    /// <summary>
    /// UcLed.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcLed : UserControl
    {
        // 이벤트 공개 RoutedEvent, 속성 공개 DepenencyProperty 
        public static readonly DependencyProperty CurrStateProperty =
            DependencyProperty.Register("CurrState", typeof(Color), typeof(UcLed), new PropertyMetadata(Color.FromRgb(83, 86, 90)));

        public Color CurrState
        {
            get { return (Color)GetValue(CurrStateProperty); }
            set { SetValue(CurrStateProperty, value); }
        }

        public UcLed()
        {
            InitializeComponent();
        }
    }
}
