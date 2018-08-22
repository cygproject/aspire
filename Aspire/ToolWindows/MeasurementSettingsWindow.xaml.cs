using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Aspire
{
    /// <summary>
    /// Interaction logic for MeasurementSettingsWindow.xaml
    /// </summary>
    public partial class MeasurementSettingsWindow : Window
    {
        public MeasurementSettingsWindow()
        {
            InitializeComponent();

            cmbInterval.ItemsSource = new string[] {
                "50",
                "100",
                "200",
                "500",
                "1000"
            };

            cmbEnableLog.ItemsSource = new string[] {
                "true",
                "false",
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var config = SettingsData.Load();
            cmbInterval.Text = config.MeasurementSettingsData.Interval.ToString();
            cmbEnableLog.Text = config.MeasurementSettingsData.LogEnabled.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var config = SettingsData.Load();
            config.MeasurementSettingsData.Interval = cmbInterval.Text;
            config.MeasurementSettingsData.LogEnabled = cmbEnableLog.Text;
            config.Save();

            // Accept the dialog and return the dialog result
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Accept the dialog and return the dialog result
            this.DialogResult = false;
            this.Close();
        }
    }
}
