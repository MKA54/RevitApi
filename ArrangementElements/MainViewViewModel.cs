using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using TrainingLibrary;

namespace ArrangementElements
{
    public class MainViewViewModel
    {
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Levels = SelectionUtils.GetLevels(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            Points = SelectionUtils.GetPoints(commandData, "Точка вставки", ObjectSnapTypes.Endpoints);
        }

        private readonly ExternalCommandData _commandData;
        public event EventHandler CloseRequest;
        public DelegateCommand SaveCommand { get; }
        public List<XYZ> Points { get; set; }
        public List<Level> Levels { get; private set; } = new List<Level>();
        public Level SelectedLevel { get; set; }

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveCommand()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var furniture = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Furniture)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            //SelectionUtils.CreateFamilyInstance(_commandData, furniture, Points[0] , SelectedLevel);

            RaiseCloseRequest();
        }
    }
}