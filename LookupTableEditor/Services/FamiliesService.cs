﻿using Autodesk.Revit.DB;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.Services
{
    public class FamiliesService
    {
        private readonly Document _doc;
        private readonly FamilyManager _familyManager;

        public FamiliesService(Document doc)
        {
            _doc = doc.ThrowIfNull();
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
                () =>
                {
                    FamilyType currentType2 = _familyManager.NewType(_doc.Title);
                    _familyManager.CurrentType = currentType2;
                }
            );
        }

        public IEnumerable<FamilyParameter> GetFamilyParameters() =>
            _doc.FamilyManager.Parameters.Cast<FamilyParameter>();

        public string GetValueAsString(FamilyParameter parameter)
        {
            var famType = _familyManager.CurrentType;
            return parameter.StorageType switch
            {
                StorageType.String => famType.AsString(parameter),
                _ => famType.AsValueString(parameter)
            };
        }
    }
}
