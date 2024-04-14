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
                .Select(p => new AbstractParameterType((ForgeTypeId)p.GetValue(null)))
                .ToList();
        }

        public ForgeTypeId? ParameterType { get; }

        public AbstractParameterType(ForgeTypeId? parameterType)
        {
            ParameterType = parameterType;
        }

        public override string ToString() =>
            ParameterType != null ? ParameterType.TypeId.Split('-').First() : string.Empty;
#else
        public ParameterType? ParameterType { get; }

        public AbstractParameterType(ParameterType? parameterType)
        {
            ParameterType = parameterType;
        }

        public override string ToString() =>
            ParameterType != null ? ParameterType.ToString() : string.Empty;

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
