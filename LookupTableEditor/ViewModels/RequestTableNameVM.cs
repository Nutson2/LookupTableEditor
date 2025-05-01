using System;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.ViewModels;

public partial class RequestTableNameVM : BaseDialogVM<string>
{
    public RequestTableNameVM(BaseViewModel ownerVM, Action<string?> action)
        : base(ownerVM, action) { }

    public override void ValidateRequestedProp(string? value) =>
        Validate(() => value?.IsValid() == true, "Не может быть пустым.", nameof(RequestedValue));
}
