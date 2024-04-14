using System.Globalization;
using System.IO;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LookupTableEditor.Extentions;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

namespace LookupTableEditor.Services
{
    public class SizeTableService
    {
        private readonly Document _doc;
        private readonly RevitApplication _app;
        private readonly Dictionary<string, string> _converter;
        private readonly string systemDecimalSeparator = CultureInfo
            .CurrentCulture
            .NumberFormat
            .NumberDecimalSeparator;

        public FamilySizeTableManager Manager { get; }

        public SizeTableService(Document doc, RevitApplication application)
        {
            _doc = doc;
            _app = application;

            FamilySizeTableManager.CreateFamilySizeTableManager(_doc, _doc.OwnerFamily.Id);
            Manager = FamilySizeTableManager.GetFamilySizeTableManager(_doc, _doc.OwnerFamily.Id);

            _converter = _app.VersionNumber switch
            {
                "2020"
                    => ParametersUnitType.GetDictionaryToConvertParamTypeInSizeTableHeaderString2020(),
                "2021"
                    => ParametersUnitType.GetDictionaryToConvertParamTypeInSizeTableHeaderString2021(),
                "2023"
                    => ParametersUnitType.GetDictionaryToConvertParamTypeInSizeTableHeaderString2023(),
                _ => ParametersUnitType.GetDictionaryToConvertParamTypeInSizeTableHeaderString2020()
            };
        }

        public SizeTableInfo GetSizeTableInfo(string name)
        {
            var dataTableInfo = new SizeTableInfo(name, _converter);
            var familySizeTable = Manager.GetSizeTable(name);

            dataTableInfo.InsertFirstColumn();

            if (familySizeTable == null)
                return dataTableInfo;

            for (int columnIndx = 1; columnIndx < familySizeTable.NumberOfColumns; columnIndx++)
            {
                var column = familySizeTable.GetColumnHeader(columnIndx);
                dataTableInfo.AddHeader(column);
            }

            for (int rowIndex = 0; rowIndex < familySizeTable.NumberOfRows; rowIndex++)
            {
                var row = dataTableInfo.Table.NewRow();
                dataTableInfo.Table.Rows.Add(row);

                for (
                    int columnIndex = 0;
                    columnIndex < familySizeTable.NumberOfColumns;
                    columnIndex++
                )
                {
                    string val = familySizeTable.AsValueString(rowIndex, columnIndex);

                    if (
                        dataTableInfo.Table.Columns[columnIndex].DataType
                        == Type.GetType("System.Double")
                    )
                    {
                        double.TryParse(
                            val.Replace(".", systemDecimalSeparator),
                            out double doubleValue
                        );
                        row[dataTableInfo.Table.Columns[columnIndex].ColumnName] = doubleValue;
                    }
                    else
                    {
                        row[dataTableInfo.Table.Columns[columnIndex].ColumnName] = val;
                    }
                }
            }
            return dataTableInfo;
        }

        public void ImportSizeTable(SizeTableInfo tableInfo)
        {
            FamilySizeTableErrorInfo errorInfo = new FamilySizeTableErrorInfo();

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
            {
                TaskDialog.Show(
                    "Проблема импорта таблицы",
                    errorInfo.FamilySizeTableErrorType.ToString()
                        + "\n"
                        + errorInfo.InvalidHeaderText
                        + "\n"
                        + errorInfo.InvalidColumnIndex
                        + "\n"
                        + errorInfo.InvalidRowIndex
                );
            }
        }

        public void SaveSizeTableOnTheDisk(SizeTableInfo tableInfo)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            tableInfo.FilePath = Path.Combine(folderPath, tableInfo.Name + ".csv");

            using (StreamWriter sw = new(tableInfo.FilePath, false, Encoding.Default))
                sw.Write(tableInfo.ConvertToString());
        }

        public string CreateFormula(
            FamilyParameter Parameter,
            SizeTableInfo tableInfo,
            List<FamilyParameter> keyParameters
        )
        {
            if (Parameter.Formula != null)
                return Parameter.Formula;

            var tableName = $"\"{tableInfo.Name}\"";
            var columnName = Parameter.Definition.Name;
            var defaultValue = tableInfo
                .Table.Rows[0][columnName.Replace(".", "_")]
                .ToString()
                .Replace("\"", "");

            var keys = string.Join(", ", keyParameters.Select(x => x.Definition.Name));

            var res = $"size_lookup({tableName}, \"{columnName}\", \"{defaultValue}\" {keys})";
            return res;
        }
    }
}
