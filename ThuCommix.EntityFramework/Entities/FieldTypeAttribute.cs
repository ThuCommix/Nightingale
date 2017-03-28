using System;

namespace ThuCommix.EntityFramework.Entities
{
    public class FieldTypeAttribute : Attribute
    {
        public string FieldType { get; }

        public FieldTypeAttribute(string fieldType)
        {
            FieldType = fieldType;
        }
    }
}
