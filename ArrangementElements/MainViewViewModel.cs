using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
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
            Elements = GetElements();
            Levels = SelectionUtils.GetLevels(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            Point = GetPoint();
        }

        private readonly ExternalCommandData _commandData;

        public List<FamilySymbol> Elements { get; private set; }
        public event EventHandler CloseRequest;
        public DelegateCommand SaveCommand { get; }
        public XYZ Point { get; set; }
        public List<Level> Levels { get; private set; }
        public Level SelectedLevel { get; set; }
        public FamilySymbol SelectedElement { get; set; }

        private void OnSaveCommand()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            using (var tr = new Transaction(doc, "Create family instance"))
            {
                tr.Start();

                doc.Create.NewFamilyInstance(Point, SelectedElement, SelectedLevel, StructuralType.NonStructural);

                tr.Commit();
            }

            RaiseCloseRequest();
        }

        private XYZ GetPoint()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;

            return uiDoc.Selection.PickPoint(ObjectSnapTypes.Endpoints, "Выберите точку вставки");
        }

        private List<FamilySymbol> GetElements()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Furniture)
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .ToList();
        }
        
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}