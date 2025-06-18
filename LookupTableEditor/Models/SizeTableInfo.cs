using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
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
            var values = Table
                .Columns.OfType<DataColumn>()
                .Select(c => Validate(row[c].ToString() ?? string.Empty, c.DataType));

            strBuilder.AppendLine(string.Join(_headerDelimiter, values));
        }

        return strBuilder.ToString();
    }

    private string GetHeaderForFirstColumn(DataColumn c) =>
        c.Caption == "_" ? string.Empty : c.Caption;

    private string Validate(string str, Type columnType) =>
        columnType == typeof(string) ? ValidateAsText(str) : ValidateAsNumber(str);

    private string ValidateAsText(string str)
    {
        return str.AsMaybe()
            .Where((str) => str.IsValid())
            .Select((str) => $"\"{str.Replace("\"", "\"\"")}\"")
            .Or(string.Empty)
            .Value;
    }

    private string ValidateAsNumber(string str) => str.Replace(_systemDecimalSeparator, ".");

    public Result PasteFromClipboard(int? rowIndex, int? columnIndex)
    {
        if (!Clipboard.ContainsText() || rowIndex is null || columnIndex is null)
            return Result.Failure(
                $"Clipboard.ContainsText() : {Clipboard.ContainsText()}\n"
                    + $"rowIndex is null : {rowIndex is null}\n"
                    + $"columnIndex is null : {columnIndex is null}"
            );

        var cells = Clipboard.GetText().ParseAsCells();

        foreach (var cell in cells)
        {
            var curRowIndex = rowIndex.Value + cell.RowIndex;
            if (curRowIndex >= Table.Rows.Count)
                Table.Rows.Add(Table.NewRow());

            var curColumnIndex = columnIndex.Value + cell.ColumnIndex;
            if (curColumnIndex >= Table.Columns.Count)
                continue;

            var res = Result.Try(
                () =>
                {
                    Type columnType = Table.Columns[curColumnIndex].DataType;
                    Table.Rows[curRowIndex][curColumnIndex] =
                        columnType == typeof(string) ? cell.Text : cell.Text.ToDouble();
                },
                (e) =>
                    $"Ошибка при попытке вставить значения из буфера обмена.\n"
                    + $"Значения в буфере обмена: {Clipboard.GetText()}\n"
                    + $"Эксепа: {e.Message}"
            );
            if (res.IsFailure)
                return res;
        }
        return Result.Success();
    }
}
