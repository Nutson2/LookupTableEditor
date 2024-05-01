using System.Xml.Serialization;
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

            var xmlSerializer = new XmlSerializer(typeof(List<DefinitionOfParameterType>));
            #region MyRegion
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

            #endregion

            return Result.Succeeded;
        }
    }
}
