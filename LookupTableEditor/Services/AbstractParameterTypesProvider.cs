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
            column.GetSpecTypeId().TypeId.IsValid()
                ? new AbstractParameterType(column.GetSpecTypeId())
                : new AbstractParameterType(SpecTypeId.String.Text);

        public AbstractParameterType FromFamilyParameter(FamilyParameter parameter) =>
            parameter.Definition.GetDataType().TypeId.IsValid()
                ? new AbstractParameterType(parameter.Definition.GetDataType())
                : new AbstractParameterType(SpecTypeId.String.Text);

        public AbstractParameterType FromParameter(Parameter parameter) =>
            parameter.Definition.GetDataType().TypeId.IsValid()
                ? new AbstractParameterType(parameter.Definition.GetDataType())
                : new AbstractParameterType(SpecTypeId.String.Text);

#else

        public static List<AbstractParameterType> GetAllTypes()
        {
            var type = typeof(UnitType);
            return Enum.GetValues(type)
                .Cast<UnitType>()
                .Select(p => new AbstractParameterType(p))
                .ToList();
        }

        public static AbstractParameterType FromDefinitionOfParameterType(
            DefinitionOfParameterType def
        )
        {
            Enum.TryParse<UnitType>(def.TypeName, out var type);
            var param = new AbstractParameterType(type);
            param.SizeTablesTypeName = def.SizeTableType;
            return param;
        }

        public AbstractParameterType FromSizeTableColumn(FamilySizeTableColumn column) =>
            column == null
                ? new AbstractParameterType(UnitType.UT_Undefined)
                : new AbstractParameterType(column.UnitType);

        public AbstractParameterType FromParameter(Parameter parameter) =>
            new AbstractParameterType(parameter.Definition.UnitType);

        public AbstractParameterType FromFamilyParameter(FamilyParameter parameter) =>
            new AbstractParameterType(parameter.Definition.UnitType);

#endif
    }
}
