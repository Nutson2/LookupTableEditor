using System;
using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions
{
    public static class FamilySizeTableColumnExtention
    {
#if R22_OR_GREATER
        public static string GetUnitTypeString(this FamilySizeTableColumn column) =>
            column.GetSpecTypeId().ToString();

        public static Type GetTypeForDataTable(this FamilySizeTableColumn column) =>
            column.GetSpecTypeId().TypeId.IsValid()
                ? Type.GetType("System.Double")
                : Type.GetType("System.String");

#else
        public static string GetUnitTypeString(this FamilySizeTableColumn column) =>
            column.UnitType.ToString();

        public static Type GetTypeForDataTable(this FamilySizeTableColumn column) =>
            column.DisplayUnitType == DisplayUnitType.DUT_UNDEFINED
                ? Type.GetType("System.String")
                : Type.GetType("System.Double");

#endif
    }
}
