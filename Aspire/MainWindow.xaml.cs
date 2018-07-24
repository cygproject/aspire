using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aspire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DATA_NO_107_BANK_SWITCHING = "107";
        private const string DATA_NO_400_LD_OFF_START = "400";
        private const string DATA_NO_401_LD_OFF_END = "401";
        private const string DATA_NO_132_BANK0_H_THRESHOLD = "132";
        private const string DATA_NO_133_BANK0_L_THRESHOLD = "133";
        private const string DATA_NO_166_BANK1_H_THRESHOLD = "166";
        private const string DATA_NO_167_BANK1_L_THRESHOLD = "167";
        private const string DATA_NO_196_BANK2_H_THRESHOLD = "196";
        private const string DATA_NO_197_BANK2_L_THRESHOLD = "197";
        private const string DATA_NO_228_BANK3_H_THRESHOLD = "228";
        private const string DATA_NO_229_BANK3_L_THRESHOLD = "229";
        private const string DATA_NO_519_MEASURED_VALUE = "519";
        private const string DATA_NO_580_SOFTWARE_VERSION = "580";

        /// <summary>
        /// シリアルポート | Serial port
        /// </summary>
        private SerialPort serialPort = null;


        public MainWindow()
        {
            InitializeComponent();

            OpenSerialPort();
        }

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        private void OpenSerialPort()
        {
            var config = SettingsData.Load();
            serialPort = new SerialPort(config.SerialPortSettingsData.PortNum);
            serialPort.BaudRate = config.SerialPortSettingsData.BaudRate;
            serialPort.Parity = config.SerialPortSettingsData.Parity;
            serialPort.StopBits = config.SerialPortSettingsData.StopBit;
            serialPort.DataBits = config.SerialPortSettingsData.Databit;
            serialPort.Handshake = config.SerialPortSettingsData.FlowControl;

            try
            {
                serialPort.Open();
            }
            catch (Exception e)
            {
                var asmatt = (System.Reflection.AssemblyTitleAttribute)Attribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(),
                                                                                                    typeof(System.Reflection.AssemblyTitleAttribute));
                MessageBox.Show(e.Message, asmatt.Title, MessageBoxButton.OK, MessageBoxImage.Error);

                serialPort.Dispose();
                serialPort = null;
            }
        }

        /// <summary>
        /// Closes the serial port connection.
        /// </summary>
        private void CloseSerialPort()
        {
            if (serialPort != null)
            {
                serialPort.Close();
                serialPort = null;
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            CloseSerialPort();

            Application.Current.Shutdown();
        }

        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }
    }
}
