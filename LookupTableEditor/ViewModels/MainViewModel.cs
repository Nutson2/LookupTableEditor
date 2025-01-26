using System.Diagnostics;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Models;
using LookupTableEditor.Services;
using LookupTableEditor.Views;

namespace LookupTableEditor.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SizeTableService _sizeTableService;
        private readonly FamiliesService _familiesService;
        private readonly AbstractParameterTypesProvider _parameterTypesProvider;

        private readonly TableContentPageViewModel _tableContentPageVM;
        private readonly SelectNewColumnViewModel _selectNewColumnPageVM;

        [ObservableProperty]
        private Page? _currentPage;

        public MainViewModel(
            SizeTableService sizeTableService,
            FamiliesService familiesService,
            AbstractParameterTypesProvider parameterTypesProvider
        )
        {
            _sizeTableService = sizeTableService;
            _familiesService = familiesService;
            _parameterTypesProvider = parameterTypesProvider;

            _tableContentPageVM = new(_sizeTableService, _parameterTypesProvider);
            _tableContentPageVM.OnAddNewColumn += SetSelectNewColumnPage;

            _selectNewColumnPageVM = new(_familiesService);
            _selectNewColumnPageVM.OnClosed += SetTableContentPage;

            SetTableContentPage();
        }

        [RelayCommand]
        private void SetTableContentPage() =>
            CurrentPage = new TableContentPage(_tableContentPageVM);

        private void SetSelectNewColumnPage(SizeTableInfo sizeTableInfo)
        {
            _selectNewColumnPageVM.SizeTableInfo = sizeTableInfo;
            CurrentPage = new SelectNewColumnPage(_selectNewColumnPageVM);
        }

        [RelayCommand]
        private void SetTableFormulasPage() =>
            CurrentPage = new TableFormulasPage(new TableFormulasViewModel());

        [RelayCommand]
        private void GotoTelegram() => Process.Start(Settings.Default.TelegramUrl);

        [RelayCommand]
        private void GoToYouTube() => Process.Start(Settings.Default.YouTubeChannelUrl);
    }
}
