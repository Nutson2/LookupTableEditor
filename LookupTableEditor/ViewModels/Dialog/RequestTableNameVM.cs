using System;
using System.IO;
using System.Linq;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.ViewModels.Dialog;

public partial class RequestTableNameVM : BaseDialogVM<string>
{
	public RequestTableNameVM(BaseViewModel ownerVM, Action<string?> action)
		: base(ownerVM, action)
	{
		RequestVal = string.Empty;
	}

	public override void ValidateRequestedProp(string? value) =>
		Validate(
			() =>
			{
				if (value?.IsValid() == false)
				{
					return "Не может быть пустым.";
				}
				var invalidChar = Path.GetInvalidFileNameChars().Where(c => value.Contains(c));
				if (invalidChar.Any())
				{
					return $"Наименование таблицы не может содержать символы: {string.Join(", ", invalidChar)}";
				}
				return null;
			},
			nameof(RequestVal)
		);
}
