namespace LookupTableEditor.Models
{
    public class DefinitionOfParameterType
    {
        public string TypeName { get; }
        public string SizeTableType { get; }

        public DefinitionOfParameterType(string typeName, string sizeTableType)
        {
            TypeName = typeName;
            SizeTableType = sizeTableType;
        }
    }
}
