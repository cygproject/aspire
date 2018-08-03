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
#if false
            // Center window with respect to MainWindow
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 
#endif
            var config = SettingsData.Load();
            cmbPortNo.Text = config.SerialPortSettingsData.PortNum;
            cmbRate.Text = config.SerialPortSettingsData.BaudRate.ToString();
            cmbData.Text = config.SerialPortSettingsData.Databit.ToString();
            cmbParity.Text = config.SerialPortSettingsData.Parity.ToString();
            cmbStopBit.Text = config.SerialPortSettingsData.StopBit.ToString();
            cmbFlowCtl.Text = config.SerialPortSettingsData.FlowControl.ToString();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = true;
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var config = SettingsData.Load();
            config.SerialPortSettingsData.PortNum = cmbPortNo.Text;
            config.SerialPortSettingsData.BaudRate = int.Parse(cmbRate.Text);
            config.SerialPortSettingsData.Databit = int.Parse(cmbData.Text);
            config.SerialPortSettingsData.Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);
            config.SerialPortSettingsData.StopBit = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBit.Text);
            config.SerialPortSettingsData.FlowControl = (Handshake)Enum.Parse(typeof(Handshake), cmbFlowCtl.Text);
            config.Save();

            IsCancel = false;
            this.Close();
        }
    }
}
