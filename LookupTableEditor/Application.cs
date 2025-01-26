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
        private const string name = nameof(LookupTableEditor);

        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            Application
                .CreatePanel("Commands", name)
                .AddPushButton<LookupTableEditorCommand>(name)
                .SetImage($"/{name};component/Resources/Icons/{name}16.png")
                .SetLargeImage($"/{name};component/Resources/Icons/{name}32.png");
        }
    }
}
