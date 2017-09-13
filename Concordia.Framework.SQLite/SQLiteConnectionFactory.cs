using System.Text;

namespace Concordia.Framework.SQLite
{
    public class SQLiteConnectionFactory : ConnectionFactory<SQLiteConnection>
    {
        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public string DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                ConnectionString = GetConnectionString();
            }
        }

        /// <summary>
        /// A value indicating whether the database is in memory only.
        /// </summary>
        public bool IsMemory
        {
            get => _isMemory;
            set
            {
                _isMemory = value;
                ConnectionString = GetConnectionString();
            }
        }

        private bool _isMemory;
        private string _dataSource;

        /// <summary>
        /// Initializes a new SQLiteConnectionFactory class.
        /// </summary>
        public SQLiteConnectionFactory()
        {
        }

        /// <summary>
        /// Initializes a new SQLiteConnectionFactory class.
        /// <param name="connectionString">The connection string.</param>
        /// </summary>
        public SQLiteConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        private string GetConnectionString()
        {
            var dataSource = IsMemory ? ":memory:" : DataSource;

            var cb = new StringBuilder();
            cb.Append($"Data Source={dataSource}; ");

            return cb.ToString();
        }
    }
}
