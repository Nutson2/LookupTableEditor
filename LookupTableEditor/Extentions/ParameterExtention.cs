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

        public static AbstractParameterType GetParameterType(this FamilyParameter parameter)
        {
            return parameter.Definition.GetDataType().TypeId.IsValid()
                ? new AbstractParameterType(parameter.Definition.GetDataType())
                : new AbstractParameterType(SpecTypeId.String.Text);
        }

        public static string GetParameterTypeLabel(this Definition definition) =>
            definition.GetDataType().ToSpecLabel();

        public static Type GetTypeForDataTable(this Definition definition)
        {
            Type ColumnType =
                definition.GetDataType() == SpecTypeId.String.Text
                    ? Type.GetType("System.String")
                    : Type.GetType("System.Double");
            return ColumnType;
        }

#else
        public static UnitType GetParameterType(this Parameter parameter) =>
            parameter.Definition.UnitType;

        public static UnitType GetParameterType(this FamilyParameter parameter) =>
            parameter.GetParameter();

        public static string GetParameterTypeLabel(this Parameter parameter) =>
            parameter.Definition.ParameterType.ToString();

        public static Type GetTypeForDataTable(this Definition definition)
        {
            Type ColumnType =
                definition.ParameterType == ParameterType.Text
                    ? Type.GetType("System.String")
                    : Type.GetType("System.Double");
            return ColumnType;
        }
#endif
    }
}
