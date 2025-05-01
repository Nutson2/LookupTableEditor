using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

namespace LookupTableEditor.Services;

public class SizeTableService : IDisposable
{
    private readonly RevitApplication _app;
    private readonly Document _doc;
    private readonly AbstractParameterTypesProvider _parameterTypesProvider;

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

        AbstractParameterTypes = defs.ConvertAll(def =>
            _parameterTypesProvider.FromDefinitionOfParameterType(def)
        );
    }

    public FamilySizeTableManager Manager { get; }
    public List<AbstractParameterType> AbstractParameterTypes { get; }

    public void Dispose()
    {
        Manager.Dispose();
    }

    private List<DefinitionOfParameterType> GetDefenitionsOfParameterType()
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
        return (List<DefinitionOfParameterType>)xmlSerializer.Deserialize(stream);
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

    public SizeTableInfo GetSizeTableInfo(string name)
    {
        SizeTableInfo sizeTableInfo = new(name, AbstractParameterTypes, _parameterTypesProvider);
        var familySizeTable = Manager.GetSizeTable(name);

        sizeTableInfo.InsertFirstColumn();

        if (familySizeTable == null)
            return sizeTableInfo;

        for (var columnIndx = 1; columnIndx < familySizeTable.NumberOfColumns; columnIndx++)
        {
            var column = familySizeTable.GetColumnHeader(columnIndx);
            sizeTableInfo.AddHeader(column);
        }

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

    public void ImportSizeTable(SizeTableInfo tableInfo)
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

        if (errorInfo.FamilySizeTableErrorType != FamilySizeTableErrorType.Undefined)
            TaskDialog.Show(
                "Проблема импорта таблицы",
                errorInfo.FamilySizeTableErrorType
                    + "\n"
                    + errorInfo.InvalidHeaderText
                    + "\n"
                    + errorInfo.InvalidColumnIndex
                    + "\n"
                    + errorInfo.InvalidRowIndex
            );
    }

    public void SaveSizeTableOnTheDisk(SizeTableInfo tableInfo)
    {
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        tableInfo.FilePath = Path.Combine(folderPath, tableInfo.Name + ".csv");

        using StreamWriter sw = new(tableInfo.FilePath, false, Encoding.Default);
        sw.Write(tableInfo.ConvertToString());

        Process.Start(tableInfo.FilePath);
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
        var defaultValue = tableInfo
            .Table.Rows[0][columnName.Replace(".", "_")]
            .ToString()
            .Replace("\"", "");

        var keys = string.Join(", ", keyParameters.Select(x => x.Definition.Name));

        var res = $"size_lookup({tableName}, \"{columnName}\", \"{defaultValue}\" {keys})";
        return res;
    }
}
