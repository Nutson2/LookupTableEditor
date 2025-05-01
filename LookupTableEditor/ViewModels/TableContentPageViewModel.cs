using System;
using System.Collections.Generic;
using System.Data;
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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTableNotExist))]
    private string? _curTableName;

    private int _selectedColumnIndex;

    [ObservableProperty]
    private string? _selectedColumnName;

    [ObservableProperty]
    private AbstractParameterType? _selectedColumnType;

    [ObservableProperty]
    private SizeTableInfo? _sizeTableInfo;

    [ObservableProperty]
    private List<string> _sizeTableNames = new();

    public TableContentViewModel(
        SizeTableService sizeTableService,
        AbstractParameterTypesProvider parameterTypesProvider
    )
    {
        _sizeTableService = sizeTableService;

        SelectedColumnType = parameterTypesProvider.Empty();

        SizeTableNames = _sizeTableService.Manager.GetAllSizeTableNames().ToList();
        CurTableName = SizeTableNames.FirstOrDefault();

        ParameterTypes = _sizeTableService
            .AbstractParameterTypes.Where(p => p.Label.IsValid())
            .OrderBy(p => p.Label)
            .ToList();
    }

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

    public List<AbstractParameterType> ParameterTypes { get; }

    public event Action? OnColumnNameChanged;
    public event Action<SizeTableInfo>? OnAddNewColumn;

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
                Type columnType = dataTable.Columns[cell.ColumnIndex].DataType;
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

        if (SizeTableInfo is null || _sizeTableService.Manager.HasSizeTable(value))
        {
            SizeTableInfo = _sizeTableService.GetSizeTableInfo(value!);
        }
        else
        {
            SizeTableInfo.Name = value;
        }
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
                SizeTableInfo = _sizeTableService.GetSizeTableInfo(curTableName);
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
    }

    [RelayCommand]
    private void AddRowOnTop()
    {
        if (SelectedRowIndex is null)
            return;
        AddRowOnTop(SelectedRowIndex.Value);
    }

    [RelayCommand]
    private void SetNewTable()
    {
        if (SizeTableInfo is null)
            return;
        _sizeTableService.SaveSizeTableOnTheDisk(SizeTableInfo);
        _sizeTableService.ImportSizeTable(SizeTableInfo);
        if (CurTableName != null)
            SizeTableInfo = _sizeTableService.GetSizeTableInfo(CurTableName);
    }

    [RelayCommand]
    private void AddNewColumn()
    {
        if (SizeTableInfo is null)
            return;
        OnAddNewColumn?.Invoke(SizeTableInfo);
    }

    #endregion
}
