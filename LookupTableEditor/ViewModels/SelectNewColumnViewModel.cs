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

        public Action? OnClosed { get; set; }
        public ObservableCollection<FamilyParameterExtended> Parameters { get; set; } = new();
        public CollectionViewSource CollectionViewSource { get; set; }

        public SelectNewColumnViewModel(
            TableContentPageViewModel tableContentPageViewModel,
            FamiliesService familiesService
        )
        {
            _tableContentPageViewModel = tableContentPageViewModel;
            _familiesService = familiesService;

            _familiesService
                .GetFamilyParameters()
                .ToList()
                .ForEach(fp =>
                {
                    Parameters.Add(
                        new FamilyParameterExtended(fp)
                        {
                            Value = _familiesService.GetValueAsString(fp)
                        }
                    );
                    ;
                });

            CollectionViewSource = new CollectionViewSource() { Source = Parameters };

            CollectionViewSource.GroupDescriptions.Add(
                new PropertyGroupDescription(nameof(FamilyParameterExtended.GroupName))
            );
            CollectionViewSource.GroupDescriptions.Add(
                new PropertyGroupDescription(nameof(FamilyParameterExtended.IsInstance))
            );
            CollectionViewSource.SortDescriptions.Add(
                new SortDescription(
                    nameof(FamilyParameterExtended.GroupName),
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
