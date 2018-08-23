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
    /// Interaction logic for DisplaySettingsWindow.xaml
    /// </summary>
    public partial class DisplaySettingsWindow : Window
    {
        public DisplaySettingsWindow()
        {
            InitializeComponent();

            cmbMinX.ItemsSource = new string[] {
                "0",
            };

            cmbMaxX.ItemsSource = new string[] {
                "30",
                "60",
                "120",
                "300"
            };

            cmbMinY.ItemsSource = new string[] {
                "0",
                "-5",
                "-10",
            };

            cmbMaxY.ItemsSource = new string[] {
                "5",
                "10",
                "15",
                "20",
            };

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var config = SettingsData.Load();
            cmbMinX.Text = config.DisplaySettingsData.MinX.ToString();
            cmbMaxX.Text = config.DisplaySettingsData.MaxX.ToString();
            cmbMinY.Text = config.DisplaySettingsData.MinY.ToString();
            cmbMaxY.Text = config.DisplaySettingsData.MaxY.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var config = SettingsData.Load();
            config.DisplaySettingsData.MinX = cmbMinX.Text;
            config.DisplaySettingsData.MaxX = cmbMaxX.Text;
            config.DisplaySettingsData.MinY = cmbMinY.Text;
            config.DisplaySettingsData.MaxY = cmbMaxY.Text;
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
