using System;

namespace Concordia.Framework.DbServices
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProcedureAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the procedure.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new ProcedureAttribute class.
        /// </summary>
        /// <param name="name">The name of the procedure.</param>
        public ProcedureAttribute(string name)
        {
            Name = name;
        }
    }
}
