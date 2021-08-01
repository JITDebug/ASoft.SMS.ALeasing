using ASoft.SMS.App.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Net.Http;

namespace ASoft.SMS.App.Services
{
    public class MainService : BaseService
    {
        public string Start()
        {
            string value = "";

            try
            {
                ConsoleLog("Connecting to server...");

                using (var cnn = OpenConnection())
                {
                    SMS_LOGService slService = new SMS_LOGService();
                    Settings settings = GetSettings();

                    SendSMS_AGMT(slService, settings, cnn);
                    SendSMS_AGMT_CLOSE(slService, settings, cnn);
                    SendSMS_TRV(slService, settings, cnn);

                    ConsoleLog("Closing server connection...");
                    cnn.Close();
                }

                ConsoleLog("Completed.");
            }
            catch (Exception ex)
            {
                value = CreateErrorResponseMessage(ex);
            }

            return value;
        }

        private void SendSMS_AGMT(SMS_LOGService slService, Settings settings, OracleConnection cnn)
        {
            ConsoleLog("Sending SMS for new rental agreements...");

            BaseDTOCollection<SMS_LOG> sls = slService.Get(new SMS_LOGGetParameter() { SMS_TYPE = "AGMT", IS_SENT = "N" }, cnn);

            foreach (SMS_LOG sl in sls.Items)
            {
                ConsoleLog($@"Loading details for [{ sl.ID }] { sl.REF_TYPE } No.'{ sl.REF_ID }'...");

                if (!string.IsNullOrEmpty(sl.MOBILE_NO))
                {
                    sl.SENT_MESSAGE = settings.SMS.Message.Template1;
                    slService.FinalizeModel(sl, cnn);
                    ConsoleLog($@"Trying to send message to ({ sl.MOBILE_NO })...");

                    SendSMS(slService, sl, settings, cnn);
                }  
            }
        }

        private void SendSMS_AGMT_CLOSE(SMS_LOGService slService, Settings settings, OracleConnection cnn)
        {
            ConsoleLog("Sending SMS for closing rental agreements...");

            BaseDTOCollection<SMS_LOG> sls = slService.Get(new SMS_LOGGetParameter() { SMS_TYPE = "AGMT-CLOSE", IS_SENT = "N" }, cnn);

            foreach (SMS_LOG sl in sls.Items)
            {
                ConsoleLog($@"Loading details for [{ sl.ID }] { sl.REF_TYPE } No.'{ sl.REF_ID }'...");

                if (!string.IsNullOrEmpty(sl.MOBILE_NO))
                {
                    sl.SENT_MESSAGE = settings.SMS.Message.Template2;
                    slService.FinalizeModel(sl, cnn);
                    ConsoleLog($@"Trying to send message to ({ sl.MOBILE_NO })...");

                    SendSMS(slService, sl, settings, cnn);
                }
            }
        }

        private void SendSMS_TRV(SMS_LOGService slService, Settings settings, OracleConnection cnn)
        {
            ConsoleLog("Sending SMS for traffic violation...");

            BaseDTOCollection<SMS_LOG> sls = slService.Get(new SMS_LOGGetParameter() { SMS_TYPE = "TRV", IS_SENT = "N" }, cnn);

            foreach (SMS_LOG sl in sls.Items)
            {
                ConsoleLog($@"Loading details for [{ sl.ID }] { sl.REF_TYPE } No.'{ sl.REF_ID }'...");

                if (!string.IsNullOrEmpty(sl.MOBILE_NO))
                {
                    sl.SENT_MESSAGE = settings.SMS.Message.Template3;
                    slService.FinalizeModel(sl, cnn);
                    ConsoleLog($@"Trying to send message to ({ sl.MOBILE_NO })...");

                    SendSMS(slService, sl, settings, cnn);
                }
            }
        }

        private bool SendSMS(SMS_LOGService slService, SMS_LOG sl, Settings settings, OracleConnection cnn)
        {
            bool isSent = false;
            sl.DATE_SENT = DateTime.Now;

            try
            {
                string url = $@"{ settings.SMS.Url }?
                                user={ settings.SMS.Name }&
                                password={ settings.SMS.Password }&
                                api_id={ settings.SMS.CustomerID }&
                                to={ sl.MOBILE_NO }&
                                text={ sl.SENT_MESSAGE }&
                                from={ settings.SMS.Originator }&
                                notify_id={ sl.ID }";

                HttpClient httpClient = new HttpClient();
                string response = httpClient.GetStringAsync(url).Result;

                if (!string.IsNullOrEmpty(response))
                {
                    sl.REMARKS = response;
                    if (response == "OK")
                        sl.IS_SENT = "Y";
                    else
                        sl.IS_SENT = "F";
                }
            }
            catch (Exception ex)
            {
                sl.REMARKS = CreateErrorResponseMessage(ex);
                sl.IS_SENT = "F";
            }

            slService.Post(sl, cnn);

            return isSent;
        }

        private void ConsoleLog(string value)
        {
            Console.WriteLine($@"{ DateTime.Now.ToString("yyMMddHHmmss") } >> { value }");
        }
    }
}
