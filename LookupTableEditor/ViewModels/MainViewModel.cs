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
        private readonly AbstractParameterTypesProvider _parameterTypesProvider;
        private readonly TableContentPageViewModel tableContentPageVM;

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

            tableContentPageVM = new TableContentPageViewModel(
                _sizeTableService,
                _parameterTypesProvider
            );
            tableContentPageVM.OnAddNewColumn = SetSelectNewColumnPage;

            SetTableContentPage();
        }

        [RelayCommand]
        private void SetTableContentPage()
        {
            CurrentPage = new TableContentPage(tableContentPageVM);
        }

        private void SetSelectNewColumnPage()
        {
            var vm = new SelectNewColumnViewModel(
                tableContentPageVM,
                _familiesService,
                _parameterTypesProvider
            );
            vm.OnClosed = SetTableContentPage;

            CurrentPage = new SelectNewColumnPage(vm);
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
