using LookupTableEditor.Commands;
using Nice3point.Revit.Extensions;
using Nice3point.Revit.Toolkit.External;

namespace LookupTableEditor;

/// <summary>
///     Application entry point
/// </summary>

public class Application : ExternalApplication
{
	private const string Name = nameof(LookupTableEditor);

	public override void OnStartup()
	{
		CreateRibbon();
	}

	private void CreateRibbon()
	{
		Application
			.CreatePanel("Commands", Name)
			.AddPushButton<LookupTableEditorCommand>(Name)
			.SetImage($"/{Name};component/Resources/Icons/{Name}16.png")
			.SetLargeImage($"/{Name};component/Resources/Icons/{Name}32.png");
	}
}
