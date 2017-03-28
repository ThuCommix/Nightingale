namespace ThuCommix.EntityFramework.Queries
{
    public class QueryParameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new QueryParameter class.
        /// </summary>
        public QueryParameter()
        {            
        }

        /// <summary>
        /// Initializes a new QueryParameter class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public QueryParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
