using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions
{
    public static class ParameterExtention
    {
        public static string GetParameterType(this Parameter parameter)
        {
#if R22_OR_GREATER
            var ut = parameter.Definition.GetDataType().ToSpecLabel();
#else
            var ut = parameter.Definition.ParameterType.ToString();
#endif
            return ut;
        }

        public static string GetParameterType(this FamilyParameter parameter) =>
            parameter.GetParameterType();

        public static Type GetTypeForDataTable(this Parameter parameter)
        {
#if R22_OR_GREATER
            Type ColumnType =
                parameter.Definition.GetDataType() == SpecTypeId.String.Text
                    ? Type.GetType("System.String")
                    : Type.GetType("System.Double");
#else
            Type ColumnType =
                parameter.Definition.ParameterType == ParameterType.Text
                    ? Type.GetType("System.String")
                    : Type.GetType("System.Double");
#endif
            return ColumnType;
        }

        public static Type GetTypeForDataTable(this FamilyParameter parameter) =>
            parameter.GetTypeForDataTable();
    }
}
