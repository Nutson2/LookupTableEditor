namespace LookupTableEditor.Extentions
{
    public static class ObjectExtention
    {
        public static T ThrowIfNull<T>(this T obj)
            where T : class => obj ?? throw new CustomNullException($"{typeof(T)} is null");
    }
}
