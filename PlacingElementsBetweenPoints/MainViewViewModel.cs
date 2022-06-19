using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using TrainingLibrary;

namespace PlacingElementsBetweenPoints
{
    public class MainViewViewModel : INotifyPropertyChanged
    {
        private readonly ExternalCommandData _commandData;
        private string _elementsCount;
        public List<XYZ> Points { get; set; }
        public List<FamilySymbol> FamilyTypes { get; private set; }
        public FamilySymbol SelectedFamilyType { get; set; }
        public DelegateCommand SaveCommand { get; }
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
            FamilyTypes = SelectionUtils.GetFamilySymbol(_commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void OnSaveCommand()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var distance = Points[0].DistanceTo(Points[1]);

            using (var ts = new Transaction(doc, "Placing elements"))
            {
                ts.Start();

                var lengthParameter = SelectedFamilyType.Parameters;
                var length = UnitUtils.ConvertFromInternalUnits(lengthParameter, UnitTypeId.Meters);
                var count = (int)distance / length;

                //ElementsCount = count.ToString(CultureInfo.InvariantCulture);

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