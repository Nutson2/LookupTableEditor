using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LookupTableEditor
{
    public partial class AbstractParameterType : ObservableObject
    {
        [ObservableProperty]
        private string? _sizeTablesTypeName = string.Empty;

#if R22_OR_GREATER
        public ForgeTypeId? ParameterType { get; }
        public string Label
        {
            get
            {
                var discpline = UnitUtils.IsMeasurableSpec(ParameterType)
                    ? LabelUtils.GetLabelForDiscipline(UnitUtils.GetDiscipline(ParameterType))
                        + " : "
                    : string.Empty;
                return discpline + LabelUtils.GetLabelForSpec(ParameterType);
            }
        }

        public AbstractParameterType(ForgeTypeId? parameterType)
        {
            ParameterType = parameterType;
        }

        public override bool Equals(object obj)
        {
            return obj is null ? false : ToString() == obj?.ToString();
        }

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString() =>
            ParameterType?.TypeId.Split('-').First() ?? string.Empty;

#else

        public UnitType? UnitType { get; }

        public AbstractParameterType(UnitType? unitType)
        {
            UnitType = unitType;
        }

        public string Label =>
            UnitType.HasValue ? LabelUtils.GetLabelFor((ParameterType)UnitType) : " - ";

        public override string ToString() => UnitType.HasValue ? UnitType.ToString() : string.Empty;

#endif
    }
}
