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
        private readonly SizeTableService _sizeTableService;
        private int selectedColumnIndex;

        public Action? OnColumnNameChanged { get; set; }
        public Action? OnAddNewColumn { get; set; }

        public bool IsTableNotExist => !SizeTableNames.Contains(CurTableName);
        public bool IsSizeTableInfoExist => SizeTableInfo is not null;
        public int SelectedColumnIndex
        {
            get { return selectedColumnIndex; }
            set
            {
                selectedColumnIndex = value;
                SelectedColumnName = SizeTableInfo.Table.Columns[value].Caption;
                SelectedColumnType = SizeTableInfo.GetColumnType(SelectedColumnName);
                OnPropertyChanged(nameof(SelectedColumnType));
            }
        }
        public SizeTableInfo? SizeTableInfo { get; set; }
        public List<AbstractParameterType> ParameterTypes { get; }

        [ObservableProperty]
        private string _selectedColumnName;

        [ObservableProperty]
        private AbstractParameterType _selectedColumnType = AbstractParameterType.Empty();

        [ObservableProperty]
        private List<string> _sizeTableNames = new();

        [ObservableProperty]
        private string _curTableName = string.Empty;

        public TableContentPageViewModel(SizeTableService sizeTableService)
        {
            _sizeTableService = sizeTableService;
            SizeTableNames = _sizeTableService.Manager.GetAllSizeTableNames().ToList();
            CurTableName = SizeTableNames.FirstOrDefault();

            ParameterTypes = _sizeTableService
                .AbstractParameterTypes.Where(p => p.Label.IsValid())
                .OrderBy(p => p.Label)
                .ToList();
        }

        #region Handlers
        partial void OnCurTableNameChanged(string value) =>
            SizeTableInfo = _sizeTableService.GetSizeTableInfo(value);

        partial void OnSelectedColumnNameChanged(string? oldValue, string newValue)
        {
            if (oldValue is not null && !oldValue.IsValid() || !newValue.IsValid())
                return;
            SizeTableInfo?.ChangeColumnName(
                SelectedColumnIndex,
                oldValue,
                newValue,
                SelectedColumnType
            );
            OnColumnNameChanged?.Invoke();
        }

        partial void OnSelectedColumnTypeChanged(AbstractParameterType value) =>
            SizeTableInfo?.ChangeColumnType(SelectedColumnName, value);

        #endregion

        #region Commands

        [RelayCommand]
        private void CreateNewTable(string name) =>
            SizeTableInfo = _sizeTableService.GetSizeTableInfo(name);

        [RelayCommand]
        private void SaveSizeTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            Process.Start(SizeTableInfo.FilePath);
        }

        [RelayCommand]
        private void AddRowOnTop() => AddRowOnTop(0);

        [RelayCommand]
        private void SetNewTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            _sizeTableService.ImportSizeTable(SizeTableInfo);
        }

        [RelayCommand]
        private void AddNewColumn()
        {
            OnAddNewColumn?.Invoke();
        }

        #endregion
        public void AddRowOnTop(int index)
        {
            SizeTableInfo.Table?.Rows.InsertAt(SizeTableInfo.Table.NewRow(), index);
        }

        public void PasteFromClipboard(int rowIndx, int columnIndx)
        {
            if (SizeTableInfo?.Table == null || !Clipboard.ContainsText())
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
