using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LookupTableEditor.Models
{
    public partial class FamilyParameterModel : ObservableObject
    {
        public FamilyParameter FamilyParameter { get; }
        public string Name => FamilyParameter.Definition.Name;
        public string StorageType => FamilyParameter.StorageType.ToString();
        public string Value { get; set; } = string.Empty;
        public string Formula => FamilyParameter.Formula;
        public AbstractParameterType ParameterType { get; }
        public string GroupName =>
            LabelUtils.GetLabelFor(FamilyParameter.Definition.ParameterGroup);
        public string IsInstance =>
            FamilyParameter.IsInstance ? "Параметры экземляра" : "Параметры типа";

        [ObservableProperty]
        private bool _isSelected;

        public FamilyParameterModel(
            FamilyParameter familyParameter,
            AbstractParameterType parameterType
        )
        {
            FamilyParameter = familyParameter;
            ParameterType = parameterType;
        }
    }
}
