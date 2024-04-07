using LookupTableEditor.Commands;
using Nice3point.Revit.Toolkit.External;

namespace LookupTableEditor
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            var panel = Application.CreatePanel("Commands", "LookupTableEditor");

            var tableEditorButton = panel.AddPushButton<LookupTableEditorCommand>(
                nameof(LookupTableEditorCommand.Execute)
            );
            tableEditorButton.SetImage(
                $"/{nameof(LookupTableEditor)};component/Resources/Icons/{nameof(LookupTableEditorCommand)}16.png"
            );
            tableEditorButton.SetLargeImage(
                $"/{nameof(LookupTableEditor)};component/Resources/Icons/{nameof(LookupTableEditorCommand)}32.png"
            );
        }
    }
}
