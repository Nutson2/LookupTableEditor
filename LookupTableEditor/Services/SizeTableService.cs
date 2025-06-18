using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

namespace LookupTableEditor.Services;

public class SizeTableService : IDisposable
{
    static Encoding win1251 = Encoding.GetEncoding("Windows-1251");

    private readonly RevitApplication _app;
    private readonly Document _doc;
    private readonly AbstractParameterTypesProvider _parameterTypesProvider;
    public FamilySizeTableManager Manager { get; }
    public List<AbstractParameterType> AbstractParameterTypes { get; }

    public SizeTableService(
        Document doc,
        RevitApplication application,
        AbstractParameterTypesProvider parameterTypesProvider
    )
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        _app = application ?? throw new ArgumentNullException(nameof(application));
        _parameterTypesProvider =
            parameterTypesProvider
            ?? throw new ArgumentNullException(nameof(parameterTypesProvider));

        Manager = GetOrCreateSizeTableManager();

        var defs = GetDefenitionsOfParameterType();

        AbstractParameterTypes =
            defs?.ConvertAll(def => _parameterTypesProvider.FromDefinitionOfParameterType(def))
            ?? new();
    }

    public void Dispose() => Manager.Dispose();

    private List<DefinitionOfParameterType>? GetDefenitionsOfParameterType()
    {
        var parametersTypes = _app.VersionNumber.ToInt() switch
        {
            2020 => Resource.ParametersTypes2020,
            2021 => Resource.ParametersTypes2021,
            2022 => Resource.ParametersTypes2022,
            2023 => Resource.ParametersTypes2023,
            2024 => Resource.ParametersTypes2024,
            _ => Resource.ParametersTypes2024,
        };
        using MemoryStream stream = new(parametersTypes);
        XmlSerializer xmlSerializer = new(typeof(List<DefinitionOfParameterType>));
        return xmlSerializer.Deserialize(stream) as List<DefinitionOfParameterType>;
    }

    private FamilySizeTableManager GetOrCreateSizeTableManager()
    {
        _doc.Run(
            "Create manager",
            () => FamilySizeTableManager.CreateFamilySizeTableManager(_doc, _doc.OwnerFamily.Id)
        );
        return FamilySizeTableManager.GetFamilySizeTableManager(_doc, _doc.OwnerFamily.Id)
            ?? throw new NullReferenceException(
                $"{nameof(FamilySizeTableManager.GetFamilySizeTableManager)} return null\n"
            );
    }

    public Result<SizeTableInfo> GetSizeTableInfo(string name)
    {
        SizeTableInfo sizeTableInfo = new(name, AbstractParameterTypes, _parameterTypesProvider);
        var familySizeTable = Manager.GetSizeTable(name);

        sizeTableInfo.InsertFirstColumn();

        if (familySizeTable == null)
            return sizeTableInfo;

        Enumerable
            .Range(1, familySizeTable.NumberOfColumns - 1)
            .Select(i => familySizeTable.GetColumnHeader(i))
            .ForEach(c => sizeTableInfo.AddHeader(c))
            .ToList();

        for (var rowIndex = 0; rowIndex < familySizeTable.NumberOfRows; rowIndex++)
        {
            var row = sizeTableInfo.Table.NewRow();
            sizeTableInfo.Table.Rows.Add(row);

            for (var columnIndex = 0; columnIndex < familySizeTable.NumberOfColumns; columnIndex++)
            {
                var val = familySizeTable.AsValueString(rowIndex, columnIndex);

                var dataColumn = sizeTableInfo.Table.Columns[columnIndex];

                row[dataColumn.ColumnName] =
                    dataColumn.DataType == typeof(double) ? val.ToDouble() : val;
            }
        }

        return sizeTableInfo;
    }

    public Result ImportSizeTable(SizeTableInfo tableInfo)
    {
        FamilySizeTableErrorInfo errorInfo = new();

        _doc.Run(
            "Set size table",
            () =>
            {
                if (Manager.HasSizeTable(tableInfo.Name))
                    Manager.RemoveSizeTable(tableInfo.Name);

                Manager.ImportSizeTable(_doc, tableInfo.FilePath, errorInfo);
            }
        );
        return errorInfo.FamilySizeTableErrorType == FamilySizeTableErrorType.Undefined
            ? Result.Success()
            : Result.Failure(
                $"Проблема импорта таблицы:\n"
                    + $"{errorInfo.FamilySizeTableErrorType}\n"
                    + $"{errorInfo.InvalidHeaderText}\n"
                    + $"{errorInfo.InvalidColumnIndex}\n"
                    + $"{errorInfo.InvalidRowIndex}"
            );
    }

    public void SaveAndOpen(SizeTableInfo tableInfo) =>
        SaveSizeTableOnTheDisk(tableInfo).Tap((path) => OpenFile(tableInfo.FilePath!));

    private Result<string> SaveSizeTableOnTheDisk(SizeTableInfo tableInfo)
    {
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        tableInfo.FilePath = Path.Combine(folderPath, tableInfo.Name + ".csv");

        using StreamWriter sw = new(tableInfo.FilePath, false, win1251);
        sw.Write(tableInfo.ConvertToString());
        return tableInfo.FilePath;
    }

    private bool OpenFile(string path) =>
        new Process { StartInfo = new ProcessStartInfo(path) { UseShellExecute = true } }.Start();

    public Result<SizeTableInfo> Update(SizeTableInfo tableInfo)
    {
        return Result
            .Success(tableInfo)
            .Tap((tableinfo) => SaveSizeTableOnTheDisk(tableInfo))
            .Tap((tableinfo) => ImportSizeTable(tableInfo))
            .Bind((tableinfo) => GetSizeTableInfo(tableInfo.Name!));
    }

    public string CreateFormula(
        FamilyParameter parameter,
        SizeTableInfo tableInfo,
        List<FamilyParameter> keyParameters
    )
    {
        if (parameter.Formula != null)
            return parameter.Formula;

        var tableName = $"\"{tableInfo.Name}\"";
        var columnName = parameter.Definition.Name;
        var defaultValue =
            tableInfo.Table.Rows[0][columnName.Replace(".", "_")].ToString()?.Replace("\"", "")
            ?? string.Empty;

        var keys = string.Join(", ", keyParameters.Select(x => x.Definition.Name));

        var res = $"size_lookup({tableName}, \"{columnName}\", \"{defaultValue}\" {keys})";
        return res;
    }
}
