using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
            // Appの場所を覚えておく
            string appPath = Environment.GetCommandLineArgs()[0];
            string appFullPath = System.IO.Path.GetFullPath(appPath);
            string appDirectoryName = System.IO.Path.GetDirectoryName(appFullPath);
            DirectoryName = appDirectoryName;

            // Appの言語を設定する
            SetLanguageDictionary();
        }

        /// <summary>
        /// Appの言語を設定する（en-US, ja-JP）
        /// </summary>
        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    //dict.Source = new Uri("..\\ResourceDictionary\\Dictionary.en-US.xaml", UriKind.Relative);
                    dict.Source = new Uri("/Aspire;component/ResourceDictionary/Dictionary.en-US.xaml", UriKind.Relative);
                    
                    break;
                   
                case "ja-JP":
                default:
                    //dict.Source = new Uri("..\\ResourceDictionary\\Dictionary.ja-JP.xaml", UriKind.Relative);
                    dict.Source = new Uri("/Aspire;component/ResourceDictionary/Dictionary.ja-JP.xaml", UriKind.Relative);
                    break;
            }

            this.Resources.MergedDictionaries.Add(dict);
        }
    }
}
