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

        private string _interval = "1000";
    }
}
