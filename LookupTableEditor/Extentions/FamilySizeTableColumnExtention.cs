﻿using System;
using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions
{
    public static class FamilySizeTableColumnExtention
    {
#if R22_OR_GREATER
        public static string GetUnitTypeString(this FamilySizeTableColumn column) =>
            column.GetSpecTypeId().ToString();

        public static Type GetTypeForDataTable(this FamilySizeTableColumn column) =>
            column.GetSpecTypeId().TypeId.IsValid() ? typeof(string) : typeof(double);

#else
        public static string GetUnitTypeString(this FamilySizeTableColumn column) =>
            column.UnitType.ToString();

        public static Type GetTypeForDataTable(this FamilySizeTableColumn column) =>
            column.DisplayUnitType == DisplayUnitType.DUT_UNDEFINED
                ? typeof(string)
                : typeof(double);

#endif
    }
}
