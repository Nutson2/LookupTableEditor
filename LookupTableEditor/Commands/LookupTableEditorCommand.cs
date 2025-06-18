using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CSharpFunctionalExtensions;
using LookupTableEditor.Services;
using LookupTableEditor.ViewModels;
using LookupTableEditor.Views;
using FResult = CSharpFunctionalExtensions.Result;
using Result = Autodesk.Revit.UI.Result;

namespace LookupTableEditor.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
class LookupTableEditorCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiApp = commandData.Application;
        Document doc = commandData.Application.ActiveUIDocument.Document;
        MainWindow? mainView = default;

        var res = FResult
            .FailureIf(() => !doc.IsFamilyDocument, "Работает только в документе семейства")
            .Tap(() => PrepaireEncoding())
            .TapTry(() => mainView = ShowWindow(uiApp, doc), (ex) => ex.Message)
            .TapError((msg) => OnFailure(msg, mainView));

        return res.IsSuccess ? Result.Succeeded : Result.Cancelled;
    }

    private static void OnFailure(string msg, MainWindow? mainView)
    {
        mainView?.Close();
        TaskDialog.Show("Error", msg);
    }

    private static MainWindow ShowWindow(UIApplication uiApp, Document doc)
    {
        var parameterTypesProvider = new AbstractParameterTypesProvider();
        var familiesService = new FamiliesService(doc, parameterTypesProvider);
        var sizeTableService = new SizeTableService(doc, uiApp.Application, parameterTypesProvider);
        var mainVM = new MainViewModel(sizeTableService, parameterTypesProvider, familiesService);
        var mainView = new MainWindow(mainVM);
        mainView.ShowDialog();
        return mainView;
    }

    private void PrepaireEncoding()
    {
#if REVIT2025_OR_GREATER
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
    }
}
