using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TrainingLibrary;

namespace PlacingElementsBetweenPoints
{
    public class MainViewViewModel : INotifyPropertyChanged
    {
        private readonly ExternalCommandData _commandData;
        private string _elementsCount;
        public List<XYZ> Points { get; set; }
        public List<FamilySymbol> FamilyTypes { get; set; }
        public event EventHandler CloseRequest;
        public event PropertyChangedEventHandler PropertyChanged;

        public string ElementsCount
        {
            get => _elementsCount;

            set
            {
                _elementsCount = value;
                OnPropertyChanged();
            }
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
                    var lengthParameter = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
                    var length = UnitUtils.ConvertFromInternalUnits(lengthParameter, UnitTypeId.Meters);
                    var count = (int)distance / length;

                    ElementsCount += e.Name + ": " + count.ToString(CultureInfo.InvariantCulture) + "/n";
                }

                ts.Commit();
            }

            //RaiseCloseRequest();
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}