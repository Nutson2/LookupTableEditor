using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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

            var dict =
                ParametersUnitType.GetDictionaryToConvertParamTypeInSizeTableHeaderString2023();

            var pair = dict.First();
            var fType = new ForgeTypeId(pair.Key);
            var absParam = new AbstractParameterType(fType);
            var label = absParam.Label;

            //doc.Run(
            //    "Add param",
            //    () =>
            //    {
            //        var group = ParameterUtils.GetParameterGroupTypeId(
            //            BuiltInParameterGroup.PG_TEXT
            //        );
            //        foreach (var item in AbstractParameterType.GetAllTypes())
            //        {
            //            try
            //            {
            //                var name = item.ToString().Replace(':', '_');

            //                doc.FamilyManager.AddParameter(name, group, item.ParameterType, true);
            //            }
            //            catch (Exception ex)
            //            {
            //                var m = ex.Message;
            //            }
            //        }
            //    }
            //);

            return Result.Succeeded;
        }
    }
}
