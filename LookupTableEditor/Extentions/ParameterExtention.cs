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
            definition.GetDataType() == SpecTypeId.String.Text
                ? Type.GetType("System.String")
                : Type.GetType("System.Double");

#else

        public static string GetParameterTypeLabel(this Parameter parameter) =>
            parameter.Definition.ParameterType.ToString();

        public static Type GetTypeForDataTable(this Definition definition) =>
            definition.ParameterType == ParameterType.Text
                ? Type.GetType("System.String")
                : Type.GetType("System.Double");
#endif
    }
}
