using System;
using System.Collections.Generic;

namespace LookupTableEditor.Extentions;

public static class IEnumerableExtention
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (T item in enumerable)
        {
            action.Invoke(item);
        }

        return enumerable;
    }
}