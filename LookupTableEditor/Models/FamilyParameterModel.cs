using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.Models;

public partial class FamilyParameterModel : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    public FamilyParameterModel(
        FamilyParameter familyParameter,
        AbstractParameterType parameterType
    )
    {
        FamilyParameter = familyParameter;
        ParameterType = parameterType;
        GroupName = FamilyParameter.Definition.GetGroupName();
    }

    public FamilyParameter FamilyParameter { get; }
    public string Name => FamilyParameter.Definition.Name;
    public string StorageType => FamilyParameter.StorageType.ToString();
    public string Value { get; set; } = string.Empty;
    public string Formula => FamilyParameter.Formula;
    public AbstractParameterType ParameterType { get; }
    public string GroupName { get; }

    public string IsInstance =>
        FamilyParameter.IsInstance ? "Параметры экземляра" : "Параметры типа";
}
