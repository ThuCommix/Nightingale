using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nightingale.Entities;
using Nightingale.Metadata;
using Nightingale.Queries;

namespace Nightingale.SQLite
{
    public class SQLiteTable<T> : Table<T> where T : Entity
    {
        private static Dictionary<string, string> DataTypeMapping = new Dictionary<string, string>
        {
            { "int", "int" },
            { "decimal", "decimal" },
            { "string", "text" },
            { "bool", "tinyint(1)" },
            { "DateTime", "datetime" }
        };

        /// <summary>
        /// Initializes a new SQLiteTable class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SQLiteTable(IConnection connection) : base(connection)
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
            fieldDefinitions.Add("Deleted tinyint(1) NOT NULL");
            fieldDefinitions.Add("Version int NOT NULL");

            foreach(var field in Metadata.Fields.Where(x => x.Name != "Id" && x.Name != "Deleted" && x.Name != "Version"))
            {
                if(field.FieldType == "string" && field.MaxLength > 0)
                {
                    fieldDefinitions.Add($"{field.Name} varchar{GetPrecisionCommand(field)} {GetMandatoryCommand(field.Mandatory)} {GetUniqueCommand(field.Unique)}");
                    continue;
                }

                fieldDefinitions.Add($"{field.Name} {DataTypeMapping[field.IsComplexFieldType ? "int" : field.FieldType]}{GetPrecisionCommand(field)} {GetMandatoryCommand(field.Mandatory)} {GetUniqueCommand(field.Unique)}");
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
            var query = new Query($"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND tbl_name = '{Metadata.Table}'", Type);
            return Convert.ToInt32(Connection.ExecuteScalar(query)) > 0;
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
        /// Gets the precision sql command.
        /// </summary>
        /// <param name="field">The field metadata.</param>
        /// <returns>Returns the command.</returns>
        private string GetPrecisionCommand(FieldMetadata field)
        {
            if (field.FieldType == "decimal")
            {
                return $"({field.DecimalPrecision}, {field.DecimalScale})";
            }

            if(field.FieldType == "string" && field.MaxLength > 0)
            {
                return $"({field.MaxLength})";
            }

            return string.Empty;
        }
    }
}
