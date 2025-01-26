using System;
using Autodesk.Revit.DB;

namespace LookupTableEditor.Extentions
{
    public static class DocumentExtentions
    {
        public static void Run(this Document document, string name, Action action)
        {
            using (var tr = new Transaction(document, name))
            {
                tr.Start();
                action();
                tr.Commit();
            }
        }
    }
}
