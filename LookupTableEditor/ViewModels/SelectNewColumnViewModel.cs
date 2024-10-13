using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Models;
using LookupTableEditor.Services;
using LookupTableEditor.Views;

namespace LookupTableEditor.ViewModels
{
    public partial class SelectNewColumnViewModel : ObservableObject
    {
        private readonly TableContentPageViewModel _tableContentPageViewModel;
        private readonly FamiliesService _familiesService;
        private readonly AbstractParameterTypesProvider _parameterTypesProvider;

        public Action? OnClosed { get; set; }
        public ObservableCollection<FamilyParameterModel> Parameters { get; set; } = new();
        public CollectionViewSource CollectionViewSource { get; set; }

        public SelectNewColumnViewModel(
            TableContentPageViewModel tableContentPageViewModel,
            FamiliesService familiesService,
            AbstractParameterTypesProvider parameterTypesProvider
        )
        {
            _tableContentPageViewModel = tableContentPageViewModel;
            _familiesService = familiesService;
            _parameterTypesProvider = parameterTypesProvider;

            foreach (var fp in _familiesService.GetFamilyParameters())
            {
                var famParamType = _parameterTypesProvider.FromFamilyParameter(fp);
                var famParamModel = new FamilyParameterModel(fp, famParamType)
                {
                    Value = _familiesService.GetValueAsString(fp),
                };

                Parameters.Add(famParamModel);
            }

            CollectionViewSource = new CollectionViewSource() { Source = Parameters };

            CollectionViewSource.GroupDescriptions.Add(
                new PropertyGroupDescription(nameof(FamilyParameterModel.GroupName))
            );
            CollectionViewSource.GroupDescriptions.Add(
                new PropertyGroupDescription(nameof(FamilyParameterModel.IsInstance))
            );
            CollectionViewSource.SortDescriptions.Add(
                new SortDescription(
                    nameof(FamilyParameterModel.GroupName),
                    ListSortDirection.Ascending
                )
            );
        }

        [RelayCommand]
        private void Cancel() => OnClosed?.Invoke();

        [RelayCommand]
        private void OK()
        {
            Parameters
                .Where(fp => fp.IsSelected)
                .ToList()
                .ForEach(fp =>
                    _tableContentPageViewModel.SizeTableInfo.AddHeader(fp.FamilyParameter)
                );
            OnClosed?.Invoke();
        }
    }
}
