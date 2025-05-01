using System.Collections.Generic;

namespace LookupTableEditor.Extentions;

public static class DictionaryExtensions
{
    public static TValue? GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        if (dict.TryGetValue(key, out var res))
        {
            return res;
        }
        return default;
    }

    public static bool TryAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dict,
        TKey key,
        TValue value
    )
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, value);
            return true;
        }
        return false;
    }
}
