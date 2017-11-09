using System;

namespace Nightingale.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EagerLoadAttribute : Attribute
    {
    }
}
