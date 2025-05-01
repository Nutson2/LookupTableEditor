using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LookupTableEditor.Extentions;

namespace LookupTableEditor.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
internal class test2 : IExternalCommand
{
    Document doc = null!;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        doc = commandData.Application.ActiveUIDocument.Document;

        Execute();

        return Result.Succeeded;
    }

    public void Execute()
    {
        using (var t = new Transaction(doc, "Обновить таблицу размеров"))
        {
            t.Start();

            FamilySizeTableErrorInfo errorInfo = new();
            FamilySizeTableManager.CreateFamilySizeTableManager(doc, doc.OwnerFamily.Id);
            var tableManager = FamilySizeTableManager.GetFamilySizeTableManager(
                doc,
                doc.OwnerFamily.Id
            );

            var name = "Кран шаровый фланцевый LD";
            if (tableManager.HasSizeTable(name))
                tableManager.RemoveSizeTable(name);
            var myPath = Path.Combine(
                @"C:\Users\Nutson\Documents\Кран шаровый фланцевый LD",
                $"{name}.csv"
            );
            var res = tableManager.ImportSizeTable(doc, myPath, errorInfo);
            Console.WriteLine(res);

            var p = doc
                .FamilyManager.Parameters.Cast<FamilyParameter>()
                .Where(fp => fp.IsDeterminedByFormula)
                .ForEach(fp => doc.FamilyManager.SetFormula(fp, fp.Formula));

            t.Commit();
        }
    }
}
