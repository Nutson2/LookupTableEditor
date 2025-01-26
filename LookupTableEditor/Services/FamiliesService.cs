using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;
using LookupTableEditor.Models;

namespace LookupTableEditor.Services
{
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
            var currentType = _familyManager.CurrentType;
            var types = _familyManager.Types;

            if (!types.IsEmpty && currentType.Name != " ")
                return;

            _doc.Run(
                "Создание типоразмера",
                () => _familyManager.CurrentType = _familyManager.NewType(_doc.Title)
            );
        }

        public IEnumerable<FamilyParameterModel> GetFamilyParameters()
        {
            return _doc
                .FamilyManager.Parameters.Cast<FamilyParameter>()
                .Select(fp => new FamilyParameterModel(
                    fp,
                    _parameterTypesProvider.FromFamilyParameter(fp)
                ))
                .ForEach(m => m.Value = GetValueAsString(m.FamilyParameter));
        }

        private string GetValueAsString(FamilyParameter parameter)
        {
            var famType = _familyManager.CurrentType;
            return parameter.StorageType switch
            {
                StorageType.String => famType.AsString(parameter),
                _ => famType.AsValueString(parameter),
            };
        }
    }
}
