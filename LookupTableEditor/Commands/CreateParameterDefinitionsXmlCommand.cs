#if DEBUG

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LookupTableEditor.Models;
using Microsoft.Win32;

namespace LookupTableEditor.Commands
{
    [Transaction(TransactionMode.Manual)]
    class CreateParameterDefinitionsXmlCommand : IExternalCommand
    {
        private const string NumberGeneralSizeTableType = "##NUMBER##GENERAL";
        private FileInfo? _definitionsFileInfo;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements
        )
        {
            var definitions = GetDefinitions();

            if (definitions.Count == 0)
                return Result.Cancelled;

            SerializeDefinitions(definitions, commandData.Application.Application.VersionNumber);

            return Result.Succeeded;
        }

        private void SerializeDefinitions(
            List<DefinitionOfParameterType> definitions,
            string versionNumber
        )
        {
            if (_definitionsFileInfo is null)
                return;
            var xmlFilePath = Path.Combine(
                _definitionsFileInfo.DirectoryName,
                $"ParametersTypes{versionNumber}.xml"
            );

            var xmlSerializer = new XmlSerializer(typeof(List<DefinitionOfParameterType>));

            using var fs = new FileStream(xmlFilePath, FileMode.OpenOrCreate);
            xmlSerializer.Serialize(fs, definitions);
        }

        private List<DefinitionOfParameterType> GetDefinitions()
        {
            var definitionsAsStringArray = ReadDefinitions();

            var definitions = new List<DefinitionOfParameterType>();

            var numberTypeNames = new List<string>()
            {
                "autodesk.spec:spec.int64",
                "autodesk.spec:spec.bool",
                "autodesk.spec.aec:numberOfPoles",
                "autodesk.spec.aec:number",
            };

            foreach (var definition in definitionsAsStringArray)
            {
                if (string.IsNullOrWhiteSpace(definition))
                    continue;

                var numberSignIndex = definition.IndexOf('#');
                var typeName = definition.Substring(0, numberSignIndex).Replace('_', ':');

                var sizeTableType = numberTypeNames.Contains(typeName)
                    ? NumberGeneralSizeTableType
                    : definition.Substring(numberSignIndex);

                definitions.Add(new(typeName, sizeTableType));
            }

            return definitions;
        }

        private string[] ReadDefinitions()
        {
            var openDialog = new OpenFileDialog { Filter = "Txt Files (*.txt)|*.txt" };

            if (openDialog.ShowDialog() == false)
                return [];

            _definitionsFileInfo = new FileInfo(openDialog.FileName);

            var header = File.ReadAllLines(_definitionsFileInfo.FullName)[0];

            return header.Split(',');
        }
    }
}

#endif
