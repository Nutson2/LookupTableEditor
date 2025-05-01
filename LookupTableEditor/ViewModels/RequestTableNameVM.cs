using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.ViewModels;

public partial class RequestTableNameVM : ErrorsViewModel
{
    private readonly Action<string> _action;

    [ObservableProperty]
    private string _tableName = string.Empty;

    partial void OnTableNameChanged(string value) =>
        Validate(() => value.IsValid(), "Не может быть пустым.", nameof(TableName));

    public RequestTableNameVM(Action<string> action)
    {
        _action = action;
    }

    private bool CanExecuteOk() => HasErrors;

    [RelayCommand(CanExecute = nameof(CanExecuteOk))]
    private void Ok()
    {
        _action.Invoke(TableName);
    }
}
