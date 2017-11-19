namespace Nightingale.Queries
{
    internal class TableJoin
    {
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the target column.
        /// </summary>
        public string TargetColumn { get; set; }

        /// <summary>
        /// A value indicating whether the join can result in null.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Initializes a new TableJoin class.
        /// </summary>
        /// <param name="table">The table which is being joined.</param>
        /// <param name="alias">The alias of the join.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetColumn">The target column.</param>
        /// <param name="isNullable">Indicates an left or inner join.</param>
        public TableJoin(string table, string alias, string target, string targetColumn, bool isNullable)
        {
            Table = table;
            Alias = alias;
            Target = target;
            TargetColumn = targetColumn;
            IsNullable = isNullable;
        }

        /// <summary>
        /// Gets the sql which represents this join.
        /// </summary>
        /// <returns>Returns a sql string.</returns>
        public override string ToString()
        {
            return (IsNullable ? "LEFT JOIN" : "INNER JOIN") + $" {Table} {Alias} ON {Alias}.Id = {Target}.{TargetColumn}";
        }
    }
}
