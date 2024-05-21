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
}
