using System;
using System.IO.Ports;
using System.Windows;

namespace Aspire
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public bool IsCancel { get; private set; }

        public SettingsWindow()
        {
            InitializeComponent();

            cmbPortNo.ItemsSource = SerialPort.GetPortNames();

            cmbRate.ItemsSource = new string[] {
                "9600",
                "38400",
                "115200"
            };

            cmbData.ItemsSource = new string[] {
                "7",
                "8"
            };

            cmbParity.ItemsSource = new string[] {
                Parity.None.ToString(),
                Parity.Even.ToString(),
                Parity.Mark.ToString(),
                Parity.Odd.ToString(),
                Parity.Space.ToString()
            };

            cmbStopBit.ItemsSource = new string[] {
                StopBits.None.ToString(),
                StopBits.One.ToString(),
                StopBits.OnePointFive.ToString(),
                StopBits.Two.ToString()
            };

            cmbFlowCtl.ItemsSource = new string[] {
                Handshake.None.ToString(),
                Handshake.RequestToSend.ToString(),
                Handshake.RequestToSendXOnXOff.ToString(),
                Handshake.XOnXOff.ToString()
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = true;
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = false;
            this.Close();
        }
    }
}
