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
        private readonly AbstractParameterTypesProvider _parameterTypesProvider;
        private int selectedColumnIndex;
        public int? SelectedRowIndex { get; set; }
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

        public List<AbstractParameterType> ParameterTypes { get; }

        [ObservableProperty]
        private SizeTableInfo? _sizeTableInfo;

        [ObservableProperty]
        private string _selectedColumnName;

        [ObservableProperty]
        private AbstractParameterType _selectedColumnType;

        [ObservableProperty]
        private List<string> _sizeTableNames = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTableNotExist))]
        private string _curTableName = string.Empty;

        public TableContentPageViewModel(
            SizeTableService sizeTableService,
            AbstractParameterTypesProvider parameterTypesProvider
        )
        {
            _sizeTableService = sizeTableService;
            _parameterTypesProvider = parameterTypesProvider;

            SelectedColumnType = _parameterTypesProvider.Empty();

            SizeTableNames = _sizeTableService.Manager.GetAllSizeTableNames().ToList();
            CurTableName = SizeTableNames.FirstOrDefault();

            ParameterTypes = _sizeTableService
                .AbstractParameterTypes.Where(p => p.Label.IsValid())
                .OrderBy(p => p.Label)
                .ToList();
        }

        #region Handlers
        partial void OnCurTableNameChanged(string value)
        {
            if (!value.IsValid())
            {
                return;
            }
            if (SizeTableInfo is null || _sizeTableService.Manager.HasSizeTable(value))
            {
                SizeTableInfo = _sizeTableService.GetSizeTableInfo(value);
            }
            else
            {
                SizeTableInfo.Name = value;
            }
        }

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
        private void CreateNewTable()
        {
            if (!CurTableName.IsValid())
                return;
            SizeTableInfo = _sizeTableService.GetSizeTableInfo(CurTableName);
        }

        [RelayCommand]
        private void SaveSizeTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            Process.Start(SizeTableInfo.FilePath);
        }

        [RelayCommand]
        private void AddRowOnTop() => AddRowOnTop(SelectedRowIndex.Value);

        [RelayCommand]
        private void SetNewTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            _sizeTableService.ImportSizeTable(SizeTableInfo);
            SizeTableInfo = _sizeTableService.GetSizeTableInfo(CurTableName);
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

        public void PasteFromClipboard()
        {
            if (SizeTableInfo?.Table == null || !Clipboard.ContainsText())
                return;

            int rowIndx = SelectedRowIndex.Value;
            int columnIndx = SelectedColumnIndex;

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
