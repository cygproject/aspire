using System;
using System.IO;
using System.Xml.Serialization;

namespace Aspire
{
    [System.Xml.Serialization.XmlRoot("SettingsData")]
    public class SettingsData
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        private static SettingsData _currentSettings = null;

        /// <summary>
        /// シリアルポート設定
        /// </summary>
        public SerialPortSettings SerialPortSettingsData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MeasurementSettings MeasurementSettingsData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DisplaySettings DisplaySettingsData { get; set; }

        /// <summary>
        ///
        /// </summary>
        private const string SETTINGS_XML_FILE_NAME = "Settings.xml";

        /// <summary>
        /// インスタンスの取得
        /// </summary>
        /// <returns></returns>
        public static SettingsData GetInstance()
        {
            if (_currentSettings == null)
            {
                _currentSettings = SettingsData.Load();
                if (_currentSettings == null)
                {
                    _currentSettings = new SettingsData();
                }
            }

            return _currentSettings;
        }

        /// <summary>
        /// XMLロード
        /// </summary>
        /// <returns></returns>
        public static SettingsData Load()
        {
            FileStream fs = null;
            SettingsData model = null;

            App app = App.Current as App;
            string file = Path.Combine(app.DirectoryName, SETTINGS_XML_FILE_NAME);

            try
            {
                fs = new FileStream(file, System.IO.FileMode.Open);
                var serializer = new XmlSerializer(typeof(SettingsData));
                model = (SettingsData)serializer.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                model = new SettingsData();
                if (fs != null) fs.Close();
            }

            return model;
        }

        /// <summary>
        /// XML保存
        /// </summary>
        /// <param name="data"></param>
        public void Save()
        {
            //出力先XMLのストリーム
            App app = App.Current as App;
            string file = Path.Combine(app.DirectoryName, SETTINGS_XML_FILE_NAME);

            FileStream stream = new FileStream(file, System.IO.FileMode.Create);
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);

            XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
            serializer.Serialize(writer, this);

            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SettingsData()
        {
            //シングルトン
            SerialPortSettingsData = new SerialPortSettings();
            MeasurementSettingsData = new MeasurementSettings();
            DisplaySettingsData = new DisplaySettings();
        }
    }
}
