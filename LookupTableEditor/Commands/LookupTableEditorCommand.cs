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

            var parameterTypesProvider = new AbstractParameterTypesProvider();

            var familiesService = new FamiliesService(doc, parameterTypesProvider);
            var sizeTableService = new SizeTableService(
                doc,
                uiApp.Application,
                parameterTypesProvider
            );
            var mainVM = new MainViewModel(
                sizeTableService,
                familiesService,
                parameterTypesProvider
            );
            var mainView = new MainWindow(mainVM);
            mainView.ShowDialog();

            return Result.Succeeded;
        }
    }
}
