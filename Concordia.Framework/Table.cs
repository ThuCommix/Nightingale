using System;
using System.Collections.Generic;
using System.Linq;
using Concordia.Framework.Entities;
using Concordia.Framework.Metadata;
using Concordia.Framework.Queries;

namespace Concordia.Framework
{
    public abstract class Table<T> where T : Entity
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type => typeof(T);

        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        protected EntityMetadata Metadata => DependencyResolver.GetInstance<IEntityMetadataResolver>().GetEntityMetadata(Type);

        /// <summary>
        /// Gets the connection.
        /// </summary>
        protected IConnection Connection { get; }

        private static Dictionary<string, ColumnType> _columnTypeMapping = new Dictionary<string, ColumnType>
        {
            { "string", ColumnType.String },
            { "int", ColumnType.Integer },
            { "decimal", ColumnType.Decimal },
            { "DateTime", ColumnType.DateTime },
            { "bool", ColumnType.Boolean }
        };

        /// <summary>
        /// Initializes a new Table class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected Table(IConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            Connection = connection;
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Removes the column.
        /// </summary>
        /// <param name="column">The column.</param>
        public abstract void RemoveColumn(Column column);

        /// <summary>
        /// Adds a new column to the table.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="defaultValue">The default value.</param>
        public abstract void AddColumn(Column column, object defaultValue = null);

        /// <summary>
        /// Gets an enumeration of the available columns.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Column> GetColumns();

        /// <summary>
        /// Executes the query without results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the count of affected rows.</returns>
        public virtual int ExecuteNonQuery(IQuery query)
        {
            return Connection.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Gets the column type for the specified field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>Returns the column type.</returns>
        protected virtual ColumnType GetColumnType(string fieldName)
        {
            var fieldMetadata = Metadata.Fields.FirstOrDefault(x => x.Name == fieldName);
            if (fieldMetadata == null)
                throw new InvalidOperationException($"The field was not found on table '{Metadata.Table}'.");

            return fieldMetadata.IsForeignKey ? ColumnType.ForeignKey : _columnTypeMapping[fieldMetadata.FieldType];
        }
    }
}
