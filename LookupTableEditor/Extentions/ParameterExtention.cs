using System;
using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions
{
    public static class ParameterExtention
    {
#if R22_OR_GREATER


        public static string GetParameterTypeLabel(this Definition definition) =>
            definition.GetDataType().ToSpecLabel();

        public static Type GetTypeForDataTable(this Definition definition) =>
            definition.GetDataType() == SpecTypeId.String.Text ? typeof(string) : typeof(double);
#else

        public static string GetParameterTypeLabel(this Parameter parameter) =>
            parameter.Definition.ParameterType.ToString();

        public static Type GetTypeForDataTable(this Definition definition) =>
            definition.ParameterType == ParameterType.Text ? typeof(string) : typeof(double);
#endif

#if R24_OR_GREATER
        public static string GetGroupName(this Definition definition)
        {
            return LabelUtils.GetLabelForGroup(definition.GetGroupTypeId());
        }
#elif R20_OR_GREATER
        public static string GetGroupName(this Definition definition)
        {
            return LabelUtils.GetLabelFor(definition.ParameterGroup);
        }
#endif
    }
}
