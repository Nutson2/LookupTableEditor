using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LookupTableEditor
{
    public class DefinitionOfParameterType
    {
        public string TypeName { get; set; }
        public string SizeTableType { get; set; }

        public DefinitionOfParameterType() { }

        public DefinitionOfParameterType(string typeName, string sizeTableType)
        {
            TypeName = typeName;
            SizeTableType = sizeTableType;
        }
    }

    public partial class AbstractParameterType : ObservableObject
    {
        public static AbstractParameterType Empty() => new(null);

        [ObservableProperty]
        private string? _sizeTablesTypeName = string.Empty;

#if R22_OR_GREATER
        public static AbstractParameterType FromDefinitionOfParameterType(
            DefinitionOfParameterType def
        )
        {
            var param = new AbstractParameterType(new ForgeTypeId(def.TypeName));
            param.SizeTablesTypeName = def.SizeTableType;
            return param;
        }

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

        public override bool Equals(object obj) => ToString().Equals(obj.ToString());

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString() =>
            ParameterType != null ? ParameterType.TypeId.Split('-').First() : string.Empty;
#else
        public static AbstractParameterType FromDefinitionOfParameterType(
            DefinitionOfParameterType def
        )
        {
            Enum.TryParse<ParameterType>(def.TypeName, out var type);
            var param = new AbstractParameterType(type);
            param.SizeTablesTypeName = def.SizeTableType;
            return param;
        }

        public static List<AbstractParameterType> GetAllTypes()
        {
            var type = typeof(ParameterType);
            return Enum.GetValues(type)
                .Cast<ParameterType>()
                .Select(p => new AbstractParameterType(p))
                .ToList();
        }

        public ParameterType? ParameterType { get; }

        public AbstractParameterType(ParameterType? parameterType)
        {
            ParameterType = parameterType;
        }

        public string Label =>
            ParameterType.HasValue ? LabelUtils.GetLabelFor((ParameterType)ParameterType) : " - ";

        public override string ToString() =>
            ParameterType.HasValue ? ParameterType.ToString() : string.Empty;

#endif
    }
}
