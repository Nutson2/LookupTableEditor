using Autodesk.Revit.DB;
#if R22_OR_GREATER
using System.Linq;
#endif

namespace LookupTableEditor.Models;

public class AbstractParameterType
{
    public string SizeTablesTypeName { get; }

#if R22_OR_GREATER
    public ForgeTypeId? ParameterType { get; }
    public string Label
    {
        get
        {
            var discpline = UnitUtils.IsMeasurableSpec(ParameterType)
                ? LabelUtils.GetLabelForDiscipline(UnitUtils.GetDiscipline(ParameterType)) + " : "
                : string.Empty;
            return discpline + LabelUtils.GetLabelForSpec(ParameterType);
        }
    }

    public AbstractParameterType(ForgeTypeId? parameterType, string sizeTablesTypeName)
    {
        ParameterType = parameterType;
        SizeTablesTypeName = sizeTablesTypeName;
    }

    public AbstractParameterType(ForgeTypeId? parameterType)
        : this(parameterType, string.Empty) { }

    public override bool Equals(object? obj) => ToString() == obj?.ToString();

    public override int GetHashCode() => ToString().GetHashCode();

    public override string ToString() => ParameterType?.TypeId.Split('-').First() ?? string.Empty;

#else

    public UnitType? UnitType { get; }

    public AbstractParameterType(UnitType? unitType, string sizeTablesTypeName)
    {
        SizeTablesTypeName = sizeTablesTypeName;
        UnitType = unitType;
    }

    public AbstractParameterType(UnitType? unitType)
        : this(unitType, string.Empty) { }

    public string Label =>
        UnitType.HasValue ? LabelUtils.GetLabelFor((ParameterType)UnitType) : " - ";

    public override string ToString()
    {
        return UnitType.HasValue ? UnitType.ToString() : string.Empty;
    }

#endif
}
