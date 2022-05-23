using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;

namespace PipesCount
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var familyInstances = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfClass(typeof(Pipe))
                .ToElements()
                .OfType<Pipe>()
                .ToList();

            TaskDialog.Show("Pipes count", familyInstances.Count.ToString());

            return Result.Succeeded;
        }
    }
}