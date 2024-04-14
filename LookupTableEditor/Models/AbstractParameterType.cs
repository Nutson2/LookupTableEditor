using Autodesk.Revit.DB;

namespace LookupTableEditor
{
    public class AbstractParameterType
    {
        public static AbstractParameterType Empty() => new(null);

#if R22_OR_GREATER
        public static List<AbstractParameterType> GetAllTypes()
        {
            var type = typeof(SpecTypeId);
            var typesProps = type.GetProperties().ToList();
            typesProps.AddRange(type.GetNestedTypes().SelectMany(t => t.GetProperties()));
            return typesProps
                .Where(p => p.Name != nameof(SpecTypeId.Custom))
                .Select(p => new AbstractParameterType((ForgeTypeId)p.GetValue(null)))
                .ToList();
        }

        public ForgeTypeId? ParameterType { get; }

        public AbstractParameterType(ForgeTypeId? parameterType)
        {
            ParameterType = parameterType;
        }

        public string Label => GetLabel();

        private string GetLabel()
        {
            var discpline = UnitUtils.IsMeasurableSpec(ParameterType)
                ? LabelUtils.GetLabelForDiscipline(UnitUtils.GetDiscipline(ParameterType)) + " : "
                : string.Empty;
            var t = discpline + LabelUtils.GetLabelForSpec(ParameterType);

            return t;
        }

        public override bool Equals(object obj) => this.ToString().Equals(obj.ToString());

        public override int GetHashCode() => this.ToString().GetHashCode();

        public override string ToString() =>
            ParameterType != null
                ? ParameterType.TypeId.Split('-').First().Replace(':', '_')
                : string.Empty;
#else
        public ParameterType? ParameterType { get; }

        public AbstractParameterType(ParameterType? parameterType)
        {
            ParameterType = parameterType;
        }

        public string Label =>
            ParameterType.HasValue ? LabelUtils.GetLabelFor((ParameterType)ParameterType) : " - ";

        public override string ToString() =>
            ParameterType.HasValue ? ParameterType.ToString() : string.Empty;

        public static List<AbstractParameterType> GetAllTypes()
        {
            var type = typeof(ParameterType);
            return Enum.GetValues(type)
                .Cast<ParameterType>()
                .Select(p => new AbstractParameterType(p))
                .ToList();
        }

#endif
    }
}
