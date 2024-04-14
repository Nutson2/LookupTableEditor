using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using LookupTableEditor.Services;

namespace LookupTableEditor.ViewModels
{
    public class SelectNewColumnViewModel
    {
        private readonly SizeTableInfo sizeTableInfo;
        private readonly FamiliesService _familiesService;

        public FamParameters Parameters { get; set; } = new FamParameters();

        public SelectNewColumnViewModel(
            SizeTableInfo sizeTableInfo,
            FamiliesService familiesService
        )
        {
            this.sizeTableInfo = sizeTableInfo;
            _familiesService = familiesService;
            _familiesService.GetFamilyParameters().ToList().ForEach(x => Parameters.Add(x));
        }
    }

    public class FamParameters : ObservableCollection<FamilyParameter> { }
}
