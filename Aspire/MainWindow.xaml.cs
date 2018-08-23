using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CsvHelper;

namespace Aspire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 依存プロパティー | Dependency Properties

        /// <summary>
        /// 縦軸の最大値 依存プロパティー | Maximum value on the vertical axis (in millimeters)
        /// </summary>
        public static readonly DependencyProperty VerticalScaleMaxProperty =
                DependencyProperty.Register(
                    "VerticalScaleMax",         // プロパティ名を指定
                    typeof(int),                // プロパティの型を指定
                    typeof(MainWindow),         // プロパティを所有する型を指定
                    new PropertyMetadata(50));  // メタデータを指定。ここではデフォルト値を設定してる

        /// <summary>
        /// 縦軸の最小値 依存プロパティー | Minimum value on the vertical axis (in millimeters)
        /// </summary>
        public static readonly DependencyProperty VerticalScaleMinProperty =
                DependencyProperty.Register(
                    "VerticalScaleMin",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(-50));

        /// <summary>
        /// 横軸の最大値 依存プロパティー | Maximum value on the horizontal axis (in seconds)
        /// </summary>
        public static readonly DependencyProperty HorizontalScaleMaxProperty =
                DependencyProperty.Register(
                    "HorizontalScaleMax",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(60));

        /// <summary>
        /// 横軸の最小値 依存プロパティー | Minimum value on the horizontal axis (in seconds)
        /// </summary>
        public static readonly DependencyProperty HorizontalScaleMinProperty =
                DependencyProperty.Register(
                    "HorizontalScaleMin",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(0));

        #endregion 依存プロパティー | Dependency Properties

        #region Properties

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
        /// Measurement interval per data in msec
        /// (50.0, 100.0, 200.0, 500.0 or 1000.0 msec)
        /// </summary>
        private double measurementInterval = 100.0;

        /// <summary>
        /// シリアルポート | Serial port
        /// </summary>
        private SerialPort serialPort = null;

        /// <summary>
        /// 
        /// </summary>
        private ZX2_SF11 sensor = null;
        
#if USE_DISPATCH_TIMER
        /// <summary>
        /// 
        /// </summary>
        DispatcherTimer dispatchTimer = null;
#else
        /// <summary>
        /// 
        /// </summary>
        System.Threading.Thread measureThread;

        /// <summary>
        /// 
        /// </summary>
        private bool enableMeasurement;
#endif
        /// <summary>
        /// 
        /// </summary>
        private bool measurementRunning = false;

        /// <summary>
        /// 
        /// </summary>
        private PlotViewModel plotViewModel = null;

        /// <summary>
        /// 
        /// </summary>
        private SettingsData config;

        /// <summary>
        /// 
        /// </summary>
        private StreamWriter writer;
        private CsvWriter csv;
        private int dataCount;
        #endregion

        private static System.Timers.Timer aTimer;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            config = SettingsData.Load();
            measurementInterval = Convert.ToDouble(config.MeasurementSettingsData.Interval);
            measurementRunning = false;
            dataCount = 0;

            HorizontalScaleMin = Convert.ToInt16(config.DisplaySettingsData.MinX);
            HorizontalScaleMax = Convert.ToInt16(config.DisplaySettingsData.MaxX);
            VerticalScaleMin = Convert.ToInt16(config.DisplaySettingsData.MinY);
            VerticalScaleMax = Convert.ToInt16(config.DisplaySettingsData.MaxY);

#if USE_DISPATCH_TIMER
            dispatchTimer = new DispatcherTimer();
            dispatchTimer.Interval = TimeSpan.FromMilliseconds(measurementInterval);
            dispatchTimer.Tick += TimerTick;
#else
            enableMeasurement = false;
#endif
            plotViewModel = DataContext as PlotViewModel;
            plotViewModel.TimeFrame = (HorizontalScaleMax - HorizontalScaleMin);
            plotViewModel.Interval = measurementInterval / 1000.0;
#if NOT_USED
            plotViewModel.ActionOccurred += PlotViewModelActionOccurred;
#endif

            sensor = new ZX2_SF11();

            OpenSerialPort();

            MenuMeasurementStart.IsEnabled = true;
            MenuMeasurementStop.IsEnabled = false;

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

        }

        private void SetTimer()
        {
            // Create a 30 second timer 
            aTimer = new System.Timers.Timer(30000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;
            StopMeasurement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimerTick(object sender, EventArgs e)
        {
            StartMeasurement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
#if USE_DISPATCH_TIMER
            dispatchTimer.Stop();
#else
            enableMeasurement = false;
#endif
            CloseSerialPort();
        }

        /// <summary>
        /// Opens a new serial port connection.
        /// </summary>
        private void OpenSerialPort()
        {
            if (serialPort != null)
            {
                MessageBox.Show("Serial port is already opened!", "Serial Port", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
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
            double val = 0.0f;

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
                        if (words[3] != "EEE.EEE") // Data is not invalid?
                        { 
                            val = Convert.ToDouble(words[3]);

                            if (config.MeasurementSettingsData.LogEnabled == "true")
                            {
                                csv.WriteField(dataCount++);
                                csv.WriteField(val);
                                csv.NextRecord();
                            }
#if ENABLE_DATA_PLOTTING
                            // Plot data (or save in CSV file)...
                            this.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                plotViewModel.AddData(val);
                            }));
#endif
                        }
                        else
                        {
                            // Stop dispatch timer
                            dispatchTimer.Stop();

                            MessageBoxResult result = MessageBox.Show("[Error-dark] The received light intensity is insufficient. Please use a suitable workpiece.", "E R R O R", MessageBoxButton.OK, MessageBoxImage.Error);

                            // Restart dispatch timer to continue data aquisition
                            dispatchTimer.Start();
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
                // Stop dispatch timer
                dispatchTimer.Stop();

                if (words[1].Equals("SR"))
                {
                    if (words[2].Equals(sensor.NoSensorConnectedError))
                    {
                        Debug.Print(sensor.NoSensorConnectedError);
                        MessageBoxResult result = MessageBox.Show("No sensor connected!", "SENSOR READ ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (words[2].Equals(sensor.TimeOutError))
                    {
                        Debug.Print(sensor.TimeOutError);
                        MessageBoxResult result = MessageBox.Show("Sensor communications error (time-out error)!", "SENSOR READ ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (words[2].Equals(sensor.ConnectionError))
                    {
                        Debug.Print(sensor.ConnectionError);
                        MessageBoxResult result = MessageBox.Show("Connection unit count error!", "SENSOR READ ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (words[2].Equals(sensor.IllegalCommandError))
                    {
                        Debug.Print(sensor.IllegalCommandError);
                        MessageBoxResult result = MessageBox.Show("Illegal command error!", "SENSOR READ ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (words[2].Equals(sensor.ParameterError))
                    {
                        Debug.Print(sensor.ParameterError);
                        MessageBoxResult result = MessageBox.Show("Parameter error!", "SENSOR READ ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Debug.Print("Unknown Error");
                        MessageBoxResult result = MessageBox.Show("Unknown error!", "SENSOR READ ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if(words[1].Equals("SW"))
                {
                    if (words[2].Equals(sensor.BankSwitching))
                    {
                        Debug.Print(sensor.BankSwitching);
                        MessageBoxResult result = MessageBox.Show("Bank Switching error!", "SENSOR WRITE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if(words[2].Equals(sensor.LDOffStart))
                    {
                        Debug.Print(sensor.LDOffStart);
                        MessageBoxResult result = MessageBox.Show("LD OFF Start error!", "SENSOR WRITE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (words[2].Equals(sensor.LDOffEnd))
                    {
                        Debug.Print(sensor.LDOffEnd);
                        MessageBoxResult result = MessageBox.Show("LD OFF End error!", "SENSOR WRITE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (words[2].Equals(sensor.Bank0HThreshold))
                    {
                        Debug.Print(sensor.Bank0HThreshold);
                        MessageBoxResult result = MessageBox.Show("BANK0 H error!", "SENSOR WRITE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (words[2].Equals(sensor.Bank0LThreshold))
                    {
                        Debug.Print(sensor.Bank0LThreshold);
                        MessageBoxResult result = MessageBox.Show("BANK0 L error!", "SENSOR WRITE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Debug.Print("Unknown Error");
                        MessageBoxResult result = MessageBox.Show("Unknown error!", "SENSOR WRITE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Debug.Print("Unknown Error");
                    MessageBoxResult result = MessageBox.Show("Unknown error!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Restart dispatch timer to continue data aquisition
                dispatchTimer.Start();
            }
            else
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Measurement_Connect_Click(object sender, RoutedEventArgs e)
        {
            OpenSerialPort();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Measurement_Start_Click(object sender, RoutedEventArgs e)
        {
            StartMeasurement_Click(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Measurement_Stop_Click(object sender, RoutedEventArgs e)
        {
            StopMeasurement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Settings_SerialPort_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.Owner = this;
            Nullable<bool> dialogResult = sw.ShowDialog();

            if (dialogResult == true)
            {
                // Close old serial port
                CloseSerialPort();

                // Open new serial port
                OpenSerialPort();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Settings_Measurement_Click(object sender, RoutedEventArgs e)
        {
            MeasurementSettingsWindow sw = new MeasurementSettingsWindow();
            sw.Owner = this;
            Nullable<bool> dialogResult = sw.ShowDialog();

            if(dialogResult == true && measurementRunning == true)
            {
                dispatchTimer.Stop();

                var config = SettingsData.Load();
                measurementInterval = Convert.ToInt16(config.MeasurementSettingsData.Interval);
                dispatchTimer.Interval = TimeSpan.FromMilliseconds(measurementInterval);

                dispatchTimer.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMeasurement_Click(object sender, RoutedEventArgs e)
        {
            writer = File.CreateText("data.csv");
            csv = new CsvWriter(writer);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.Delimiter = ",";

            MenuMeasurementStart.IsEnabled = false;
            MenuMeasurementStop.IsEnabled = true;

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            var config = SettingsData.Load();
            measurementInterval = Convert.ToInt16(config.MeasurementSettingsData.Interval);
            plotViewModel.Interval = measurementInterval / 1000.0;
#if USE_DISPATCH_TIMER
            dispatchTimer.Interval = TimeSpan.FromMilliseconds(measurementInterval);
            dispatchTimer.Start();
#else
            measureThread = new System.Threading.Thread(new System.Threading.ThreadStart(StartMeasurement));
            measureThread.Start();
#endif
            // TODO: Timer starts here...
            SetTimer();

            measurementRunning = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMeasurement_Click(object sender, RoutedEventArgs e)
        {
            StopMeasurement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearMeasurement_Click(object sender, RoutedEventArgs e)
        {
            ClearPlotView();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartMeasurement()
        {
            string command;
            command = "SR,01," + sensor.MeasuredValue + Environment.NewLine;
#if USE_DISPATCH_TIMER
            serialPort.Write(command);
#else
            enableMeasurement = true;

            while (enableMeasurement)
            {
                serialPort.Write(command);
                System.Threading.Thread.Sleep(measurementInterval);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        private void StopMeasurement()
        {
#if USE_DISPATCH_TIMER
            dispatchTimer.Stop();
#else
            enableMeasurement = false;
#endif
            measurementRunning = false;

            MenuMeasurementStart.IsEnabled = true;
            MenuMeasurementStop.IsEnabled = false;

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            dataCount = 0;
            writer.Close();
        }

        /// <summary>
        /// Clears data on plot view
        /// </summary>
        private void ClearPlotView()
        {
#if USE_DISPATCH_TIMER

#else
            enableMeasurement = false;
#endif
            plotViewModel.ClearAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_View_ClearGraph_Click(object sender, RoutedEventArgs e)
        {
            ClearPlotView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_View_AdjustGraph_Click(object sender, RoutedEventArgs e)
        {
            DisplaySettingsWindow sw = new DisplaySettingsWindow();
            sw.Owner = this;
            Nullable<bool> dialogResult = sw.ShowDialog();

            if (dialogResult == true)
            {
                var config = SettingsData.Load();
                HorizontalScaleMin = Convert.ToInt16(config.DisplaySettingsData.MinX);
                HorizontalScaleMax = Convert.ToInt16(config.DisplaySettingsData.MaxX);
                VerticalScaleMin = Convert.ToInt16(config.DisplaySettingsData.MinY);
                VerticalScaleMax = Convert.ToInt16(config.DisplaySettingsData.MaxY);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {

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
