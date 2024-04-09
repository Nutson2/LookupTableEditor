using Autodesk.Revit.DB;
using LookupTableEditor.Services;

namespace LookupTableEditor.ViewModels
{
    public class SelectNewColumnViewModel
    {
        private readonly SizeTableInfo sizeTableInfo;
        private readonly FamiliesService _familiesService;

        public List<FamilyParameter> Parameters { get; set; }

        public SelectNewColumnViewModel(
            SizeTableInfo sizeTableInfo,
            FamiliesService familiesService
        )
        {
            this.sizeTableInfo = sizeTableInfo;
            _familiesService = familiesService;
            Parameters = _familiesService.GetFamilyParameters().ToList();
        }
    }
}
