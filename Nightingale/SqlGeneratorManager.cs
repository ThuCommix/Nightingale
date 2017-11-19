using Nightingale.Queries;

namespace Nightingale
{
    public static class SqlGeneratorManager
    {
        /// <summary>
        /// Gets the current sql engine.
        /// </summary>
        public static ISqlGenerator SqlEngine { get; set; }

        /// <summary>
        /// Initializes a new SqlEngineManager class.
        /// </summary>
        static SqlGeneratorManager()
        {
            SqlEngine = new AnsiSqlEngine();
        }
    }
}
