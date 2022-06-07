using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;

namespace OutputValues
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var walls = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            var wallsInfo = string.Empty;

            foreach (var wall in walls)
            {
                var typeName = wall.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_NAME).AsString();
                var volume = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble();

                wallsInfo += $"{typeName}, {volume}{Environment.NewLine}";
            }

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var txtPath = Path.Combine(desktopPath, "wallsInfo.txt");

            File.WriteAllText(txtPath, wallsInfo);

            return Result.Succeeded;
        }
    }
}