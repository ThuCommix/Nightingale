using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Concordia.Framework.Entities;
using Concordia.Framework.Metadata;
using Concordia.Framework.Queries;

namespace Concordia.Framework.MySql
{
    public class MySqlTable<T> : Table<T> where T : Entity
    {
        private static readonly Dictionary<string, string> DataTypeMapping = new Dictionary<string, string>
        {
            {"int", "int" },
            {"decimal", "decimal" },
            {"bool", "tinyint(1)" },
            {"DateTime", "datetime" }
        };

        /// <summary>
        /// Initializes a new MySqlTable class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public MySqlTable(IConnection connection) : base(connection)
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
            fieldDefinitions.Add("Id INTEGER PRIMARY KEY AUTO_INCREMENT");
            fieldDefinitions.Add("Deleted TINYINT(1) NOT NULL");
            fieldDefinitions.Add("Version int NOT NULL");

            foreach (var field in Metadata.Fields.Where(x => x.Name != "Id" && x.Name != "Deleted" && x.Name != "Version"))
            {
                fieldDefinitions.Add($"{field.Name} {GetDataTypeMapping(field)}{GetDecimalPrecisionCommand(field)} {GetMandatoryCommand(field.Mandatory)} {GetUniqueCommand(field.Unique)}");
            }

            commandBuilder.AppendLine(string.Join(",", fieldDefinitions));

            commandBuilder.AppendLine(");");

            var query = new Query(commandBuilder.ToString(), Type);
            Connection.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public override void Delete()
        {
            var query = new Query($"DROP TABLE IF EXISTS {Metadata.Table}", Type);
            Connection.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Gets a value indicating whether the table exists.
        /// </summary>
        public override bool Exists()
        {
            var query = new Query($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = (SELECT DATABASE()) AND table_name = '{Metadata.Table}'", Type);
            return Convert.ToInt32(Connection.ExecuteScalar(query)) == 1;
        }

        /// <summary>
        /// Gets the data type mapping.
        /// </summary>
        /// <param name="field">The field metadata.</param>
        /// <returns>Returns the datatype mapped string.</returns>
        private string GetDataTypeMapping(FieldMetadata field)
        {
            return field.FieldType == "string" ? GetStringFieldCommand(field.MaxLength) : DataTypeMapping[field.IsComplexFieldType ? "int" : field.FieldType];
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
            if (field.FieldType == "decimal")
            {
                return $"({field.DecimalPrecision}, {field.DecimalScale})";
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the datatype for string fields.
        /// </summary>
        /// <param name="maxLength">The maxlength.</param>
        /// <returns>Returns the command.</returns>
        private string GetStringFieldCommand(int maxLength)
        {
            return maxLength == 0 ? "text" : $"varchar({maxLength})";
        }
    }
}
