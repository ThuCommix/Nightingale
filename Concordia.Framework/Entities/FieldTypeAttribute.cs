using System;

namespace Concordia.Framework.Entities
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
