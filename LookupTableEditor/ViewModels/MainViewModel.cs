using System.Diagnostics;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Services;
using LookupTableEditor.Views;

namespace LookupTableEditor.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SizeTableService _sizeTableService;
        private SizeTableInfo _sizeTableInfo;

        [ObservableProperty]
        private Page _currentPage;

        [ObservableProperty]
        private List<string> _sizeTableNames = new();

        [ObservableProperty]
        private string _curTableName = string.Empty;

        partial void OnCurTableNameChanged(string value) =>
            _sizeTableInfo = _sizeTableService.GetSizeTableInfo(value);

        public MainViewModel(SizeTableService sizeTableService)
        {
            _sizeTableService = sizeTableService;

            SizeTableNames = _sizeTableService.Manager.GetAllSizeTableNames().ToList();
            CurTableName = SizeTableNames.FirstOrDefault();
        }

        [RelayCommand]
        private void SetTableContentPage() =>
            CurrentPage = new TableContentPage(
                new TableContentPageViewModel(_sizeTableService, _sizeTableInfo)
            );

        [RelayCommand]
        private void SetTableDefinitionPage() =>
            CurrentPage = new TableDefinitionPage(new TableDefinitionPageViewModel(_sizeTableInfo));

        [RelayCommand]
        private void SetTableFormulasPage() =>
            CurrentPage = new TableFormulasPage(new TableFormulasViewModel(_sizeTableInfo));

        [RelayCommand]
        private void SetNewTable()
        {
            _sizeTableService.SaveSizeTableOnTheDisk(_sizeTableInfo);
            _sizeTableService.ImportSizeTable(_sizeTableInfo);
        }

        [RelayCommand]
        private void GotoTelegram() => Process.Start(Settings.Default.TelegramUrl);

        [RelayCommand]
        private void GoToYouTube() => Process.Start(Settings.Default.YouTubeChannelUrl);
    }
}
