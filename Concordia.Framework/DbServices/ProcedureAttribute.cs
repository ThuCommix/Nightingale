using System;

namespace Concordia.Framework.DbServices
{
    [Obsolete("The DbServices namespace with it's content will be removed in v2.0.0 to accomplish .net standard 2.0 compatibility.")]
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
