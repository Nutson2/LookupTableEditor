namespace LookupTableEditor.Models;

public class Cell(string text, int rowIndex, int columnIndex)
{
    public string Text { get; } = text;
    public int RowIndex { get; } = rowIndex;
    public int ColumnIndex { get; } = columnIndex;
}
