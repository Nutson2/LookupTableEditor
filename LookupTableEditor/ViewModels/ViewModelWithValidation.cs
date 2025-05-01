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

    public IEnumerable? GetErrors(string propertyName) =>
        _propertyErrors.GetOrDefault(propertyName) ?? Enumerable.Empty<string>();

    private void AddError(string propertyName, string errorMessage)
    {
        _propertyErrors.TryAdd(propertyName, new());
        _propertyErrors[propertyName].Add(errorMessage);
        OnErrorsChanged(propertyName);
    }

    private void ClearErrors(string propertyName)
    {
        if (_propertyErrors.Remove(propertyName))
        {
            OnErrorsChanged(propertyName);
        }
    }

    private void OnErrorsChanged(string propertyName) =>
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

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
    }
}
