namespace Concordia.Framework
{
    public class ColumnDescription
    {
        /// <summary>
        /// Gets or sets the column type.
        /// </summary>
        public ColumnType ColumnType { get; set; }

        /// <summary>
        /// Gets the fieldname.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// A value indicating whether the column is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// A value indicating whether the column is the primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// A value indicating whether the column is unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the max length for string based properties.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the decimal precision.
        /// </summary>
        public int DecimalPrecision { get; set; }

        /// <summary>
        /// Gets or sets the decimal scale.
        /// </summary>
        public int DecimalScale { get; set; }

        /// <summary>
        /// Initializes a new ColumnDescription class.
        /// </summary>
        /// <param name="fieldName">The fieldName.</param>
        public ColumnDescription(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}
