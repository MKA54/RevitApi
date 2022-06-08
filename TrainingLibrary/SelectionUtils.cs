using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TrainingLibrary
{
    public class SelectionUtils
    {
        public static List<FamilySymbol> GetFamilySymbols(ExternalCommandData commandData)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var familySymbols = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .ToList();

            return familySymbols;
        }

        public static List<XYZ> GetPoints(ExternalCommandData commandData, string promptMessage, 
            ObjectSnapTypes objectSnapTypes)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;

            var points = new List<XYZ>();

            while (true)
            {
                XYZ pickedPoint;
                try
                {
                    pickedPoint = uiDoc.Selection.PickPoint(objectSnapTypes, promptMessage);
                }
                catch (OperationCanceledException)
                {
                   break;
                }

                points.Add(pickedPoint);
            }

            return points;
        }

        public static List<XYZ> GetPoints(ExternalCommandData commandData, string promptMessage)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;

            var points = new List<XYZ>();

            while (true)
            {
                XYZ pickedPoint;
                try
                {
                    pickedPoint = uiDoc.Selection.PickPoint(promptMessage);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                points.Add(pickedPoint);
            }

            return points;
        }

        public static List<DuctType> GetDuctTypes(ExternalCommandData commandData)
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            return new FilteredElementCollector(doc)
                .OfClass(typeof(DuctType))
                .Cast<DuctType>()
                .ToList();
        }

        public static List<Level> GetLevels(ExternalCommandData commandData)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();
        }

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
                .ToList(); 
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