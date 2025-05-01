using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LookupTableEditor.Models;

namespace LookupTableEditor.Extentions;

public static class StringExtentions
{
    public static int ToInt(this string str)
    {
        int.TryParse(str, out int result);
        return result;
    }

    public static double ToDouble(this string str)
    {
        string systemDecimalSeparator = CultureInfo
            .CurrentCulture
            .NumberFormat
            .NumberDecimalSeparator;
        double.TryParse(str.Replace(".", systemDecimalSeparator), out double result);
        return result;
    }

    public static bool IsValid(this string? str)
    {
        return !string.IsNullOrWhiteSpace(str) & !string.IsNullOrEmpty(str);
    }

    public static IEnumerable<Cell> ParseAsCells(this string clipboardContent) =>
        clipboardContent
            .Split(new[] { "\r\n" }, StringSplitOptions.None)
            .Where(x => !string.IsNullOrEmpty(x))
            .Select((rowAsText, rowIndex) => (rowAsText, rowIndex))
            .SelectMany(row =>
                row.rowAsText.Split('\t')
                    .Select(
                        (string text, int columnIndex) => new Cell(text, row.rowIndex, columnIndex)
                    )
            );
}
