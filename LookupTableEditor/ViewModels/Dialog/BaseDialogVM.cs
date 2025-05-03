using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LookupTableEditor.ViewModels;

public abstract partial class BaseDialogVM<T> : BaseViewModel, INotifyDataErrorInfo
{
	private BaseViewModel _ownerVM;
	private readonly Action<T?>? _action;

	private T? requestVal;
	public T? RequestVal
	{
		get => requestVal;
		set
		{
			requestVal = value;
			ValidateRequestedProp(value);
			OnPropertyChanged();
			OkCommand.NotifyCanExecuteChanged();
		}
	}

	public abstract void ValidateRequestedProp(T? value);

	public BaseDialogVM(BaseViewModel ownerVM, Action<T?>? action)
	{
		_ownerVM = ownerVM;
		_action = action;
	}

	private bool CanExecuteOk() => !HasErrors;

	[RelayCommand(CanExecute = nameof(CanExecuteOk))]
	private void Ok()
	{
		_ownerVM.DialogPage = null;
		_action?.Invoke(RequestVal);
	}

	[RelayCommand]
	private void Cancel() => _ownerVM.DialogPage = null;
}
