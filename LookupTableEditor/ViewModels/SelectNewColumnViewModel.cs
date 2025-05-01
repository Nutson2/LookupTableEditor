using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;
using LookupTableEditor.Services;

namespace LookupTableEditor.ViewModels;

public partial class SelectNewColumnViewModel : ObservableObject
{
	public SelectNewColumnViewModel(FamiliesService familiesService)
	{
		Parameters = new ObservableCollection<FamilyParameterModel>(familiesService.GetFamilyParameters());

		CollectionViewSource = new CollectionViewSource { Source = Parameters };

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

	public SizeTableInfo? SizeTableInfo { get; set; }
	private ObservableCollection<FamilyParameterModel> Parameters { get; }
	public CollectionViewSource CollectionViewSource { get; set; }

	public event Action? OnClosed;

	[RelayCommand]
	private void Cancel()
	{
		OnClosed?.Invoke();
	}

	[RelayCommand]
	private void Ok()
	{
		Parameters
		   .Where(fp => fp.IsSelected)
		   .ForEach(fp => SizeTableInfo?.AddHeader(fp.FamilyParameter));
		SizeTableInfo = null;
		OnClosed?.Invoke();
	}
}