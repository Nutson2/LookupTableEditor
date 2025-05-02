using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;
using LookupTableEditor.Services;
using LookupTableEditor.Views.Pages;

namespace LookupTableEditor.ViewModels;

public partial class TableContentViewModel : BaseViewModel
{
    private readonly SizeTableService _sizeTableService;
    private readonly FamiliesService _familiesService;
    private int _selectedColumnIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTableNotExist))]
    private string? _curTableName;

    [ObservableProperty]
    private string? _selectedColumnName;

    [ObservableProperty]
    private AbstractParameterType? _selectedColumnType;

    [ObservableProperty]
    private SizeTableInfo? _sizeTableInfo;

    public ObservableCollection<string> SizeTableNames { get; private set; }
    public List<AbstractParameterType> ParameterTypes { get; }

    public event Action? OnColumnNameChanged;
    public event Action<SizeTableInfo>? OnAddNewColumn;
    public int? SelectedRowIndex { get; set; }
    public bool IsTableNotExist =>
        CurTableName is not null && !SizeTableNames.Contains(CurTableName);
    public bool IsSizeTableInfoExist => SizeTableInfo is not null;

    public int SelectedColumnIndex
    {
        get => _selectedColumnIndex;
        set
        {
            if (SizeTableInfo is null)
                return;
            _selectedColumnIndex = value;
            SelectedColumnName = SizeTableInfo.Table.Columns[value].Caption;
            SelectedColumnType = SizeTableInfo.GetColumnType(SelectedColumnName);
            OnPropertyChanged(nameof(SelectedColumnType));
        }
    }

    public TableContentViewModel(
        SizeTableService sizeTableService,
        AbstractParameterTypesProvider parameterTypesProvider,
        FamiliesService familiesService
    )
    {
        _sizeTableService = sizeTableService;
        _familiesService = familiesService;
        SelectedColumnType = parameterTypesProvider.Empty();

        SizeTableNames = new(_sizeTableService.Manager.GetAllSizeTableNames().ToList());
        CurTableName = SizeTableNames.FirstOrDefault();

        ParameterTypes = _sizeTableService
            .AbstractParameterTypes.Where(p => p.Label.IsValid())
            .OrderBy(p => p.Label)
            .ToList();
    }

    private void AddRowOnTop(int index)
    {
        SizeTableInfo?.Table.Rows.InsertAt(SizeTableInfo.Table.NewRow(), index);
    }

    public void PasteFromClipboard()
    {
        if (SizeTableInfo?.Table is null || !Clipboard.ContainsText() || SelectedRowIndex is null)
            return;

        int rowIndex = SelectedRowIndex.Value;
        int columnIndex = SelectedColumnIndex;
        DataTable dataTable = SizeTableInfo.Table;

        string clipboardContent = Clipboard.GetText();

        var cells = clipboardContent.ParseAsCells();

        foreach (var cell in cells)
        {
            var curRowIndex = rowIndex + cell.RowIndex;
            if (curRowIndex >= dataTable.Rows.Count)
                dataTable.Rows.Add(dataTable.NewRow());

            var curColumnIndex = columnIndex + cell.ColumnIndex;
            if (curColumnIndex >= dataTable.Columns.Count)
                continue;

            try
            {
                Type columnType = dataTable.Columns[curColumnIndex].DataType;
                dataTable.Rows[curRowIndex][curColumnIndex] =
                    columnType == typeof(string) ? cell.Text : cell.Text.ToDouble();
            }
            catch
            {
                // ignored
            }
        }
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

    partial void OnSelectedColumnNameChanged(string? oldValue, string? newValue)
    {
        if (!oldValue.IsValid() || !newValue.IsValid() || SelectedColumnType is null)
            return;

        SizeTableInfo?.ChangeColumnName(
            SelectedColumnIndex,
            oldValue!,
            newValue!,
            SelectedColumnType
        );
        OnColumnNameChanged?.Invoke();
    }

    partial void OnSelectedColumnTypeChanged(AbstractParameterType? value)
    {
        if (SelectedColumnName is null || value is null)
            return;
        SizeTableInfo?.ChangeColumnType(SelectedColumnName, value);
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
        Process.Start(SizeTableInfo.FilePath);
    }

    [RelayCommand]
    private void AddRowOnTop()
    {
        if (SelectedRowIndex is null)
            return;
        AddRowOnTop(SelectedRowIndex.Value);
    }

    [RelayCommand]
    private void UpdateTable()
    {
        string message = "Таблица выбора успешно загружена.";
        try
        {
            if (SizeTableInfo is null)
                return;
            _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
            _sizeTableService.ImportSizeTable(SizeTableInfo);
            if (CurTableName != null)
                SizeTableInfo = _sizeTableService.GetSizeTableInfo(CurTableName);
        }
        catch (Exception ex)
        {
            message = $"Возникла проблема:\n{ex.Message}";
        }
        var resultVM = new ResultVM(this, null, message);
        DialogPage = new ResultDialog(resultVM);
    }

    [RelayCommand]
    private void AddNewColumn()
    {
        var parameters = _familiesService.GetFamilyParameters();
        var requestNewColumnVM = new SelectNewColumnViewModel(
            this,
            (parameters) =>
            {
                parameters
                    .Where(fp => fp.IsSelected)
                    .ForEach(fp => SizeTableInfo?.AddHeader(fp.FamilyParameter));

                var tmp = SizeTableInfo;
                SizeTableInfo = null;
                SizeTableInfo = tmp;
            },
            parameters
        );

        DialogPage = new SelectNewColumnPage(requestNewColumnVM);
    }

    #endregion
}
