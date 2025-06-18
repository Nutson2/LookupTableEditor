using System;

namespace LookupTableEditor.ViewModels.Dialog;

public partial class ResultVM : BaseDialogVM<string>
{
    public ResultVM(BaseViewModel ownerVM, Action<string>? action, string message)
        : base(ownerVM, action)
    {
        RequestVal = message;
    }

    public override void ValidateRequestedProp(string? value) { }
}
