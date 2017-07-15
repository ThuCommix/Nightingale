using System.Text;

namespace Concordia.Framework.PostgreSql
{
    public class NpgsqlConnectionFactory : ConnectionFactory<NpgsqlConnection>
    {
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        public string Host
        {
            get { return _host; }
            set
            {
                _host = value;
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

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                ConnectionString = GetConnectionString();
            }
        }

        private string _host;
        private string _user;
        private string _password;
        private string _database;
        private bool _pooling;
        private int _maxPoolSize;
        private int _port;

        /// <summary>
        /// Initializes a new NpgsqlConnectionFactory class.
        /// </summary>
        public NpgsqlConnectionFactory()
        {
            Pooling = true;
            MaxPoolSize = 100;
        }

        /// <summary>
        /// Initializes a new NpgsqlConnectionFactory class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public NpgsqlConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        private string GetConnectionString()
        {
            var pooling = Pooling ? "true" : "false";

            var cb = new StringBuilder();
            cb.Append($"Host={Host}; ");
            cb.Append($"Database={Database}; ");
            cb.Append($"User ID={User}; ");
            cb.Append($"Port={Port}; ");
            cb.Append($"Password={Password}; ");
            cb.Append($"Pooling={pooling}; ");
            cb.Append($"Max Pool Size={MaxPoolSize}; ");

            return cb.ToString();
        }
    }
}
