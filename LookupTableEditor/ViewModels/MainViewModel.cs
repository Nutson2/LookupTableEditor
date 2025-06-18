using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;
using LookupTableEditor.Services;
using LookupTableEditor.ViewModels.Dialog;
using LookupTableEditor.Views.Pages;

namespace LookupTableEditor.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly SizeTableService _sizeTableService;
    private readonly FamiliesService _familiesService;

    [ObservableProperty]
    private int _selectedColumnIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTableNotExist))]
    private string? _curTableName;

    [ObservableProperty]
    private SizeTableInfo? _sizeTableInfo;

    public ObservableCollection<string> SizeTableNames { get; private set; }

    public bool IsTableNotExist =>
        CurTableName is not null && !SizeTableNames.Contains(CurTableName);

    public int? SelectedRowIndex { get; set; }

    public MainViewModel(
        SizeTableService sizeTableService,
        AbstractParameterTypesProvider parameterTypesProvider,
        FamiliesService familiesService
    )
    {
        _sizeTableService = sizeTableService;
        _familiesService = familiesService;

        SizeTableNames = new(_sizeTableService.Manager.GetAllSizeTableNames().ToList());
        CurTableName = SizeTableNames.FirstOrDefault();
    }

    public void PasteFromClipboard() =>
        SizeTableInfo
            ?.PasteFromClipboard(SelectedRowIndex, SelectedColumnIndex)
            .TapError((msg) => ShowInfo(msg));

    #region Handlers

    partial void OnCurTableNameChanged(string? value)
    {
        value
            .AsMaybe()
            .Where((name) => name.IsValid())
            .ToResult("string is empty")
            .Bind((name) => _sizeTableService.GetSizeTableInfo(name))
            .Tap((table) => SizeTableInfo = table)
            .TapError((msg) => ShowInfo(msg));
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void CreateNewTable()
    {
        var dialogVM = new RequestTableNameVM(this, SetNewTableName);
        DialogPage = new RequestTableName(dialogVM);
    }

    private void SetNewTableName(string curTableName)
    {
        SizeTableNames.Add(curTableName);
        CurTableName = curTableName;
    }

    [RelayCommand]
    private void SaveSizeTable() =>
        Maybe
            .From(SizeTableInfo)
            .ToResult($"SizeTableInfo is null")
            .Tap((sti) => _sizeTableService.SaveAndOpen(sti))
            .TapError((msg) => ShowInfo(msg));

    [RelayCommand]
    private void AddRowOnTop() =>
        SelectedRowIndex
            .AsMaybe()
            .Execute((i) => SizeTableInfo?.Table.Rows.InsertAt(SizeTableInfo.Table.NewRow(), i));

    [RelayCommand]
    private void UpdateTable()
    {
        string message = "Таблица выбора успешно загружена.";
        _sizeTableService
            .Update(SizeTableInfo!)
            .Tap((tableInfo) => SizeTableInfo = tableInfo)
            .TapError((msg) => ShowInfo(msg))
            .Tap(() => ShowInfo(message));
    }

    private void ShowInfo(string message)
    {
        var resultVM = new ResultVM(this, null, message);
        DialogPage = new ResultDialog(resultVM);
    }

    [RelayCommand]
    private void AddNewColumn()
    {
        var requestNewColumnVM = new SelectNewColumnViewModel(
            this,
            (parameters) => AddNewColumns(parameters),
            _familiesService.GetFamilyParameters()
        );

        DialogPage = new SelectNewColumnPage(requestNewColumnVM);
    }

    private void AddNewColumns(IEnumerable<FamilyParameterModel> parameters)
    {
        parameters
            .Where(fp => fp.IsSelected)
            .ForEach(fp => SizeTableInfo?.AddHeader(fp.FamilyParameter));
        var tmp = SizeTableInfo;
        SizeTableInfo = null;
        SizeTableInfo = tmp;
    }

    [RelayCommand]
    private void GotoTelegram() => Process.Start(Settings.Default.TelegramUrl);

    [RelayCommand]
    private void GoToVacation() => Process.Start(Settings.Default.Vacation);

    #endregion
}
