using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LookupTableEditor.Services;
using LookupTableEditor.ViewModels;
using LookupTableEditor.Views;

namespace LookupTableEditor.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class LookupTableEditorCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements
        )
        {
            UIApplication uiApp = commandData.Application;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            if (!doc.IsFamilyDocument)
                return Result.Cancelled;

            var familiesService = new FamiliesService(doc);
            var sizeTableService = new SizeTableService(doc, uiApp.Application);
            var mainVM = new MainViewModel(sizeTableService, familiesService);
            var mainView = new MainWindow(mainVM);
            mainView.ShowDialog();

            return Result.Succeeded;
        }
    }
}
