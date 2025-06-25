namespace LookupTableEditor.Models;

public struct Cell(string text, int rowIndex, int columnIndex)
{
	public string Text { get; } = text;
	public int RowIndex { get; } = rowIndex;
	public int ColumnIndex { get; } = columnIndex;
	public Cell WithText(string text) => new Cell(text, RowIndex, ColumnIndex);

	public Cell WithRowIndex(int rowIndex) => new Cell(Text, rowIndex, ColumnIndex);

	public Cell WithColumnIndex(int columnIndex) => new Cell(Text, RowIndex, columnIndex);
}
