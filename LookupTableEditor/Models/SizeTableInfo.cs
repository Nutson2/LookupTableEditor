using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;
using LookupTableEditor.Services;

namespace LookupTableEditor.Models;

public class SizeTableInfo
{
	private readonly List<AbstractParameterType> _abstractParameterTypes;
	private readonly string _headerDelimiter = ",";

	private readonly Dictionary<string, AbstractParameterType> _headerTypes = [];
	private readonly AbstractParameterTypesProvider _parameterTypesProvider;

	private readonly string _systemDecimalSeparator = CultureInfo
		.CurrentCulture
		.NumberFormat
		.NumberDecimalSeparator;

	public string? Name { get; set; }
	public string? FilePath { get; set; }

	public DataTable Table { get; } = new();

	public SizeTableInfo(
		string? name,
		List<AbstractParameterType> abstractParameterTypes,
		AbstractParameterTypesProvider parameterTypesProvider
	)
	{
		Name = name;
		_abstractParameterTypes = abstractParameterTypes;
		_parameterTypesProvider = parameterTypesProvider;
	}

	public void InsertFirstColumn()
	{
		Table.Columns.Add("_", typeof(string));
		_headerTypes.Add("_", _parameterTypesProvider.Empty());
	}

	public void AddHeader(FamilySizeTableColumn column)
	{
		Type dataTableHeaderType = column.GetTypeForDataTable();
		AbstractParameterType headerType = _parameterTypesProvider.FromSizeTableColumn(column);
		string? name = column.Name;

		AddHeaderInternal(name, dataTableHeaderType, headerType);
	}

	public void AddHeader(FamilyParameter parameter)
	{
		string? name = parameter.Definition.Name;
		Type dataTableHeaderType = parameter.Definition.GetTypeForDataTable();
		AbstractParameterType headerType = _parameterTypesProvider.FromFamilyParameter(parameter);
		AddHeaderInternal(name, dataTableHeaderType, headerType);
	}

	private void AddHeaderInternal(
		string headerName,
		Type dataTableHeaderType,
		AbstractParameterType? headerType
	)
	{
		headerType = _abstractParameterTypes.FirstOrDefault(p => p.Equals(headerType));
		if (headerType is null)
			return;

		if (_headerTypes.TryAdd(headerName, headerType))
		{
			DataColumn tableColumn = Table.Columns.Add(headerName, dataTableHeaderType);
			tableColumn.Caption = headerName;
		}
	}

	public string ConvertToString()
	{
		StringBuilder strBuilder = new();

		strBuilder.AppendLine(
			string.Join(
				_headerDelimiter,
				Table
					.Columns.Cast<DataColumn>()
					.Where(c => c is not null)
					.Select(c => (c, _headerTypes[c.Caption]))
					.Select(pair =>
						$"{GetHeaderForFirstColumn(pair.c)}" + $"{pair.Item2.SizeTablesTypeName}"
					)
			)
		);

		foreach (DataRow row in Table.Rows)
		{
			strBuilder.AppendLine(
				string.Join(
					_headerDelimiter,
					Table
						.Columns.Cast<DataColumn>()
						.Select(c => Validate(row[c].ToString() ?? string.Empty, c.DataType))
				)
			);
		}

		return strBuilder.ToString();
	}

	private string GetHeaderForFirstColumn(DataColumn c) =>
		c.Caption == "_" ? string.Empty : c.Caption;

	private string Validate(string str, Type columnType) =>
		columnType == typeof(string) ? ValidateAsText(str) : ValidateAsNumber(str);

	private string ValidateAsText(string str)
	{
		if (!str.IsValid())
			return str;
		return $"\"{str.Replace("\"", "\"\"")}\"";
	}

	private string ValidateAsNumber(string str) => str.Replace(_systemDecimalSeparator, ".");

	public void PasteFromClipboard(int rowIndex, int columnIndex)
	{
		DataTable dataTable = Table;

		string clipboardContent = Clipboard.GetText();

		var cells = clipboardContent.ParseAsCells();

		foreach (var cell in cells)
		{
			var curRowIndex = rowIndex + cell.RowIndex;
			if (curRowIndex >= dataTable.Rows.Count)
				dataTable.Rows.Add(dataTable.NewRow());

			var curColumnIndex = columnIndex + cell.ColumnIndex;
			if (curColumnIndex >= dataTable.Columns.Count)
				continue;

			try
			{
				Type columnType = dataTable.Columns[curColumnIndex].DataType;
				dataTable.Rows[curRowIndex][curColumnIndex] =
					columnType == typeof(string) ? cell.Text : cell.Text.ToDouble();
			}
			catch
			{
				// ignored
			}
		}
	}
}
