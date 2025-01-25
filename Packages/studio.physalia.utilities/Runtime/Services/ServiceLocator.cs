using System;
using System.Collections.Generic;

namespace Physalia
{
    public static class ServiceLocator
    {
        private static readonly Logger.Label Label = Logger.Label.CreateFromCurrentClass();

        private static readonly Dictionary<Type, object> _services = new(16);

        public static bool Has<T>() where T : class
        {
            Type type = typeof(T);
            return _services.ContainsKey(type);
        }

        public static void Register<T>(T service) where T : class
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Logger.Error(Label, $"Register failed! Service of '{type}' has already existed.");
                return;
            }

            _services.Add(type, service);
        }

        public static void Unregister<T>() where T : class
        {
            Type type = typeof(T);
            _services.Remove(type);
        }

        public static T Resolve<T>() where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object value))
            {
                return value as T;
            }

            Logger.Error(Label, $"Resolve failed! Service of '{type}' doesn't exist.");
            return null;
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}
