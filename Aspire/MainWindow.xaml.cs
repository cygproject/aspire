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
        
        /// <summary>
        /// 
        /// </summary>
        System.Threading.Thread measureThread;

        /// <summary>
        /// 
        /// </summary>
        private bool enableMeasurement;


        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            enableMeasurement = false;

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

            // Data received event
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();

            char[] trimChars = { '\r', '\n' };
            indata = indata.TrimEnd(trimChars);
            Debug.Print(indata);

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            enableMeasurement = false;

            CloseSerialPort();

            Application.Current.Shutdown();
        }

        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.Owner = this;
            sw.ShowDialog();
        }

        private void StartMeasurement_Click(object sender, RoutedEventArgs e)
        {
#if false
            StartMeasurement();
#else
            measureThread = new System.Threading.Thread(new System.Threading.ThreadStart(StartMeasurement));
            measureThread.Start();
#endif
        }

        private void StopMeasurement_Click(object sender, RoutedEventArgs e)
        {
            enableMeasurement = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartMeasurement()
        {
            string command;
            enableMeasurement = true;

            while (enableMeasurement)
            {
#if CRLF
                command = "SR,01," + DATA_NO_519_MEASURED_VALUE + CR + LF;
#else
                sensor = new ZX2_SF11();
                command = "SR,01," + sensor.MeasuredValue + Environment.NewLine;
#endif
                serialPort.Write(command);

                System.Threading.Thread.Sleep(1000);
            }
        }

        #region For Test Only

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

        private void NoSensorConnected_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SR,00" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void TimeoutError_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SR,02" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void ConnectionError_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SR,20" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void IllegalCommandError_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SR,30" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void ParameterError_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SR,31" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Error107_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SW,107" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Error400_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SW,400" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Error401_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SW,401" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Error132_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SW,132" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Error133_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SW,133" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Error519_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SW,519" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Error580_Click(object sender, RoutedEventArgs e)
        {
            string command;
            command = "ER,SW,580" + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        #endregion

    }
}
