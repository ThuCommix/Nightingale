using System;
using System.Collections.Generic;

namespace ThuCommix.EntityFramework
{
    public static class DependencyResolver
    {
        private static Dictionary<Type, object> _dependencies;
        
        static DependencyResolver()
        {
            _dependencies = new Dictionary<Type, object>();
        }

        public static void Register<T>(T instance)
        {
            _dependencies[typeof(T)] = instance;
        }

        public static void Register<T>()
        {
            Register(Activator.CreateInstance<T>());
        }

        public static T GetInstance<T>()
        {
            if (!_dependencies.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"The type '{typeof(T).Name}' was not registered.");

            return (T)_dependencies[typeof(T)];
        }

        public static void Clear()
        {
            _dependencies.Clear();
        }
    }
}
