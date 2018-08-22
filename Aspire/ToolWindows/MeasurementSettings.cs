using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire
{
    public class MeasurementSettings
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement("Interval")]
        public string Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        private string _interval = "100";

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement("LogEnabled")]
        public string LogEnabled
        {
            get { return _logEnabled; }
            set { _logEnabled = value; }
        }

        private string _logEnabled = "false";
    }
}
