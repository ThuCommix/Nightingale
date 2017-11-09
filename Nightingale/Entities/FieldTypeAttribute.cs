using System;

namespace Nightingale.Entities
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
