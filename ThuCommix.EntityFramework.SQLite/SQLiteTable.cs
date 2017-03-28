using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Metadata;
using ThuCommix.EntityFramework.Queries;

namespace ThuCommix.EntityFramework.SQLite
{
    public class SQLiteTable<T> : Table<T> where T : Entity
    {
        private static Dictionary<string, string> DataTypeMapping = new Dictionary<string, string>
        {
            {"int", "int" },
            {"decimal", "decimal" },
            {"string", "text" },
            {"bool", "tinyint(1)" },
            {"DateTime", "datetime" }
        };

        /// <summary>
        /// Initializes a new SQLiteTable class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public SQLiteTable(IDataProvider dataProvider) : base(dataProvider)
        {
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        public override void Create()
        {
            var commandBuilder = new StringBuilder();
            commandBuilder.AppendLine($"CREATE TABLE IF NOT EXISTS {Metadata.Table} (");
            var fieldDefinitions = new List<string>();
            fieldDefinitions.Add("Id INTEGER PRIMARY KEY AUTOINCREMENT");
            fieldDefinitions.Add("Deleted TINYINT(1) NOT NULL");
            fieldDefinitions.Add("Version int NOT NULL");

            foreach(var field in Metadata.Fields.Where(x => x.Name != "Id" && x.Name != "Deleted" && x.Name != "Version"))
            {
                fieldDefinitions.Add($"{field.Name} {DataTypeMapping[field.IsComplexFieldType ? "int" : field.FieldType]}{GetDecimalPrecisionCommand(field)} {GetMandatoryCommand(field.Mandatory)} {GetUniqueCommand(field.Unique)}");
            }

            commandBuilder.AppendLine(string.Join(",", fieldDefinitions));

            commandBuilder.AppendLine(");");

            var query = new Query(commandBuilder.ToString(), Type);
            DataProvider.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public override void Delete()
        {
            var query = new Query($"DROP TABLE IF EXISTS {Metadata.Table}", Type);
            DataProvider.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Removes the column.
        /// </summary>
        /// <param name="column">The column.</param>
        public override void RemoveColumn(Column column)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new column to the table.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="defaultValue">The default value.</param>
        public override void AddColumn(Column column, object defaultValue = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an enumeration of the available columns.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Column> GetColumns()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the mandatory sql command.
        /// </summary>
        /// <param name="mandatory">The mandatory flag.</param>
        /// <returns>Returns the command.</returns>
        private string GetMandatoryCommand(bool mandatory)
        {
            return mandatory ? "NOT NULL" : string.Empty;
        }

        /// <summary>
        /// Gets the unqiue sql command.
        /// </summary>
        /// <param name="unique">The unique flag.</param>
        /// <returns>Returns the command.</returns>
        private string GetUniqueCommand(bool unique)
        {
            return unique ? "UNIQUE" : string.Empty;
        }

        /// <summary>
        /// Gets the decimal precision/scale sql command.
        /// </summary>
        /// <param name="field">The field metadata.</param>
        /// <returns>Returns the command.</returns>
        private string GetDecimalPrecisionCommand(FieldMetadata field)
        {
            if(field.FieldType == "decimal")
            {
                return $"({field.DecimalPrecision}, {field.DecimalScale})";
            }

            return string.Empty;
        }
    }
}
