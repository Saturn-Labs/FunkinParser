using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Funkin.Core.Data.v20X
{
    public class SongData : VersioningData, IData
    {
        private static readonly PropertyInfo[] PropertiesCache = typeof(SongData).GetProperties();
        public object? this[string key] => GetValue(key);

        public object? GetValue(string key)
        {
            var prop = PropertiesCache.FirstOrDefault(p =>
            {
                if (p.Name == key)
                    return true;
                if (!(p.GetCustomAttribute<JsonPropertyAttribute>() is {} attr))
                    return false;
                return attr.PropertyName == key;
            });
            return prop?.GetValue(this);
        }

        public T GetValue<T>(string key)
        {
            if (!(GetValue(key) is T value))
                throw new InvalidCastException($"Failed to cast key '{key}' value to type '{typeof(T).Name}'.");
            return value;
        }
        
        public bool HasValue(string key)
        {
            return PropertiesCache.FirstOrDefault(p =>
                {
                    if (p.Name == key)
                        return true;
                    if (!(p.GetCustomAttribute<JsonPropertyAttribute>() is { } attr))
                        return false;
                    return attr.PropertyName == key;
                }
            ) is {};
        }
    }
}