using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LookupTableEditor.ViewModels;

public abstract partial class BaseDialogVM<T> : ErrorsViewModel, INotifyDataErrorInfo
{
    private BaseViewModel _ownerVM;
    private readonly Action<T?> _action;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private T? _requestedValue;

    partial void OnRequestedValueChanged(T? value) => ValidateRequestedProp(value);

    public abstract void ValidateRequestedProp(T? value);

    public BaseDialogVM(BaseViewModel ownerVM, Action<T?> action)
    {
        _ownerVM = ownerVM;
        _action = action;
    }

    private bool CanExecuteOk() => !HasErrors;

    [RelayCommand(CanExecute = nameof(CanExecuteOk))]
    private void Ok()
    {
        _ownerVM.DialogPage = null;
        _action.Invoke(RequestedValue);
    }

    [RelayCommand]
    private void Cancel() => _ownerVM.DialogPage = null;
}
