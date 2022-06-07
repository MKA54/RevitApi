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
            Points = SelectionUtils.GetPoints(commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
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

            using (var ts = new Transaction(doc, "Create duct"))
            {
                ts.Start();

                var length = SelectedFamilyType.Id.IntegerValue;
                var count = distance % length;

                ElementsCount = count.ToString(CultureInfo.InvariantCulture);

                ts.Commit();
            }

            RaiseCloseRequest();
        }

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}