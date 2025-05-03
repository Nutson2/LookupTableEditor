using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;

namespace LookupTableEditor.Services;

public class FamiliesService
{
    private readonly Document _doc;
    private readonly FamilyManager _familyManager;
    private readonly AbstractParameterTypesProvider _parameterTypesProvider;

    public FamiliesService(Document doc, AbstractParameterTypesProvider parameterTypesProvider)
    {
        _doc = doc ?? throw new ArgumentException(nameof(doc));
        _parameterTypesProvider = parameterTypesProvider;
        _familyManager = _doc.FamilyManager;
        CheckFamilyType();
    }

    private void CheckFamilyType()
    {
        FamilyType? currentType = _familyManager.CurrentType;
        FamilyTypeSet? types = _familyManager.Types;

        if (!types.IsEmpty && currentType.Name != " ")
            return;

        _doc.Run(
            "Создание типоразмера",
            () => _familyManager.CurrentType = _familyManager.NewType(_doc.Title)
        );
    }

    public List<FamilyParameterModel> GetFamilyParameters()
    {
        var res = _doc
            .FamilyManager.Parameters.Cast<FamilyParameter>()
            .Select(fp => new FamilyParameterModel(
                fp,
                _parameterTypesProvider.FromFamilyParameter(fp)
            ))
            .ToList();

        res.ForEach(m =>
        {
            var val = GetValueAsString(m.FamilyParameter);
            m.Value = val;
        });
        return res;
    }

    private string GetValueAsString(FamilyParameter parameter)
    {
        FamilyType? famType = _familyManager.CurrentType;
        return parameter.StorageType switch
        {
            StorageType.String => famType.AsString(parameter),
            _ => famType.AsValueString(parameter),
        };
    }
}
