using System.Text;

namespace Concordia.Framework.MySql
{
    public class MySqlConnectionFactory : ConnectionFactory<MySqlConnection>
    {
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public string Server
        {
            get { return _server; }
            set
            {
                _server = value;
                ConnectionString = GetConnectionString();
            }
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public string Database
        {
            get { return _database; }
            set
            {
                _database = value;
                ConnectionString = GetConnectionString();
            }
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
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
        /// Gets or sets the max pool size.
        /// </summary>
        public int MaxPoolSize
        {
            get { return _maxPoolSize; }
            set
            {
                _maxPoolSize = value;
                _pooling = _maxPoolSize > 1;
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
                if (_maxPoolSize < 2)
                    _maxPoolSize = 100;

                ConnectionString = GetConnectionString();
            }
        }

        private string _server;
        private string _user;
        private string _password;
        private string _database;
        private bool _pooling;
        private int _maxPoolSize;

        /// <summary>
        /// Initializes a new MySqlConnectionFactory class.
        /// </summary>
        public MySqlConnectionFactory()
        {
            Pooling = true;
            MaxPoolSize = 100;
        }

        /// <summary>
        /// Initializes a new MySqlConnectionFactory class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MySqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        private string GetConnectionString()
        {
            var cb = new StringBuilder();
            cb.Append($"Server={Server}; ");
            cb.Append($"Database={Database}; ");
            cb.Append($"Uid={User}; ");
            cb.Append($"Pwd={Password}; ");
            cb.Append($"maximumpoolsize={MaxPoolSize}; ");

            return cb.ToString();
        }
    }
}
