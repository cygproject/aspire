using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire
{
    public class DisplaySettings
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement("MinX")]
        public string MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        private string _minX = "0";

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement("MaxX")]
        public string MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        private string _maxX = "30";

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement("MinY")]
        public string MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        private string _minY = "0";

        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlElement("MaxY")]
        public string MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        private string _maxY = "15";
    }
}
