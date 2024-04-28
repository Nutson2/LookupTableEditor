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
        private readonly FamiliesService _familiesService;
        private SizeTableInfo _sizeTableInfo;

        [ObservableProperty]
        private Page _currentPage;

        [ObservableProperty]
        private List<string> _sizeTableNames = new();

        [ObservableProperty]
        private string _curTableName = string.Empty;
        public bool IsTableNotExist => !SizeTableNames.Contains(CurTableName);
        public bool CanSelectPage => _sizeTableInfo != null;

        partial void OnCurTableNameChanged(string value) =>
            _sizeTableInfo = _sizeTableService.GetSizeTableInfo(value);

        public MainViewModel(SizeTableService sizeTableService, FamiliesService familiesService)
        {
            _sizeTableService = sizeTableService;
            _familiesService = familiesService;
            SizeTableNames = _sizeTableService.Manager.GetAllSizeTableNames().ToList();
            CurTableName = SizeTableNames.FirstOrDefault();

            SetTableContentPage();
        }

        [RelayCommand]
        private void CreateNewTable(string name) =>
            _sizeTableInfo = _sizeTableService.GetSizeTableInfo(name);

        [RelayCommand]
        private void SetTableContentPage()
        {
            var vm = new TableContentPageViewModel(_sizeTableService, _sizeTableInfo);
            CurrentPage = new TableContentPage(vm);
        }

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
