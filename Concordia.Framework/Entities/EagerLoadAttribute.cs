using System;

namespace Concordia.Framework.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : Attribute
    {
    }
}
