using System.Data;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Extentions;
using LookupTableEditor.Services;

namespace LookupTableEditor
{
    public partial class LookupTableViewModel : ObservableObject
    {
        private readonly SizeTableService _sizeTableService;
        public SizeTableInfo? SizeTableInfo { get; set; }

        [ObservableProperty]
        private List<string> _sizeTableNames = new();

        [ObservableProperty]
        private string _curTableName = string.Empty;

        partial void OnCurTableNameChanged(string value) =>
            SizeTableInfo = _sizeTableService.GetSizeTableInfo(value);

        public LookupTableViewModel(SizeTableService sizeTableService)
        {
            _sizeTableService = sizeTableService.ThrowIfNull();

            SizeTableNames = _sizeTableService.Manager.GetAllSizeTableNames().ToList();
            CurTableName = SizeTableNames.FirstOrDefault();
        }

        [RelayCommand]
        private void SaveSizeTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            Process.Start(SizeTableInfo.FilePath);
        }

        [RelayCommand]
        private void SetNewTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            _sizeTableService.ImportSizeTable(SizeTableInfo);
        }

        [RelayCommand]
        private void AddRowOnTop() =>
            SizeTableInfo.Table?.Rows.InsertAt(SizeTableInfo.Table.NewRow(), 0);

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

        [RelayCommand]
        private void GotoTelegram() => Process.Start(Settings.Default.TelegramUrl);

        [RelayCommand]
        private void GoToYouTube() => Process.Start(Settings.Default.YouTubeChannelUrl);
    }
}
