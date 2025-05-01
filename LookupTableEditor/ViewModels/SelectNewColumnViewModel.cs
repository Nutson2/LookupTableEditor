using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using LookupTableEditor.Models;

namespace LookupTableEditor.ViewModels;

public partial class SelectNewColumnViewModel : BaseDialogVM<IEnumerable<FamilyParameterModel>>
{
    private const ListSortDirection ASC = ListSortDirection.Ascending;

    public CollectionViewSource CollectionViewSource { get; set; }

    public SelectNewColumnViewModel(
        BaseViewModel ownerVM,
        Action<IEnumerable<FamilyParameterModel>?> action,
        IEnumerable<FamilyParameterModel> parameters
    )
        : base(ownerVM, action)
    {
        RequestVal = new ObservableCollection<FamilyParameterModel>(parameters);

        CollectionViewSource = new CollectionViewSource { Source = RequestVal };

        CollectionViewSource.GroupDescriptions.Add(
            new PropertyGroupDescription(nameof(FamilyParameterModel.GroupName))
        );
        CollectionViewSource.GroupDescriptions.Add(
            new PropertyGroupDescription(nameof(FamilyParameterModel.IsInstance))
        );
        CollectionViewSource.SortDescriptions.Add(
            new SortDescription(nameof(FamilyParameterModel.GroupName), ASC)
        );
        CollectionViewSource.SortDescriptions.Add(
            new SortDescription(nameof(FamilyParameterModel.StorageType), ASC)
        );
        CollectionViewSource.SortDescriptions.Add(
            new SortDescription(nameof(FamilyParameterModel.Name), ASC)
        );
    }

    public override void ValidateRequestedProp(IEnumerable<FamilyParameterModel>? value) { }
}
