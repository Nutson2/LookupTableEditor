#if DEBUG

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LookupTableEditor.Models;
using Microsoft.Win32;

namespace LookupTableEditor.Commands;

[Transaction(TransactionMode.Manual)]
internal class CreateParameterDefinitionsXmlCommand : IExternalCommand
{
	private const string NumberGeneralSizeTableType = "##NUMBER##GENERAL";
	private FileInfo? _definitionsFileInfo;

	public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
	{
		List<DefinitionOfParameterType> definitions = GetDefinitions();

		if(definitions.Count == 0)
			return Result.Cancelled;

		SerializeDefinitions(definitions, commandData.Application.Application.VersionNumber);

		return Result.Succeeded;
	}

	private void SerializeDefinitions(List<DefinitionOfParameterType> definitions, string versionNumber)
	{
		if(_definitionsFileInfo is null)
			return;
		string xmlFilePath = Path.Combine(_definitionsFileInfo.DirectoryName, $"ParametersTypes{versionNumber}.xml");

		XmlSerializer xmlSerializer = new(typeof(List<DefinitionOfParameterType>));

		using FileStream fs = new(xmlFilePath, FileMode.OpenOrCreate);
		xmlSerializer.Serialize(fs, definitions);
	}

	private List<DefinitionOfParameterType> GetDefinitions()
	{
		string[] definitionsAsStringArray = ReadDefinitions();

		List<DefinitionOfParameterType> definitions = new();

		List<string> numberTypeNames = new()
		{
			"autodesk.spec:spec.int64",
			"autodesk.spec:spec.bool",
			"autodesk.spec.aec:numberOfPoles",
			"autodesk.spec.aec:number",
		};

		foreach(string definition in definitionsAsStringArray)
		{
			if(string.IsNullOrWhiteSpace(definition))
				continue;

			int numberSignIndex = definition.IndexOf('#');
			string typeName = definition.Substring(0, numberSignIndex).Replace('_', ':');

			string sizeTableType = numberTypeNames.Contains(typeName)
				? NumberGeneralSizeTableType
				: definition.Substring(numberSignIndex);

			definitions.Add(
				new DefinitionOfParameterType { TypeName = typeName, SizeTableType = sizeTableType }
			);
		}

		return definitions;
	}

	private string[] ReadDefinitions()
	{
		OpenFileDialog openDialog = new() { Filter = "Txt Files (*.txt)|*.txt" };

		if(openDialog.ShowDialog() == false)
			return [];

		_definitionsFileInfo = new FileInfo(openDialog.FileName);

		string? header = File.ReadAllLines(_definitionsFileInfo.FullName)[0];

		return header.Split(',');
	}
}

#endif
