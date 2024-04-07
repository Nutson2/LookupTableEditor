namespace LookupTableEditor.Extentions
{
    public static class StringExtentions
    {
        public static int ToInt(this string str)
        {
            int.TryParse(str, out int result);
            return result;
        }

        public static double ToDouble(this string str)
        {
            double.TryParse(str, out double result);
            return result;
        }

        public static bool IsValid(this string str)
        {
            return !string.IsNullOrWhiteSpace(str) && !string.IsNullOrEmpty(str);
        }
    }
}
