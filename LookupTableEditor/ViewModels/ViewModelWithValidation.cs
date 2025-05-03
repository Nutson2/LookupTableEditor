using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.ViewModels;

public class ErrorsViewModel : ObservableObject, INotifyDataErrorInfo
{
	private readonly Dictionary<string, List<string>> _propertyErrors = new();
	public bool HasErrors => _propertyErrors.Any();

	public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

	public IEnumerable GetErrors(string? propertyName)
	{
		if (propertyName is null)
			Enumerable.Empty<string>();

		return _propertyErrors.GetOrDefault(propertyName!) ?? Enumerable.Empty<string>();
	}

	private void AddError(string propertyName, string errorMessage)
	{
		_propertyErrors.TryAdd(propertyName, new());
		_propertyErrors[propertyName].Add(errorMessage);
	}

	private void ClearErrors(string propertyName) => _propertyErrors.Remove(propertyName);

	private void OnErrorsChanged(string propertyName)
	{
		OnPropertyChanged(nameof(HasErrors));
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}

	public void Validate(
		Func<bool> predicat,
		string warningMessage,
		[CallerMemberName] string propertyName = ""
	)
	{
		ClearErrors(propertyName);
		if (!predicat.Invoke())
		{
			AddError(propertyName, warningMessage);
		}
		OnErrorsChanged(propertyName);
	}

	public void Validate(Func<string?> predicat, [CallerMemberName] string propertyName = "")
	{
		ClearErrors(propertyName);
		var msg = predicat.Invoke();
		if (msg is not null)
		{
			AddError(propertyName, msg);
		}
		OnErrorsChanged(propertyName);
	}
}
