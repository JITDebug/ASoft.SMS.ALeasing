using ASoft.SMS.App.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ASoft.SMS.App.Services
{
    public class SMS_LOGService : BaseService
    {
        public BaseDTOCollection<SMS_LOG> Get(SMS_LOGGetParameter param)
        {
            BaseDTOCollection<SMS_LOG> data = null;

            using (var cnn = OpenConnection())
            {
                data = Get(param, cnn);

                cnn.Close();
            };

            return data;
        }

        public SMS_LOG GetNew()
        {
            return new SMS_LOG();
        }

        public SMS_LOG Get(long id)
        {
            SMS_LOG data = null;

            using (var cnn = OpenConnection())
            {
                data = Get(id, cnn);

                cnn.Close();
            };

            return data;
        }


        public SMS_LOG Post(SMS_LOG model)
        {
            using (var cnn = OpenConnection())
            {
                model = Post(model, cnn);

                cnn.Close();
            }

            return model;
        }

        internal BaseDTOCollection<SMS_LOG> Get(SMS_LOGGetParameter param, OracleConnection cnn)
        {
            string query = "SELECT * FROM SMS_LOG WHERE ROWNUM > 0";

            //only include SMS_LOG with MOBILE_NO
            query += " AND MOBILE_NO IS NOT NULL";

            int? pageSize = null;
            int? skip = null;

            if (param != null)
            {
                if (!string.IsNullOrEmpty(param.LookupText))
                {
                    param.LookupText = Regex.Replace(param.LookupText, $@"[^a-zA-Z0-9_.\s]+", "", RegexOptions.Compiled);

                    query += $@"
                    AND (
                           TO_CHAR(REF_ID) LIKE UPPER('%{ param.LookupText }%')
                        OR UPPER(REF_TYPE) LIKE UPPER('%{ param.LookupText }%')
                    )";
                }

                if (!string.IsNullOrEmpty(param.IS_SENT))
                    query += $@" AND IS_SENT = '{ param.IS_SENT }'";

                if (!string.IsNullOrEmpty(param.SMS_TYPE))
                    query += $@" AND SMS_TYPE = '{ param.SMS_TYPE }'";

                pageSize = param.PageSize;
                skip = param.Skip;
            }

            return Get<SMS_LOG>(query, cnn, null, "REF_LOC, REF_TYPE, REF_ID", pageSize, skip);
        }

        internal SMS_LOG Get(long id, OracleConnection cnn)
        {
            string query = $@"SELECT * FROM SMS_LOG WHERE ID = :ID";

            List<OracleParameter> parameters = new List<OracleParameter>
            {
                new OracleParameter("ID", OracleDbType.Int64, id, ParameterDirection.Input)
            };

            return Get<SMS_LOG>(query, cnn, parameters).Items.FirstOrDefault();
        }


        internal SMS_LOG Post(SMS_LOG model, OracleConnection cnn)
        {
            FinalizeModel(model, cnn);
            if (ModelIsValid(model, cnn))
            {
                Save(model, cnn);
            }

            return model;
        }


        internal void Save(SMS_LOG model, OracleConnection cnn)
        {
            string query;
            if (Exists(model.ID ?? 0, cnn))
            {
                #region update query

                query = $@"
                UPDATE SMS_LOG SET
                    ID = :ID,
                    SMS_TYPE = :SMS_TYPE,
                    REF_ID = :REF_ID,
                    REF_TYPE = :REF_TYPE,
                    REF_LOC = :REF_LOC,
                    IS_SENT = :IS_SENT,
                    DATE_CREATED = :DATE_CREATED,
                    DATE_SENT = :DATE_SENT,
                    SENT_MESSAGE = :SENT_MESSAGE,
                    REMARKS = :REMARKS,
                    TRANSACTION_ID = :TRANSACTION_ID,
                    MOBILE_NO = :MOBILE_NO,
                    REG_NO = :REG_NO,
                    MODEL_NAME = :MODEL_NAME,
                    RENT_DATE = :RENT_DATE,
                    DATE_IN = :DATE_IN,
                    TOT_DAYS = :TOT_DAYS,
                    TIME_OUT = :TIME_OUT,
                    TIME_IN = :TIME_IN,
                    TOT_INV_AMT = :TOT_INV_AMT,
                    TRV_DATE = :TRV_DATE,
                    TRV_TIME = :TRV_TIME,
                    TRV_LOCATION = :TRV_LOCATION,
                    TRV_AMOUNT = :TRV_AMOUNT
                    WHERE ID = :ID";

                #endregion
            }
            else
            {
                #region insert query

                query = @"
                INSERT INTO SMS_LOG (
                    ID, 
                    SMS_TYPE, 
                    REF_ID, 
                    REF_TYPE, 
                    REF_LOC, 
                    IS_SENT, 
                    DATE_CREATED, 
                    DATE_SENT, 
                    SENT_MESSAGE, 
                    REMARKS, 
                    TRANSACTION_ID, 
                    MOBILE_NO, 
                    REG_NO, 
                    MODEL_NAME, 
                    RENT_DATE, 
                    DATE_IN, 
                    TOT_DAYS, 
                    TIME_OUT, 
                    TIME_IN, 
                    TOT_INV_AMT, 
                    TRV_DATE, 
                    TRV_TIME, 
                    TRV_LOCATION, 
                    TRV_AMOUNT
                ) VALUES (
                    :ID, 
                    :SMS_TYPE, 
                    :REF_ID, 
                    :REF_TYPE, 
                    :REF_LOC, 
                    :IS_SENT, 
                    :DATE_CREATED, 
                    :DATE_SENT, 
                    :SENT_MESSAGE, 
                    :REMARKS, 
                    :TRANSACTION_ID, 
                    :MOBILE_NO, 
                    :REG_NO, 
                    :MODEL_NAME, 
                    :RENT_DATE, 
                    :DATE_IN, 
                    :TOT_DAYS, 
                    :TIME_OUT, 
                    :TIME_IN, 
                    :TOT_INV_AMT, 
                    :TRV_DATE, 
                    :TRV_TIME, 
                    :TRV_LOCATION, 
                    :TRV_AMOUNT
                )";

                #endregion
            }

            #region parameters

            List<OracleParameter> parameters = new List<OracleParameter>
            {
                new OracleParameter("ID", OracleDbType.Int64, model.ID, ParameterDirection.Input),
                new OracleParameter("SMS_TYPE", OracleDbType.Varchar2, model.SMS_TYPE, ParameterDirection.Input),
                new OracleParameter("REF_ID", OracleDbType.Varchar2, model.REF_ID, ParameterDirection.Input),
                new OracleParameter("REF_TYPE", OracleDbType.Varchar2, model.REF_TYPE, ParameterDirection.Input),
                new OracleParameter("REF_LOC", OracleDbType.Int32, model.REF_LOC, ParameterDirection.Input),
                new OracleParameter("IS_SENT", OracleDbType.Varchar2, model.IS_SENT, ParameterDirection.Input),
                new OracleParameter("DATE_CREATED", OracleDbType.Date, model.DATE_CREATED, ParameterDirection.Input),
                new OracleParameter("DATE_SENT", OracleDbType.Date, model.DATE_SENT, ParameterDirection.Input),
                new OracleParameter("SENT_MESSAGE", OracleDbType.Varchar2, model.SENT_MESSAGE, ParameterDirection.Input),
                new OracleParameter("REMARKS", OracleDbType.Varchar2, model.REMARKS, ParameterDirection.Input),
                new OracleParameter("TRANSACTION_ID", OracleDbType.Varchar2, model.TRANSACTION_ID, ParameterDirection.Input),
                new OracleParameter("MOBILE_NO", OracleDbType.Varchar2, model.MOBILE_NO, ParameterDirection.Input),
                new OracleParameter("REG_NO", OracleDbType.Varchar2, model.REG_NO, ParameterDirection.Input),
                new OracleParameter("MODEL_NAME", OracleDbType.Varchar2, model.MODEL_NAME, ParameterDirection.Input),
                new OracleParameter("RENT_DATE", OracleDbType.Date, model.RENT_DATE, ParameterDirection.Input),
                new OracleParameter("DATE_IN", OracleDbType.Date, model.DATE_IN, ParameterDirection.Input),
                new OracleParameter("TOT_DAYS", OracleDbType.Int64, model.TOT_DAYS, ParameterDirection.Input),
                new OracleParameter("TIME_OUT", OracleDbType.Date, model.TIME_OUT, ParameterDirection.Input),
                new OracleParameter("TIME_IN", OracleDbType.Date, model.TIME_IN, ParameterDirection.Input),
                new OracleParameter("TOT_INV_AMT", OracleDbType.Decimal, model.TOT_INV_AMT, ParameterDirection.Input),
                new OracleParameter("TRV_DATE", OracleDbType.Date, model.TRV_DATE, ParameterDirection.Input),
                new OracleParameter("TRV_TIME", OracleDbType.Date, model.TRV_TIME, ParameterDirection.Input),
                new OracleParameter("TRV_LOCATION", OracleDbType.Varchar2, model.TRV_LOCATION, ParameterDirection.Input),
                new OracleParameter("TRV_AMOUNT", OracleDbType.Decimal, model.TRV_AMOUNT, ParameterDirection.Input)
            };

            #endregion

            Post(query, cnn, parameters);
        }

        internal bool Exists(long id, OracleConnection cnn)
        {
            return Get(id, cnn) != null;
        }

        internal void FinalizeModel(SMS_LOG model, OracleConnection cnn)
        {
            FinalizeMOBILE_NO(model);
            FinalizeSENT_MESSAGE(model);
        }

        internal void FinalizeMOBILE_NO(SMS_LOG model)
        {
            string mobileNo = model.MOBILE_NO;

            mobileNo = Regex.Replace(mobileNo, @"^\d$", "", RegexOptions.Compiled);

            //if (mobileNo.Length < 8)
            //    throw new Exception($@"Invalid Number [{ model.MOBILE_NO }] for SMS Log No. { model.ID }.");

            //if (mobileNo.Length > 8)
            //    mobileNo = mobileNo.Substring((mobileNo.Length - 8) - 1, 8);

            //mobileNo = "+974" + mobileNo;

            model.MOBILE_NO = mobileNo;
        }

        internal void FinalizeSENT_MESSAGE(SMS_LOG model)
        {
            model.SENT_MESSAGE = model.SENT_MESSAGE
                .Replace("{REF_ID}", model.REF_ID)
                .Replace("{REF_TYPE}", model.REF_TYPE)
                .Replace("{REG_NO}", model.REG_NO)
                .Replace("{MODEL_NAME}", model.MODEL_NAME)
                .Replace("{TRV_LOCATION}", model.TRV_LOCATION);
                
            if (model.TOT_DAYS != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{TOT_DAYS}", model.TOT_DAYS.ToString());

            if (model.TOT_INV_AMT != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{TOT_INV_AMT}", "QR " + model.TOT_INV_AMT.Value.ToString("#,##.00"));

            if (model.RENT_DATE != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{RENT_DATE}", model.RENT_DATE.Value.ToString("dd/MM/yyyy"));

            if (model.DATE_IN != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{DATE_IN}", model.DATE_IN.Value.ToString("dd/MM/yyyy"));

            if (model.TIME_OUT != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{TIME_OUT}", model.TIME_OUT.Value.ToString("hh:mm tt"));

            if (model.TIME_IN != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{TIME_IN}", model.TIME_IN.Value.ToString("hh:mm tt"));

            if (model.TRV_DATE != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{TRV_DATE}", model.TRV_DATE.Value.ToString("dd/MM/yyyy"));

            if (model.TRV_TIME != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{TRV_TIME}", model.TRV_TIME.Value.ToString("hh:mm tt"));

            if (model.TRV_AMOUNT != null)
                model.SENT_MESSAGE = model.SENT_MESSAGE
                    .Replace("{TRV_AMOUNT}", "QR " + model.TRV_AMOUNT.Value.ToString("#,##.00"));

            model.SENT_MESSAGE = HttpUtility.UrlEncode(model.SENT_MESSAGE);
        }

        internal bool ModelIsValid(SMS_LOG model, OracleConnection cnn)
        {

            return true;
        }
    }
}
