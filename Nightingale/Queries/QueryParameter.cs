using System.Data;

namespace Nightingale.Queries
{
    public class QueryParameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the db type.
        /// </summary>
        public SqlDbType DbType { get; set; }

        /// <summary>
        /// A value indicating whether the parameter is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Sets the size of size dependent dbtypes e.g. varchar.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the decimal precision.
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// Gets or sets the decimal scale.
        /// </summary>
        public int? Scale { get; set; }

        /// <summary>
        /// Initializes a new QueryParameter class.
        /// </summary>
        public QueryParameter()
        {            
        }

        /// <summary>
        /// Initializes a new QueryParameter class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">The dbtype.</param>
        /// <param name="isNullable">A value indicating the parameter is nullable.</param>
        /// <param name="size">The size.</param>
        public QueryParameter(string name, object value, SqlDbType dbType, bool isNullable, int size = 0)
        {
            Name = name;
            Value = value;
            DbType = dbType;
            IsNullable = isNullable;
            Size = size;
        }
    }
}
