using System;
using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions;

public static class FamilySizeTableColumnExtention
{
#if R22_OR_GREATER
    public static string GetUnitTypeString(this FamilySizeTableColumn column) =>
        column.GetSpecTypeId().ToString() ?? string.Empty;

    public static Type GetTypeForDataTable(this FamilySizeTableColumn column) =>
        column.GetSpecTypeId().TypeId.IsValid() ? typeof(double) : typeof(string);

#else
    public static string GetUnitTypeString(this FamilySizeTableColumn column)
    {
        return column.UnitType.ToString();
    }

    public static Type GetTypeForDataTable(this FamilySizeTableColumn column)
    {
        return column.DisplayUnitType == DisplayUnitType.DUT_UNDEFINED
            ? typeof(string)
            : typeof(double);
    }

#endif
}
