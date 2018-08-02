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
        #region 依存プロパティー

        /// <summary>
        /// 縦軸の最大値 依存プロパティー | Maximum value on the vertical axis
        /// </summary>
        public static readonly DependencyProperty VerticalScaleMaxProperty =
                DependencyProperty.Register(
                    "VerticalScaleMax",             // プロパティ名を指定
                    typeof(int),                    // プロパティの型を指定
                    typeof(MainWindow),             // プロパティを所有する型を指定
                    new PropertyMetadata(999));     // メタデータを指定。ここではデフォルト値を設定してる

        /// <summary>
        /// 縦軸の最小値 依存プロパティー | Minimum value on the vertical axis
        /// </summary>
        public static readonly DependencyProperty VerticalScaleMinProperty =
                DependencyProperty.Register(
                    "VerticalScaleMin",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(-99));

        /// <summary>
        /// 横軸の最大値 依存プロパティー | Maximum value on the horizontal axis
        /// </summary>
        public static readonly DependencyProperty HorizontalScaleMaxProperty =
                DependencyProperty.Register(
                    "HorizontalScaleMax",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(60));

        /// <summary>
        /// 横軸の最小値 依存プロパティー | Minimum value on the horizontal axis
        /// </summary>
        public static readonly DependencyProperty HorizontalScaleMinProperty =
                DependencyProperty.Register(
                    "HorizontalScaleMin",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(0));

        #endregion 依存プロパティー

        /// <summary>
        /// 縦軸スケールの最大値 | Maximum value of vertical scale
        /// </summary>
        public int VerticalScaleMax
        {
            get { return (int)GetValue(VerticalScaleMaxProperty); }
            set { SetValue(VerticalScaleMaxProperty, value); }
        }

        /// <summary>
        /// 縦軸スケールの最小値 | Minimum value of vertical scale
        /// </summary>
        public int VerticalScaleMin
        {
            get { return (int)GetValue(VerticalScaleMinProperty); }
            set { SetValue(VerticalScaleMinProperty, value); }
        }

        /// <summary>
        /// 横軸スケールの最大値 | Maximum value on the horizontal axis scale
        /// </summary>
        public int HorizontalScaleMax
        {
            get { return (int)GetValue(HorizontalScaleMaxProperty); }
            set { SetValue(HorizontalScaleMaxProperty, value); }
        }

        /// <summary>
        /// 横軸スケールの最小値 | Minimum value on the horizontal axis scale
        /// </summary>
        public int HorizontalScaleMin
        {
            get { return (int)GetValue(HorizontalScaleMinProperty); }
            set { SetValue(HorizontalScaleMinProperty, value); }
        }

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
        private PlotViewModel plotViewModel = null;


        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            enableMeasurement = false;

            plotViewModel = DataContext as PlotViewModel;
            plotViewModel.MaxCount = (HorizontalScaleMax - HorizontalScaleMin);

            sensor = new ZX2_SF11();

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

            // Process received data...
            ParseString(indata);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ParseString(string data)
        {
            char[] delimiterChars = { ',' };
            string[] words = data.Split(delimiterChars);

            if (words[0].Equals("SR")) // Response OK
            {
                if (words[1].Equals("01")) // Channel 1
                {
                    if (words[2].Equals(sensor.BankSwitching))
                    {
                        Debug.Print(sensor.BankSwitching);
                    }
                    else if (words[2].Equals(sensor.LDOffStart))
                    {
                        Debug.Print(sensor.LDOffStart);
                    }
                    else if (words[2].Equals(sensor.LDOffEnd))
                    {
                        Debug.Print(sensor.LDOffEnd);
                    }
                    else if (words[2].Equals(sensor.Bank0HThreshold))
                    {
                        Debug.Print(sensor.Bank0HThreshold);
                    }
                    else if (words[2].Equals(sensor.Bank0LThreshold))
                    {
                        Debug.Print(sensor.Bank0LThreshold);
                    }
                    else if (words[2].Equals(sensor.MeasuredValue))
                    {
                        Debug.Print(sensor.MeasuredValue);
                        double val = Convert.ToDouble(words[3]);

                        // Plot data or save in CSV file...
                        if (true)
                        {
                            this.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                plotViewModel.AddData(val, val, val);
                            }));
                        }

                    }
                    else if (words[2].Equals(sensor.SoftwareVersion))
                    {
                        Debug.Print(sensor.SoftwareVersion);
                    }
                    else
                    {
                        Debug.Print("Unknown Error");
                    }
                }
            }
            else if (words[0].Equals("ER")) // Response Error
            {
                if (words[1].Equals("SR"))
                {
                    if (words[2].Equals(sensor.NoSensorConnectedError))
                    {
                        Debug.Print(sensor.NoSensorConnectedError);
                    }
                    else if (words[2].Equals(sensor.TimeOutError))
                    {
                        Debug.Print(sensor.TimeOutError);
                    }
                    else if (words[2].Equals(sensor.ConnectionError))
                    {
                        Debug.Print(sensor.ConnectionError);
                    }
                    else if (words[2].Equals(sensor.IllegalCommandError))
                    {
                        Debug.Print(sensor.IllegalCommandError);
                    }
                    else if (words[2].Equals(sensor.ParameterError))
                    {
                        Debug.Print(sensor.ParameterError);
                    }
                    else
                    {
                        Debug.Print("Unknown Error");
                    }
                }
                else if(words[1].Equals("SW"))
                {
                    if (words[2].Equals(sensor.BankSwitching))
                    {
                        Debug.Print(sensor.BankSwitching);
                    }
                    else if(words[2].Equals(sensor.LDOffStart))
                    {
                        Debug.Print(sensor.LDOffStart);
                    }
                    else if (words[2].Equals(sensor.LDOffEnd))
                    {
                        Debug.Print(sensor.LDOffEnd);
                    }
                    else if (words[2].Equals(sensor.Bank0HThreshold))
                    {
                        Debug.Print(sensor.Bank0HThreshold);
                    }
                    else if (words[2].Equals(sensor.Bank0LThreshold))
                    {
                        Debug.Print(sensor.Bank0LThreshold);
                    }
                    else if (words[2].Equals(sensor.MeasuredValue))
                    {
                        Debug.Print(sensor.MeasuredValue);
                    }
                    else if (words[2].Equals(sensor.SoftwareVersion))
                    {
                        Debug.Print(sensor.SoftwareVersion);
                    }
                    else
                    {
                        Debug.Print("Unknown Error");
                    }
                }
                else
                {
                    Debug.Print("Unknown Error");
                }
            }
            else
            {

            }

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
            command = "SR,01," + sensor.BankSwitching + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string command;
            command = "SR,01," + sensor.LDOffStart + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string command;
            command = "SR,01," + sensor.LDOffEnd + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string command;
            command = "SR,01," + sensor.Bank0HThreshold + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string command;
            command = "SR,01," + sensor.Bank0LThreshold + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            string command;
            command = "SR,01," + sensor.MeasuredValue + Environment.NewLine;
            Debug.Print(command);
            serialPort.Write(command);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            string command;
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
