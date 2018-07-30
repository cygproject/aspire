using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// シリアルポート | Serial port
        /// </summary>
        private SerialPort serialPort = null;

        /// <summary>
        /// 
        /// </summary>
        private ZX2_SF11 sensor = null;
        

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string command;

            sensor = new ZX2_SF11();
            command = "SR,01," + sensor.BankSwitching + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string command;
            sensor = new ZX2_SF11();
            command = "SR,01," + sensor.LDOffStart + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string command;
            sensor = new ZX2_SF11();
            command = "SR,01," + sensor.LDOffEnd + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string command;
            sensor = new ZX2_SF11();
            command = "SR,01," + sensor.Bank0HThreshold + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string command;
            sensor = new ZX2_SF11();
            command = "SR,01," + sensor.Bank0LThreshold + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            string command;
            sensor = new ZX2_SF11();
            command = "SR,01," + sensor.MeasuredValue + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            string command;
            sensor = new ZX2_SF11();
            command = "SR,01," + sensor.SoftwareVersion + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

    }
}
