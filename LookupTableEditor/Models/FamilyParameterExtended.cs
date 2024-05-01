using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LookupTableEditor.Models
{
    public partial class FamilyParameterExtended : ObservableObject
    {
        public FamilyParameter FamilyParameter { get; }
        public string Name => FamilyParameter.Definition.Name;
        public string StorageType => FamilyParameter.StorageType.ToString();
        public string GroupName =>
            LabelUtils.GetLabelFor(FamilyParameter.Definition.ParameterGroup);
        public string IsInstance => FamilyParameter.IsInstance ? "Instanse" : "Type";

        [ObservableProperty]
        private bool _isSelected;

        public FamilyParameterExtended(FamilyParameter familyParameter)
        {
            FamilyParameter = familyParameter;
        }
    }
}
