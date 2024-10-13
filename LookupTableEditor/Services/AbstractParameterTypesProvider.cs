using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.Services
{
    public class AbstractParameterTypesProvider
    {
        public AbstractParameterType Empty() => new(null);

#if R22_OR_GREATER

        public List<AbstractParameterType> GetAllTypes()
        {
            var type = typeof(SpecTypeId);
            var typesProps = type.GetProperties().ToList();
            typesProps.AddRange(type.GetNestedTypes().SelectMany(t => t.GetProperties()));
            return typesProps
                .Where(p => p.Name != nameof(SpecTypeId.Custom))
                .Select(p => new AbstractParameterType((ForgeTypeId)p.GetValue(null)))
                .ToList();
        }

        public AbstractParameterType FromDefinitionOfParameterType(DefinitionOfParameterType def)
        {
            var param = new AbstractParameterType(new ForgeTypeId(def.TypeName));
            param.SizeTablesTypeName = def.SizeTableType;
            return param;
        }

        public AbstractParameterType FromSizeTableColumn(FamilySizeTableColumn column) =>
            FromForgeType(column.GetSpecTypeId());

        public AbstractParameterType FromFamilyParameter(FamilyParameter parameter) =>
            FromForgeType(parameter.Definition.GetDataType());

        public AbstractParameterType FromParameter(Parameter parameter) =>
            FromForgeType(parameter.Definition.GetDataType());

        private AbstractParameterType FromForgeType(ForgeTypeId type) =>
            type.TypeId.IsValid()
                ? new AbstractParameterType(type)
                : new AbstractParameterType(SpecTypeId.String.Text);
#else

        public List<AbstractParameterType> GetAllTypes()
        {
            var type = typeof(UnitType);
            return Enum.GetValues(type)
                .Cast<UnitType>()
                .Select(p => new AbstractParameterType(p))
                .ToList();
        }

        public AbstractParameterType FromDefinitionOfParameterType(DefinitionOfParameterType def)
        {
            Enum.TryParse<UnitType>(def.TypeName, out var type);
            var param = new AbstractParameterType(type);
            param.SizeTablesTypeName = def.SizeTableType;
            return param;
        }

        public AbstractParameterType FromSizeTableColumn(FamilySizeTableColumn column) =>
            new AbstractParameterType(column.UnitType);

        public AbstractParameterType FromParameter(Parameter parameter) =>
            new AbstractParameterType(parameter.Definition.UnitType);

        public AbstractParameterType FromFamilyParameter(FamilyParameter parameter) =>
            new AbstractParameterType(parameter.Definition.UnitType);

#endif
    }
}
