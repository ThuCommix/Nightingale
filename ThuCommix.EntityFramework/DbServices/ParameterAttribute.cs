using System;

namespace ThuCommix.EntityFramework.DbServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new ParameterAttribute class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
