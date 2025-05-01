using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.ViewModels;

public partial class RequestTableNameVM : ErrorsViewModel, INotifyDataErrorInfo
{
    private BaseViewModel _ownerVM;
    private readonly Action<string> _action;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string _tableName = null!;

    partial void OnTableNameChanged(string value) =>
        Validate(() => value.IsValid(), "Не может быть пустым.", nameof(TableName));

    public RequestTableNameVM(BaseViewModel ownerVM, Action<string> action)
    {
        _ownerVM = ownerVM;
        _action = action;

        TableName = string.Empty;
    }

    private bool CanExecuteOk() => !HasErrors;

    [RelayCommand(CanExecute = nameof(CanExecuteOk))]
    private void Ok()
    {
        _ownerVM.DialogPage = null;
        _action.Invoke(TableName);
    }

    [RelayCommand]
    private void Cancel() => _ownerVM.DialogPage = null;
}
