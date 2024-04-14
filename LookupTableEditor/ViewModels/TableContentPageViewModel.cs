using System.Data;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Extentions;
using LookupTableEditor.Services;

namespace LookupTableEditor.Views
{
    public partial class TableContentPageViewModel : ObservableObject
    {
        private int selectedColumnIndex;

        private SizeTableService _sizeTableService;
        public SizeTableInfo? SizeTableInfo { get; set; }

        public int SelectedColumnIndex
        {
            get { return selectedColumnIndex; }
            set
            {
                selectedColumnIndex = value;
                SelectedColumnName = SizeTableInfo.Table.Columns[value].ColumnName;
                SelectedColumnType = SizeTableInfo.GetColumnType(SelectedColumnName);
                SelectedColumnSizeTableType = SizeTableInfo.GetColumnSizeTableType(
                    SelectedColumnType
                );
            }
        }

        [ObservableProperty]
        private string _selectedColumnName;

        [ObservableProperty]
        private AbstractParameterType _selectedColumnType;

        [ObservableProperty]
        private string _selectedColumnSizeTableType;

        public List<AbstractParameterType> ParameterTypes { get; } =
            AbstractParameterType.GetAllTypes();

        public TableContentPageViewModel(
            SizeTableService sizeTableService,
            SizeTableInfo sizeTableInfo
        )
        {
            _sizeTableService = sizeTableService.ThrowIfNull();
            SizeTableInfo = sizeTableInfo;
        }

        partial void OnSelectedColumnNameChanged(string? oldValue, string newValue)
        {
            SizeTableInfo.ChangeColumnName(oldValue, newValue, SelectedColumnType);
        }

        partial void OnSelectedColumnTypeChanged(AbstractParameterType value)
        {
            SizeTableInfo.ChangeColumnType(SelectedColumnName, value);
        }

        [RelayCommand]
        private void SaveSizeTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            Process.Start(SizeTableInfo.FilePath);
        }

        [RelayCommand]
        private void AddRowOnTop() => AddRowOnTop(0);

        public void AddRowOnTop(int index)
        {
            SizeTableInfo.Table?.Rows.InsertAt(SizeTableInfo.Table.NewRow(), index);
        }

        public void PasteFromClipboard(int rowIndx, int columnIndx)
        {
            if (SizeTableInfo.Table == null || !Clipboard.ContainsText())
                return;

            int tmpColIndx = 0;
            int tmpRowIndx = 0;
            DataTable dataTable = SizeTableInfo.Table;

            var clipboardContent = Clipboard.GetText();
            var rows = clipboardContent
                .Split(new string[] { "\r\n" }, StringSplitOptions.None)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            foreach (string row in rows)
            {
                if (rowIndx + tmpRowIndx >= dataTable.Rows.Count)
                    dataTable.Rows.Add(dataTable.NewRow());
                tmpColIndx = 0;
                string[] columnsValue = row.Split('\t');
                foreach (string columnValue in columnsValue)
                {
                    if (columnIndx + tmpColIndx >= dataTable.Columns.Count)
                        break;

                    try
                    {
                        if (dataTable.Columns[tmpColIndx].DataType == Type.GetType("System.String"))
                            dataTable.Rows[rowIndx + tmpRowIndx][columnIndx + tmpColIndx] =
                                columnValue.ToString();
                        else
                            dataTable.Rows[rowIndx + tmpRowIndx][columnIndx + tmpColIndx] =
                                double.Parse(columnValue);
                    }
                    catch { }
                    tmpColIndx++;
                }
                tmpRowIndx++;
            }
        }
    }
}
