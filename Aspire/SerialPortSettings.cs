using System.IO.Ports;

namespace Aspire
{
    [System.Xml.Serialization.XmlRoot("SerialPortSettings")]
    public class SerialPortSettings
    {
        /// <summary>
        /// ポート番号
        /// </summary>
        [System.Xml.Serialization.XmlElement("PortNum")]
        public string PortNum
        {
            get { return _portNum; }
            set { _portNum = value; }
        }

        private string _portNum = "COM1";

        /// <summary>
        /// ボーレート
        /// </summary>
        [System.Xml.Serialization.XmlElement("BaudRate")]
        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        private int _baudRate = 38400;

        /// <summary>
        /// パリティ
        /// </summary>
        [System.Xml.Serialization.XmlElement("Parity")]
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        private Parity _parity = Parity.None;

        /// <summary>
        /// ストップビット
        /// </summary>
        [System.Xml.Serialization.XmlElement("StopBit")]
        public StopBits StopBit
        {
            get { return _stopBit; }
            set { _stopBit = value; }
        }

        private StopBits _stopBit = StopBits.One;

        /// <summary>
        /// データビット
        /// </summary>
        [System.Xml.Serialization.XmlElement("Databit")]
        public int Databit
        {
            get { return _dataBit; }
            set { _dataBit = value; }
        }

        private int _dataBit = 8;

        /// <summary>
        /// フロー制御
        /// </summary>
        [System.Xml.Serialization.XmlElement("FlowControl")]
        public Handshake FlowControl
        {
            get { return _flowControl; }
            set { _flowControl = value; }
        }

        private Handshake _flowControl = Handshake.None;
    }
}
