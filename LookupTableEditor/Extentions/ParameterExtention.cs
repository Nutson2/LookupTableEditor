using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions
{
    public static class ParameterExtention
    {
#if R22_OR_GREATER
        public static AbstractParameterType GetParameterType(this Parameter parameter)
        {
            return parameter.GetTypeId().TypeId.IsValid()
                ? new AbstractParameterType(parameter.GetTypeId())
                : new AbstractParameterType(SpecTypeId.String.Text);
        }

        public static AbstractParameterType GetParameterType(this FamilyParameter parameter) =>
            parameter.GetParameterType();
#else
        public static UnitType GetParameterType(this Parameter parameter) =>
            parameter.Definition.UnitType;

        public static UnitType GetParameterType(this FamilyParameter parameter) =>
            parameter.GetParameter();
#endif

        public static string GetParameterTypeLabel(this Parameter parameter)
        {
#if R22_OR_GREATER
            var ut = parameter.Definition.GetDataType().ToSpecLabel();
#else
            var ut = parameter.Definition.ParameterType.ToString();
#endif
            return ut;
        }

        public static string GetParameterTypeLabel(this FamilyParameter parameter) =>
            parameter.GetParameterTypeLabel();

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
