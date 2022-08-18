using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WpfScadaApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private string serverIpNum = "192.168.200.105";
        private string subscribeStr = "machine01";
        private MqttClient client;          // MQTT Subscribe / Publish 객체
        private string connectionString;    // SQL Server 연결 문자열
        private bool opened;                // 연결 여부 확인
        private string clientId = "HMI_monitor";

        public MainWindow()
        {
            InitializeComponent();
            InitAllCustom();
        }

        private void InitAllCustom()
        {
            LblStatus.Text = string.Empty;
            connectionString = $@"Data Source={serverIpNum};" +
                                "Initial Catalog=HMI_DATA;" +
                                "Integrated Security=True";
            IPAddress serverAddress;
            serverAddress = IPAddress.Parse(serverIpNum);
            client = new MqttClient(serverAddress);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.MqttMsgPublished += Client_MqttMsgPublished;

            App.LOGGER.Info("HMI Monitoring start!");
        }

        private void Client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                LblStatus.Text = "비상모터 처리";
            }));
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                var message = Encoding.UTF8.GetString(e.Message);
                var currentDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                var color = currentDatas["color"];
                var state = currentDatas["state"];
                LblStatus.Text = currentDatas["color"] + "/" + currentDatas["state"];   
                
                if (color == "green" && state == "1")
                {
                    LedAlarm.CurrState = Color.FromRgb(0, 255, 0);
                }
                else if (color == "red" && state == "1")
                {
                    LedAlarm.CurrState = Color.FromRgb(255, 0, 0);
                }
                else
                {
                    LedAlarm.CurrState = Color.FromRgb(83, 86, 90);
                }
            }));
        }

        private void MySimpleButton_CustomClick(object sender, RoutedEventArgs e)
        {
            if (client.IsConnected)
            {
                string allData = "{ \"motor\" : \"120\" }";
                client.Publish("machine01/4002/", Encoding.UTF8.GetBytes(allData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
        }

        private void BtnMonitoring_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Connect("Server01");
                LblStatus.Text = "Monitoring started";
                client.Subscribe(new string[] { "machine01/4001/" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            catch (Exception ex)
            {
                App.LOGGER.Error($"BtnMonitoring_Click error occurred : {ex.Message}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.Disconnect();
        }
    }
}
