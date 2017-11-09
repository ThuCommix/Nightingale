using System;
using System.Collections.Generic;

namespace Nightingale
{
    public static class DependencyResolver
    {
        private static readonly Dictionary<Type, object> Dependencies;
        
        static DependencyResolver()
        {
            Dependencies = new Dictionary<Type, object>();

            Registry.Initialize();
        }

        public static void Register<T>(T instance)
        {
            Dependencies[typeof(T)] = instance;
        }

        public static void Register<T>()
        {
            Register(Activator.CreateInstance<T>());
        }

        public static T GetInstance<T>()
        {
            if (!Dependencies.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"The type '{typeof(T).Name}' was not registered.");

            return (T)Dependencies[typeof(T)];
        }

        public static T TryGetInstance<T>() where T : class
        {
            if (!Dependencies.ContainsKey(typeof(T)))
                return default(T);

            return (T)Dependencies[typeof(T)];
        }

        public static void Clear()
        {
            Dependencies.Clear();
        }
    }
}
