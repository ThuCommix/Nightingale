using System.Collections.Generic;
using System.Data;
using Concordia.Framework.Metadata;

namespace Concordia.Framework.Extensions
{
    public static class FieldMetadataExtensions
    {
        private static Dictionary<string, ColumnType> _columnTypeMapping = new Dictionary<string, ColumnType>
        {
            { "string", ColumnType.String },
            { "int", ColumnType.Integer },
            { "decimal", ColumnType.Decimal },
            { "DateTime", ColumnType.DateTime },
            { "bool", ColumnType.Boolean }
        };

        /// <summary>
        /// Gets the sql db type based on the specified field metadata.
        /// </summary>
        /// <param name="fieldMetadata">The field metadata.</param>
        /// <returns>Returns the sqldbtype.</returns>
        public static SqlDbType GetSqlDbType(this FieldMetadata fieldMetadata)
        {
            if (fieldMetadata.FieldType == "string")
            {
                return fieldMetadata.MaxLength > 0 ? SqlDbType.VarChar : SqlDbType.Text;
            }

            if (fieldMetadata.FieldType == "decimal")
                return SqlDbType.Decimal;

            if (fieldMetadata.FieldType == "bool")
                return SqlDbType.Bit;

            if (fieldMetadata.FieldType == "DateTime")
                return SqlDbType.DateTime;

            return SqlDbType.Int;
        }

        /// <summary>
        /// Gets the complex field name.
        /// </summary>
        /// <param name="fieldMetadata">The field metadata.</param>
        /// <returns>Returns the name of the field.</returns>
        public static string GetComplexFieldName(this FieldMetadata fieldMetadata)
        {
            if (!fieldMetadata.IsComplexFieldType)
                return fieldMetadata.Name;

            return fieldMetadata.Name.Substring(3, fieldMetadata.Name.Length - 6);
        }

        /// <summary>
        /// Converts the field metadata to column description.
        /// </summary>
        /// <param name="fieldMetadata">The field metadata.</param>
        /// <returns>Returns the column description.</returns>
        public static ColumnDescription AsColumnDescription(this FieldMetadata fieldMetadata)
        {
            return new ColumnDescription(fieldMetadata.Name)
            {
                ColumnType = fieldMetadata.GetColumnType(),
                IsPrimaryKey = fieldMetadata.Name == "Id",
                DecimalPrecision = fieldMetadata.DecimalPrecision,
                DecimalScale = fieldMetadata.DecimalScale,
                IsNullable = !fieldMetadata.Mandatory,
                IsUnique = fieldMetadata.Unique,
                MaxLength = fieldMetadata.MaxLength
            };
        }

        /// <summary>
        /// Gets the column type for the specified field metadata.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>Returns the column type.</returns>
        public static ColumnType GetColumnType(this FieldMetadata fieldMetadata)
        {
            if (fieldMetadata.Enum || fieldMetadata.IsForeignKey)
                return ColumnType.Integer;

            return _columnTypeMapping[fieldMetadata.FieldType];
        }
    }
}
