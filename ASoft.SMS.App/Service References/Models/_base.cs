using System;
using System.Collections.Generic;

namespace ASoft.SMS.App.Models
{
    public class BaseGetParameter
    {
        public int? PageSize { get; set; }
        public int? Skip { get; set; }
        public string LookupText { get; set; }
    }

    public class BasePostPopulateParameter<T>
    {
        public T model { get; set; }
    }

    public class BaseDTO
    {
        public string _Type
        {
            get { return this.GetType().Name; }
            set { }
        }
    }

    public class BaseDTOCollection
    {
        public int TotalCount { get; set; }
    }

    public class BaseDTOCollectionRaw : BaseDTOCollection
    {
        public string Data { get; set; }
    }

    public class BaseDTOCollectionDynamic : BaseDTOCollection
    {
        public dynamic Items { get; set; }
    }


    public class BaseDTOCollection<T> : BaseDTOCollection
    {
        public BaseDTOCollection()
        {
            this.Items = new List<T>();
        }

        public List<T> Items { get; set; }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string entityType, string key)
        {
            this.EntityType = entityType;
            this.Key = key;
        }

        public string EntityType { get; set; }
        public string Key { get; set; }
    }

    public class RequiredFieldException : Exception
    {
        public RequiredFieldException(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }

    public class InvalidFieldValueException : Exception
    {
        public InvalidFieldValueException(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }

    public class DuplicateFieldValueException : Exception
    {
        public DuplicateFieldValueException(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }

    public class Settings
    {
        public Settings()
        {
            this.SMS = new SettingsSMS();
        }

        public SettingsSMS SMS { get; set; }
    }

    public class SettingsSMS
    {
        public SettingsSMS()
        {
            this.Message = new SettingsSMSMessage();
        }

        public string Url { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Originator { get; set; }

        public SettingsSMSMessage Message { get; set; }
    }

    public class SettingsSMSMessage
    {
        public string Template1 { get; set; }
        public string Template2 { get; set; }
        public string Template3 { get; set; }
        public string Template4 { get; set; }
        public string Template5 { get; set; }
    }
}
