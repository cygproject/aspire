using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspire
{
    public class ZX2_SF11
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ZX2_SF11()
        {

        }

        private const string DATA_NO_107_BANK_SWITCHING = "107";
        private const string DATA_NO_400_LD_OFF_START = "400";
        private const string DATA_NO_401_LD_OFF_END = "401";
        private const string DATA_NO_132_BANK0_H_THRESHOLD = "132";
        private const string DATA_NO_133_BANK0_L_THRESHOLD = "133";
        private const string DATA_NO_166_BANK1_H_THRESHOLD = "166";
        private const string DATA_NO_167_BANK1_L_THRESHOLD = "167";
        private const string DATA_NO_196_BANK2_H_THRESHOLD = "196";
        private const string DATA_NO_197_BANK2_L_THRESHOLD = "197";
        private const string DATA_NO_228_BANK3_H_THRESHOLD = "228";
        private const string DATA_NO_229_BANK3_L_THRESHOLD = "229";
        private const string DATA_NO_519_MEASURED_VALUE = "519";
        private const string DATA_NO_580_SOFTWARE_VERSION = "580";

        private const string ERR_NO_SENSOR_CONNECTED = "00";
        private const string ERR_TIME_OUT_ERROR = "02";
        private const string ERR_CONNECTION_ERROR = "20";
        private const string ERR_ILLEGAL_COMMAND_ERROR = "30";
        private const string ERR_PARAMETER_ERROR = "31";


        public string BankSwitching { get { return DATA_NO_107_BANK_SWITCHING; } }
        public string LDOffStart { get { return DATA_NO_400_LD_OFF_START; } }
        public string LDOffEnd { get { return DATA_NO_401_LD_OFF_END; } }

        public string Bank0HThreshold { get { return DATA_NO_132_BANK0_H_THRESHOLD; } }
        public string Bank0LThreshold { get { return DATA_NO_133_BANK0_L_THRESHOLD; } }

        public string MeasuredValue { get { return DATA_NO_519_MEASURED_VALUE; } }
        public string SoftwareVersion { get { return DATA_NO_580_SOFTWARE_VERSION; } }

        public string NoSensorConnectedError { get { return ERR_NO_SENSOR_CONNECTED; } }
        public string TimeOutError { get { return ERR_TIME_OUT_ERROR; } }
        public string ConnectionError { get { return ERR_CONNECTION_ERROR; } }
        public string IllegalCommandError { get { return ERR_ILLEGAL_COMMAND_ERROR; } }
        public string ParameterError { get { return ERR_PARAMETER_ERROR; } }

    }
}
