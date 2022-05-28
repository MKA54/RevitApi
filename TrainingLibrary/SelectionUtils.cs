using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TrainingLibrary
{
    public class SelectionUtils
    {
        public static List<Element> PickObjects(ExternalCommandData commandData, string message = "Выберите элементы")
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var elementList = new List<Element>();

            try
            {
                var selectedObjects = uiDoc.Selection.PickObjects(ObjectType.Element, message);
                elementList = selectedObjects.Select(selectedObject => doc.GetElement(selectedObject)).ToList();
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
            { }

            return elementList;
        }

        public static List<WallType> GetTypesWalls(ExternalCommandData commandData)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            return new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .Cast<WallType>()
                .ToList(); ;
        }

        public static int CalculatePipesCount(ExternalCommandData commandData)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            return new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .ToElements()
                .OfType<Pipe>()
                .ToList()
                .Count;
        }

        public static int CalculateDoorsCount(ExternalCommandData commandData)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList()
                .Count;
        }

        public static double CalculateWallVolume(ExternalCommandData commandData)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var walls = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            var volumeOfSelectedWalls = 0.0;

            foreach (var wall in walls)
            {
                var volumeParameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);

                if (volumeParameter.StorageType == StorageType.Double)
                {
                    volumeOfSelectedWalls += volumeParameter.AsDouble();
                }
            }

            return volumeOfSelectedWalls;
        }
    }
}