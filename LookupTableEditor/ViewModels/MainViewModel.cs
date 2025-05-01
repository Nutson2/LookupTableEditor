using System.Diagnostics;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Models;
using LookupTableEditor.Services;
using LookupTableEditor.Views.Pages;

namespace LookupTableEditor.ViewModels;

public partial class MainViewModel : ObservableObject
{
	private readonly SelectNewColumnViewModel _selectNewColumnPageVm;
	private readonly TableContentPageViewModel _tableContentPageVm;

	[ObservableProperty] private Page? _currentPage;

	public MainViewModel(SizeTableService sizeTableService,
						 FamiliesService familiesService,
						 AbstractParameterTypesProvider parameterTypesProvider
	)
	{
		_tableContentPageVm = new TableContentPageViewModel(sizeTableService, parameterTypesProvider);
		_tableContentPageVm.OnAddNewColumn += SetSelectNewColumnPage;

		_selectNewColumnPageVm = new SelectNewColumnViewModel(familiesService);
		_selectNewColumnPageVm.OnClosed += SetTableContentPage;

		SetTableContentPage();
	}

	[RelayCommand]
	private void SetTableContentPage()
	{
		CurrentPage = new TableContentPage(_tableContentPageVm);
	}

	private void SetSelectNewColumnPage(SizeTableInfo sizeTableInfo)
	{
		_selectNewColumnPageVm.SizeTableInfo = sizeTableInfo;
		CurrentPage = new SelectNewColumnPage(_selectNewColumnPageVm);
	}

	[RelayCommand]
	private void SetTableFormulasPage()
	{
		CurrentPage = new TableFormulasPage(new TableFormulasViewModel());
	}

	[RelayCommand]
	private void GotoTelegram()
	{
		Process.Start(Settings.Default.TelegramUrl);
	}

	[RelayCommand]
	private void GoToYouTube()
	{
		Process.Start(Settings.Default.YouTubeChannelUrl);
	}
}