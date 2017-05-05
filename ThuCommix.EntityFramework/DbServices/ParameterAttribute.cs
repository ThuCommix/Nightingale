using System;
using System.Data;

namespace ThuCommix.EntityFramework.DbServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the sql db type.
        /// </summary>
        public SqlDbType DbType { get; }
        
        /// <summary>
        /// Gets the size.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// A value indicating whether the parameter is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Initializes a new ParameterAttribute class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dbType">The sql db type.</param>
        /// <param name="isNullable">Indicates that the parameter is nullable.</param>
        /// <param name="size">The size.</param>
        public ParameterAttribute(string name, SqlDbType dbType, bool isNullable = false, int size = 0)
        {
            Name = name;
            DbType = dbType;
            Size = size;
            IsNullable = IsNullable;
        }
    }
}
