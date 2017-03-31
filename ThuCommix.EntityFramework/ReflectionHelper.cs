using System;
using System.ComponentModel;
using System.Reflection;

namespace ThuCommix.EntityFramework
{
    internal static class ReflectionHelper
    {
        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static string GetDisplayName(Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }
    }
}
