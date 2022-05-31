using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<XYZ> Points { get; set; } = new List<XYZ>();
        public List<FamilySymbol> FamilyTypes { get; private set; }

        public event EventHandler CloseRequest;
        public DelegateCommand SaveCommand { get; set; }

        public string ElementsCount { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Points = SelectionUtils.GetPoints(commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
            FamilyTypes = SelectionUtils.GetFamilySymbols(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveCommand()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            using (var ts = new Transaction(doc, "Create duct"))
            {
                ts.Start();

                ts.Commit();
            }

            RaiseCloseRequest();
        }
    }
}
