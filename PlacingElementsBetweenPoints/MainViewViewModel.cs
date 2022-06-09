using System;
using System.Collections.Generic;
using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using TrainingLibrary;

namespace PlacingElementsBetweenPoints
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public List<XYZ> Points { get; set; }
        public List<FamilySymbol> FamilyTypes { get; private set; }
        public FamilySymbol SelectedFamilyType { get; set; }
        public DelegateCommand SaveCommand { get; }
        public event EventHandler CloseRequest;

        public string ElementsCount { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Points = GetPoints();
            FamilyTypes = SelectionUtils.GetFamilySymbols(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnSaveCommand()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var distance = Math.Sqrt((Points[0].X - Points[1].X) * (Points[0].X - Points[1].X)
                                     + (Points[0].Y - Points[1].Y) * (Points[0].Y - Points[1].Y)
                                     + (Points[0].Z - Points[1].Z) * (Points[0].Z - Points[1].Z));

            using (var ts = new Transaction(doc, "Placing elements"))
            {
                ts.Start();

                var lengthParameter = SelectedFamilyType.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                var length = UnitUtils.ConvertFromInternalUnits(lengthParameter, UnitTypeId.Meters);
                var count = (int)distance / length;

                ElementsCount = count.ToString(CultureInfo.InvariantCulture);

                ts.Commit();
            }

            RaiseCloseRequest();
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

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}