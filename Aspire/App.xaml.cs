using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Aspire
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Exeのあるフォルダ
        /// </summary>
        public string DirectoryName { get; private set; }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            //Appの場所を覚えておく
            string appPath = Environment.GetCommandLineArgs()[0];
            string appFullPath = System.IO.Path.GetFullPath(appPath);
            string appDirectoryName = System.IO.Path.GetDirectoryName(appFullPath);
            DirectoryName = appDirectoryName;
        }
    }
}
