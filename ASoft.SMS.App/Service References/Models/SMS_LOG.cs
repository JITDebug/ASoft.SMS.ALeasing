using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.SMS.App.Models
{
    public class SMS_LOG : BaseDTO
    {
        public long? ID { get; set; }
        public string SMS_TYPE { get; set; }
        public string REF_ID { get; set; }
        public string REF_TYPE { get; set; }
        public int? REF_LOC { get; set; }
        public string IS_SENT { get; set; }
        public DateTime? DATE_CREATED { get; set; }
        public DateTime? DATE_SENT { get; set; }
        public string SENT_MESSAGE { get; set; }
        public string REMARKS { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string REG_NO { get; set; }
        public string MODEL_NAME { get; set; }
        public DateTime? RENT_DATE { get; set; }
        public DateTime? DATE_IN { get; set; }
        public long? TOT_DAYS { get; set; }
        public DateTime? TIME_OUT { get; set; }
        public DateTime? TIME_IN { get; set; }
        public decimal? TOT_INV_AMT { get; set; }
        public DateTime? TRV_DATE { get; set; }
        public DateTime? TRV_TIME { get; set; }
        public string TRV_LOCATION { get; set; }
        public decimal? TRV_AMOUNT { get; set; }
    }

    public class SMS_LOGGetParameter : BaseGetParameter
    {
        public string SMS_TYPE { get; set; }
        public string IS_SENT { get; set; }
    }
}
