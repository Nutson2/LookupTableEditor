using System.Data;
using System.Globalization;
using System.Text;
using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;

namespace LookupTableEditor
{
    public partial class SizeTableInfo
    {
        private readonly Dictionary<string, string> _headerConverter;
        private readonly string _headerDelimiter = ",";
        private readonly string systemDecimalSeparator = CultureInfo
            .CurrentCulture
            .NumberFormat
            .NumberDecimalSeparator;

        public string? Name { get; set; }
        public string? FilePath { get; set; }
        public DataTable Table { get; } = new DataTable();
        public List<Header> Headers { get; } = new();

        public SizeTableInfo(string? name, Dictionary<string, string> headerConverter)
        {
            Name = name;
            _headerConverter = headerConverter;
        }

        public void InsertFirstColumn()
        {
            Headers.Add(new Header());
            Table.Columns.Add(" ", Type.GetType("System.String"));
        }

        public void AddHeader(FamilySizeTableColumn column)
        {
            var headerName = column.Name.Replace(".", "_");
            var dataTableHeaderType = column.GetTypeForDataTable();

            var headerType = column.GetHeaderType();

            AddHeader(headerName, dataTableHeaderType, headerType);
        }

        public void AddHeader(FamilyParameter parameter)
        {
            var headerName = parameter.Definition.Name.Replace(".", "_");
            var dataTableHeaderType = parameter.GetTypeForDataTable();
            var headerType = parameter.GetParameterType();

            AddHeader(headerName, dataTableHeaderType, headerType);
        }

        private void AddHeader(string headerName, Type dataTableHeaderType, ForgeTypeId headerType)
        {
            Headers.Add(new Header() { Name = headerName, Type = headerType });
            Table.Columns.Add(headerName, dataTableHeaderType);
        }

        public string ConvertToString()
        {
            var strBuilder = new StringBuilder();

            Headers.ForEach(h => h.Index = Table.Columns.IndexOf(h.Name));

            strBuilder.AppendLine(
                string.Join(
                    _headerDelimiter,
                    Headers.OrderBy(h => h.Index).Select(h => h.Name + TryGetValue(h.TypeString))
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

        private string TryGetValue(string key) =>
            key.IsValid() ? _headerConverter[key] : string.Empty;

        private string Validate(string str, Type columnType) =>
            columnType == typeof(string) ? ValidateAsText(str) : ValidateAsNumber(str);

        private string ValidateAsText(string str) => $"\"{str.Replace("\"", "\"\"")}\"";

        private string ValidateAsNumber(string str) => str.Replace(systemDecimalSeparator, ".");
    }
}
