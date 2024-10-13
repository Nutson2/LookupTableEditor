using System.Data;
using System.Globalization;
using System.Text;
using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;
using LookupTableEditor.Services;

namespace LookupTableEditor
{
    public partial class SizeTableInfo
    {
        private readonly string _headerDelimiter = ",";
        private readonly string systemDecimalSeparator = CultureInfo
            .CurrentCulture
            .NumberFormat
            .NumberDecimalSeparator;

        private readonly Dictionary<string, AbstractParameterType> _headerTypes = new();
        private readonly List<AbstractParameterType> _abstractParameterTypes;
        private readonly AbstractParameterTypesProvider _parameterTypesProvider;

        public string? Name { get; set; }
        public string? FilePath { get; set; }
        public DataTable Table { get; } = new DataTable();

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
            Table.Columns.Add("_", Type.GetType("System.String"));
            _headerTypes.Add("_", _parameterTypesProvider.Empty());
        }

        public void AddHeader(FamilySizeTableColumn column)
        {
            var dataTableHeaderType = column.GetTypeForDataTable();
            var headerType = _parameterTypesProvider.FromSizeTableColumn(column);

            headerType = _abstractParameterTypes.FirstOrDefault(p => p.Equals(headerType));
            if (headerType is null)
                return;

            var headerName = column.Name;

            _headerTypes.Add(headerName, headerType);
            DataColumn tableColumn = Table.Columns.Add(headerName, dataTableHeaderType);
            tableColumn.Caption = headerName;
        }

        public void AddHeader(FamilyParameter parameter)
        {
            var dataTableHeaderType = parameter.Definition.GetTypeForDataTable();
            var headerType = _parameterTypesProvider.FromFamilyParameter(parameter);
            headerType = _abstractParameterTypes.FirstOrDefault(p => p.Equals(headerType));
            if (headerType is null)
                return;
            var headerName = parameter.Definition.Name;

            _headerTypes.Add(headerName, headerType);
            DataColumn tableColumn = Table.Columns.Add(headerName, dataTableHeaderType);
            tableColumn.Caption = headerName;
        }

        public string ConvertToString()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine(
                string.Join(
                    _headerDelimiter,
                    Table
                        .Columns.Cast<DataColumn>()
                        .Select(c => (c, _headerTypes[c.Caption]))
                        .Select(pair =>
                            $"{GetHeaderForFirstColumn(pair.c)}"
                            + $"{pair.Item2.SizeTablesTypeName}"
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
                            .Select(c => Validate(row[c].ToString(), c.DataType))
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

        private string ValidateAsNumber(string str) => str.Replace(systemDecimalSeparator, ".");

        internal AbstractParameterType GetColumnType(string selectedColumnName) =>
            _headerTypes.ContainsKey(selectedColumnName)
                ? _headerTypes[selectedColumnName]
                : _parameterTypesProvider.Empty();

        internal void ChangeColumnName(
            int selectedColumnIndex,
            string? oldValue,
            string newValue,
            AbstractParameterType selectedColumnType
        )
        {
            var column = Table.Columns[selectedColumnIndex];
            if (column.Caption != oldValue)
                return;
            column.Caption = newValue;
            _headerTypes.Remove(oldValue);
            _headerTypes.Add(newValue, selectedColumnType);
        }

        internal void ChangeColumnType(string selectedColumnName, AbstractParameterType value)
        {
            _headerTypes[selectedColumnName] = value;
        }
    }
}
