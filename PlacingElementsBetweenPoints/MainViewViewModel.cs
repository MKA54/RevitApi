using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TrainingLibrary;

namespace PlacingElementsBetweenPoints
{
    public class MainViewViewModel
    {
        private readonly ExternalCommandData _commandData;
        public List<XYZ> Points { get; set; }
        public List<FamilyInstance> FamilyTypes { get; set; }

        public string ElementsCount {
            get;
            set;
        }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Points = GetPoints();
            FamilyTypes = SelectionUtils.GetFamilyTypes(_commandData);
            OutputData();
        }

        private void OutputData()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var distance = Points[0].DistanceTo(Points[1]);

            using (var ts = new Transaction(doc, "Placing elements"))
            {
                ts.Start();

                foreach (var e in FamilyTypes)
                {
                    var lengthParameter = e.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
                    var length = UnitUtils.ConvertFromInternalUnits(lengthParameter, UnitTypeId.CubicMeters);
                    var count = Math.Ceiling(distance / length);

                    ElementsCount += e.Name + ": " + count.ToString(CultureInfo.InvariantCulture) + "\n";
                }

                ts.Commit();
            }
        }

        private List<XYZ> GetPoints()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;

            var points = new List<XYZ>();

            var i = 0;
            while (i != 2)
            {
                var pickedPoint = uiDoc.Selection.PickPoint(ObjectSnapTypes.Endpoints, "Выберите 2 точки");

                points.Add(pickedPoint);

                i++;
            }

            return points;
        }
    }
}