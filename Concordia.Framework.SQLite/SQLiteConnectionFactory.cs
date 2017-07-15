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
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                ConnectionString = GetConnectionString();
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// A value indicating whether the database is in memory only.
        /// </summary>
        public bool IsMemory
        {
            get { return _isMemory; }
            set
            {
                _isMemory = value;
                ConnectionString = GetConnectionString();
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                ConnectionString = GetConnectionString();
            }
        }

        /// <summary>
        /// A value indicating whether the connection uses pooling.
        /// </summary>
        public bool Pooling
        {
            get { return _pooling; }
            set
            {
                _pooling = value;
                ConnectionString = GetConnectionString();
            }
        }

        /// <summary>
        /// Gets or sets the max pool size.
        /// </summary>
        public int MaxPoolSize
        {
            get { return _maxPoolSize; }
            set
            {
                _maxPoolSize = value;
                ConnectionString = GetConnectionString();
            }
        }

        private int _maxPoolSize;
        private bool _pooling;
        private string _password;
        private bool _isMemory;
        private string _dataSource;

        /// <summary>
        /// Initializes a new SQLiteConnectionFactory class.
        /// </summary>
        public SQLiteConnectionFactory()
        {
            Version = 3;
            Pooling = true;
            MaxPoolSize = 100;
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
            var pooling = Pooling ? "True" : "False";

            var cb = new StringBuilder();
            cb.Append($"Data Source={dataSource}; ");
            cb.Append($"Version={Version}; ");

            if(!string.IsNullOrWhiteSpace(Password))
            {
                cb.Append($"Password={Password}; ");
            }

            if(Pooling)
            {
                cb.Append($"Pooling={pooling}; ");
                cb.Append($"Max Pool Size={MaxPoolSize};");
            }

            return cb.ToString();
        }
    }
}
