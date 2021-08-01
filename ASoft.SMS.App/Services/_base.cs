using ASoft.SMS.App.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace ASoft.SMS.App.Services
{
    public class BaseService
    {
        protected OracleConnection OpenConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            OracleConnection cnn = new OracleConnection(connectionString);
            cnn.Open();

            return cnn;
        }

        protected BaseDTOCollection<T> Get<T>(string query, OracleConnection cnn, List<OracleParameter> parameters = null, string orderbyField = "", int? pageSize = null, int? skip = null) where T : BaseDTO
        {
            BaseDTOCollection<T> data = new BaseDTOCollection<T>();

            BaseDTOCollectionRaw raw = GetRaw(query, cnn, parameters, orderbyField, pageSize, skip);
            data.TotalCount = raw.TotalCount;
            if (!string.IsNullOrEmpty(raw.Data))
                data.Items = (List<T>)JsonConvert.DeserializeObject(raw.Data, typeof(List<T>));

            return data;
        }

        protected BaseDTOCollectionDynamic GetDynamic(string query, OracleConnection cnn, List<OracleParameter> parameters = null, string orderbyField = "", int? pageSize = null, int? skip = null)
        {
            BaseDTOCollectionDynamic data = new BaseDTOCollectionDynamic();

            BaseDTOCollectionRaw raw = GetRaw(query, cnn, parameters, orderbyField, pageSize, skip);
            data.TotalCount = raw.TotalCount;
            if (!string.IsNullOrEmpty(raw.Data))
                data.Items = JsonConvert.DeserializeObject(raw.Data);
            else
                data.Items = new Newtonsoft.Json.Linq.JArray();

            return data;
        }

        protected BaseDTOCollectionRaw GetRaw(string query, OracleConnection cnn, List<OracleParameter> parameters = null, string orderbyField = "", int? pageSize = null, int? skip = null)
        {
            BaseDTOCollectionRaw data = new BaseDTOCollectionRaw();

            OracleCommand cmd = cnn.CreateCommand();
            cmd.CommandText = query;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            cmd.BindByName = true;
            cmd.CommandText = $@"SELECT COUNT(*) FROM ({ query })";
            data.TotalCount = Convert.ToInt32(cmd.ExecuteScalar());

            if (!string.IsNullOrEmpty(orderbyField))
            {
                query += $@" ORDER BY { orderbyField }";

                if (skip != null || pageSize != null)
                    query = GenerateQueryWithPagination(query, orderbyField, pageSize ?? 0, skip ?? 0);
            }

            cmd.CommandText = query;

            using (var reader = cmd.ExecuteReader())
            {
                using (DataTable dt = new DataTable())
                {
                    dt.Load(reader);

                    if (dt.Rows.Count > 0)
                        data.Data = JsonConvert.SerializeObject(dt);
                }
            };

            return data;
        }

        protected object Post(string query, OracleConnection cnn, List<OracleParameter> parameters = null, OracleTransaction trn = null)
        {
            object value = null;

            using (OracleCommand cmd = new OracleCommand())
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters.ToArray());

                if (trn != null)
                    cmd.Transaction = trn;

                cmd.BindByName = true;
                cmd.CommandText = query;
                cmd.Connection = cnn;

                value = cmd.ExecuteScalar();
            }

            return value;
        }

        protected string GenerateQueryWithPagination(string query, string orderBy, int pageSize, int skip)
        {
            return $@"
            SELECT * FROM (
                SELECT
                    T.*,
                    ROW_NUMBER() OVER(ORDER BY { orderBy }) AS ""_Index""
                    FROM ({ query }) T
            ) WHERE ""_Index"" > { skip }
                AND ""_Index"" <= { skip } + { pageSize }";
        }

        protected void ValidateRequiredField(object field, string fieldName, bool allowZero = false)
        {
            if (field == null)
                throw new RequiredFieldException(fieldName);
            else if (field is string)
            {
                if (string.IsNullOrEmpty((string)field))
                    throw new RequiredFieldException(fieldName);
            }
            else if (field is int? && !allowZero)
            {
                if (!((((int?)field) ?? 0) > 0))
                    throw new RequiredFieldException(fieldName);
            }
            else if (field is long? && !allowZero)
            {
                if (!((((long?)field) ?? 0) > 0))
                    throw new RequiredFieldException(fieldName);
            }
            else if (field is decimal? && !allowZero)
            {
                if (!((((decimal?)field) ?? 0) > 0))
                    throw new RequiredFieldException(fieldName);
            }
            else if (field is double? && !allowZero)
            {
                if (!((((double?)field) ?? 0) > 0))
                    throw new RequiredFieldException(fieldName);
            }
        }

        protected DateTime RemoveTime(DateTime dateTime)
        {
            return dateTime.Date;
        }

        protected DateTime CombineDateAndTime(DateTime date, DateTime time)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hour,
                time.Minute,
                time.Second
            );
        }

        protected static void Log(string text)
        {
            string location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string path = Path.Combine(location, "log.txt");

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(text);
            }
        }

        protected string CreateErrorResponseMessage(Exception ex)
        {
            string content;

            if (ex is NotFoundException)
            {
                string entityType = ((NotFoundException)ex).EntityType;
                string key = ((NotFoundException)ex).Key;

                content = $@"{ entityType } [{ key }] not found.";
            }
            else if (ex is RequiredFieldException)
            {
                string fieldName = ((RequiredFieldException)ex).FieldName;

                content = $@"{ fieldName } is required.";
            }
            else if (ex is InvalidFieldValueException)
            {
                string fieldName = ((InvalidFieldValueException)ex).FieldName;

                content = $@"Invalid { fieldName }.";
            }
            else if (ex is DuplicateFieldValueException)
            {
                string fieldName = ((DuplicateFieldValueException)ex).FieldName;

                content = $@"{ fieldName } already exists.";
            }
            else
            {
                content = GetExceptionMessage(ex);
            }

            string dt = DateTime.Now.ToString("yyMMddHHmmss");
            string log = $@"{ dt } >> { content }";

            Log(log);

            return content;
        }

        private string GetExceptionMessage(Exception ex)
        {
            string message = ex.Message;

            if (ex.InnerException != null)
                message += " " + GetExceptionMessage(ex.InnerException);

            return message;
        }

        public static Settings GetSettings()
        {
            Settings value = null;

            Settings settings = new Settings();

            settings.SMS.Url = ConfigurationManager.AppSettings["SMS:Url"];
            settings.SMS.CustomerID = Convert.ToInt32(ConfigurationManager.AppSettings["SMS:CustomerID"]);
            settings.SMS.Name = ConfigurationManager.AppSettings["SMS:Name"];
            settings.SMS.Password = ConfigurationManager.AppSettings["SMS:Password"];
            settings.SMS.Originator = ConfigurationManager.AppSettings["SMS:Originator"];

            settings.SMS.Message.Template1 = ConfigurationManager.AppSettings["SMS:Message:Template1"];
            settings.SMS.Message.Template2 = ConfigurationManager.AppSettings["SMS:Message:Template2"];
            settings.SMS.Message.Template3 = ConfigurationManager.AppSettings["SMS:Message:Template3"];
            settings.SMS.Message.Template4 = ConfigurationManager.AppSettings["SMS:Message:Template4"];
            settings.SMS.Message.Template5 = ConfigurationManager.AppSettings["SMS:Message:Template5"];

            value = settings;

            return value;
        }
    }
}
