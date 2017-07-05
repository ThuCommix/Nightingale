namespace Concordia.Framework
{
    public class Column
    {
        /// <summary>
        /// Gets the column type.
        /// </summary>
        public ColumnType ColumnType { get; }

        /// <summary>
        /// Gets the fieldname.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// A value indicating whether the column is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// A value indicating whether the column is the primary key.
        /// </summary>
        public bool IsPrimaryKey { get; }

        /// <summary>
        /// A value indicating whether the column is unique.
        /// </summary>
        public bool IsUnique { get; }

        /// <summary>
        /// Initializes a new Column class.
        /// </summary>
        /// <param name="fieldName">The fieldName.</param>
        /// <param name="columnType">The column type.</param>
        /// <param name="nullable">Indicates if the column is nullable.</param>
        /// <param name="primaryKey">Indicates if the column is the primary key.</param>
        /// <param name="unique">Indicates if the column is unique.</param>
        public Column(string fieldName, ColumnType columnType, bool nullable, bool primaryKey, bool unique)
        {
            FieldName = fieldName;
            ColumnType = ColumnType;
            IsNullable = nullable;
            IsPrimaryKey = primaryKey;
            IsUnique = unique;
        }
    }
}
