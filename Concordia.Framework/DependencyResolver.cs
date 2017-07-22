using System;
using System.Collections.Generic;

namespace Concordia.Framework
{
    public static class DependencyResolver
    {
        private static Dictionary<Type, object> _dependencies;
        
        static DependencyResolver()
        {
            _dependencies = new Dictionary<Type, object>();

            Registry.Initialize();
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

        public static T TryGetInstance<T>() where T : class
        {
            if (!_dependencies.ContainsKey(typeof(T)))
                return default(T);

            return (T)_dependencies[typeof(T)];
        }

        public static void Clear()
        {
            _dependencies.Clear();
        }
    }
}
