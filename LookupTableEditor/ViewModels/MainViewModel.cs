using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public int SelectedColumnIndex
    {
        get => _selectedColumnIndex;
        set
        {
            if (SizeTableInfo is null)
                return;
            _selectedColumnIndex = value;
        }
    }

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

    public void PasteFromClipboard()
    {
        if (SizeTableInfo?.Table is null || !Clipboard.ContainsText() || SelectedRowIndex is null)
            return;

        int rowIndex = SelectedRowIndex.Value;
        int columnIndex = SelectedColumnIndex;

        SizeTableInfo.PasteFromClipboard(rowIndex, columnIndex);
    }

    #region Handlers

    partial void OnCurTableNameChanged(string? value)
    {
        if (!value.IsValid())
        {
            return;
        }

        SizeTableInfo = _sizeTableService.GetSizeTableInfo(value!);
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void CreateNewTable()
    {
        var dialogVM = new RequestTableNameVM(
            this,
            (curTableName) =>
            {
                if (curTableName is null)
                    return;
                SizeTableNames.Add(curTableName);
                CurTableName = curTableName;
            }
        );
        DialogPage = new RequestTableName(dialogVM);
    }

    [RelayCommand]
    private void SaveSizeTable()
    {
        if (SizeTableInfo is null)
            return;
        _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
        if (SizeTableInfo.FilePath is null)
            return;
        new Process
        {
            StartInfo = new ProcessStartInfo(SizeTableInfo.FilePath) { UseShellExecute = true },
        }.Start();
    }

    [RelayCommand]
    private void AddRowOnTop()
    {
        if (SelectedRowIndex is null)
            return;
        SizeTableInfo?.Table.Rows.InsertAt(SizeTableInfo.Table.NewRow(), SelectedRowIndex.Value);
    }

    [RelayCommand]
    private void UpdateTable()
    {
        var res = _sizeTableService.Update(SizeTableInfo!);
        string message = res.Item2 is not null ? res.Item2 : "Таблица выбора успешно загружена.";
        if (res.Item1 is not null)
            SizeTableInfo = res.Item1;

        var resultVM = new ResultVM(this, null, message);
        DialogPage = new ResultDialog(resultVM);
    }

    [RelayCommand]
    private void AddNewColumn()
    {
        var requestNewColumnVM = new SelectNewColumnViewModel(
            this,
            (parameters) =>
            {
                parameters
                    ?.Where(fp => fp.IsSelected)
                    .ForEach(fp => SizeTableInfo?.AddHeader(fp.FamilyParameter));

                var tmp = SizeTableInfo;
                SizeTableInfo = null;
                SizeTableInfo = tmp;
            },
            _familiesService.GetFamilyParameters()
        );

        DialogPage = new SelectNewColumnPage(requestNewColumnVM);
    }

    [RelayCommand]
    private void GotoTelegram() => Process.Start(Settings.Default.TelegramUrl);

    [RelayCommand]
    private void GoToVacation() => Process.Start(Settings.Default.Vacation);

    #endregion
}
