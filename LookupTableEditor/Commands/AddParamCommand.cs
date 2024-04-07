using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.Commands
{
    [Transaction(TransactionMode.Manual)]
    class AddParamCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements
        )
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            doc.Run(
                "Add param",
                () => {
                    //var type = typeof(SpecTypeId);
                    //var typesProps = type.GetProperties().ToList();
                    //typesProps.AddRange(type.GetNestedTypes().SelectMany(t => t.GetProperties()));

                    //var group = ParameterUtils.GetParameterGroupTypeId(
                    //    BuiltInParameterGroup.PG_TEXT
                    //);
                    //foreach (var item in typesProps)
                    //{
                    //    try
                    //    {
                    //        ForgeTypeId specTypeId = (ForgeTypeId)item.GetValue(null);

                    //        doc.FamilyManager.AddParameter(item.Name, group, specTypeId, true);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        var m = ex.Message;
                    //    }
                    //}
                }
            );

            return Result.Succeeded;
        }
    }
}
