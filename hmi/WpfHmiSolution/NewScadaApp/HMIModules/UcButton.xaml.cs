using System.Windows;
using System.Windows.Controls;

namespace NewScadaApp
{
    /// <summary>
    /// UcButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcButton : UserControl
    {
        // Step 1, RoutedEvent 정의
        public static readonly RoutedEvent CustomClickEvent =
            EventManager.RegisterRoutedEvent("CustomClick", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UcButton));

        // Step 2, RoutedEventHandler 선언
        public event RoutedEventHandler CustomClick
        {
            add { AddHandler(CustomClickEvent, value); }  // += eventHandler
            remove { RemoveHandler(CustomClickEvent, value); } // -= eventHandler
        }

        public UcButton()
        {
            InitializeComponent();
        }

        private void BtnClick_Click(object sender, RoutedEventArgs e)
        {
            // Step 3, 버튼클릭 이벤트를 상위로 올려주는 실행
            RaiseEvent(new RoutedEventArgs(CustomClickEvent, sender));
        }
    }
}
