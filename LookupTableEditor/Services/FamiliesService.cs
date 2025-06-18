using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;

namespace LookupTableEditor.Services;

public class FamiliesService
{
    private const string createType = "Создание типоразмера";
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

        _familyManager.CurrentType = _doc.Run(createType, () => _familyManager.NewType(_doc.Title));
    }

    public List<FamilyParameterModel> GetFamilyParameters()
    {
        var res = _doc
            .FamilyManager.Parameters.OfType<FamilyParameter>()
            .Select(fp => (fp, type: _parameterTypesProvider.FromFamilyParameter(fp)))
            .Select(pair => new FamilyParameterModel(pair.fp, pair.type))
            .ForEach(m => m.Value = GetValueAsString(m.FamilyParameter));
        return res.ToList();
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
