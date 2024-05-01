using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions
{
    public static class FamilySizeTableColumnExtention
    {
        public static string GetUnitTypeString(this FamilySizeTableColumn column)
        {
#if R22_OR_GREATER
            var ut = column.GetSpecTypeId();
#else
            var ut = column.UnitType;
#endif
            return ut.ToString();
        }

        public static Type GetTypeForDataTable(this FamilySizeTableColumn column)
        {
#if R22_OR_GREATER
            var t = column.GetSpecTypeId().TypeId.IsValid()
                ? Type.GetType("System.Double")
                : Type.GetType("System.String");
#else
            var t =
                column.DisplayUnitType == DisplayUnitType.DUT_UNDEFINED
                    ? Type.GetType("System.String")
                    : Type.GetType("System.Double");
#endif
            return t;
        }

#if R22_OR_GREATER
        public static AbstractParameterType GetHeaderType(this FamilySizeTableColumn column) =>
            column.GetSpecTypeId().TypeId.IsValid()
                ? new AbstractParameterType(column.GetSpecTypeId())
                : new AbstractParameterType(SpecTypeId.String.Text);

#else
        public static AbstractParameterType GetHeaderType(this FamilySizeTableColumn column) =>
            column == null
                ? new AbstractParameterType(UnitType.UT_Undefined)
                : new AbstractParameterType(column.UnitType);
#endif
    }
}
