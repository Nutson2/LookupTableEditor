﻿#if DEBUG && R22_OR_GREATER

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
            var doc = commandData.Application.ActiveUIDocument.Document;

            doc.Run(
                "Add param",
                () =>
                {
                    var group = ParameterUtils.GetParameterGroupTypeId(
                        BuiltInParameterGroup.PG_TEXT
                    );
                    foreach (var item in AbstractParameterType.GetAllTypes())
                    {
                        try
                        {
                            var name = item.ToString().Replace(':', '_');

                            doc.FamilyManager.AddParameter(name, group, item.ParameterType, true);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            );

            return Result.Succeeded;
        }
    }
}
#endif