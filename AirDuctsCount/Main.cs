using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using Autodesk.Revit.DB.Mechanical;

namespace AirDuctsCount
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var familyInstances = new FilteredElementCollector(doc)
                .OfClass(typeof(Duct))
                .ToElements()
                .OfType<Duct>()
                .ToList();

            TaskDialog.Show("Ducts count", familyInstances.Count.ToString());

            return Result.Succeeded;
        }
    }
}