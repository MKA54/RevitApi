using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Linq;

namespace AirDuctsCountPerFloor
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

            var levelsArray = new string[] {
                "1 этаж: ",
                "2 этаж: "
            };

            var groundFloorQuanity = 0;
            var secondOnFloorQuanity = 0;

            for (var i = 0; i < familyInstances.Count; i++)
            {
                if (familyInstances[i].ReferenceLevel.Name.Contains("1"))
                {
                    groundFloorQuanity++;
                    continue;
                }

                secondOnFloorQuanity++;
            }

            levelsArray[0] = levelsArray[0] + groundFloorQuanity;
            levelsArray[1] = levelsArray[1] + secondOnFloorQuanity;

            TaskDialog.Show("Ducts count", string.Join(", ", levelsArray));

            return Result.Succeeded;
        }
    }
}