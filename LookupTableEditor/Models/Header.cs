using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;

namespace LookupTableEditor
{
    public class Header
    {
        public string Name { get; set; } = string.Empty;
#if R22_OR_GREATER
        public ForgeTypeId Type { get; set; } = SpecTypeId.String.Text;
        public string TypeString => Name.IsValid() ? Type.ToSpecLabel() : string.Empty;
#else
        public UnitType Type { get; set; } = UnitType.UT_Undefined;
        public string TypeString => Name.IsValid() ? Type.ToString() : string.Empty;

#endif
    }
}
