using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;

namespace CreatingSheets
{
    public class MainViewViewModel
    {
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SheetTypes = GetSheetTypes();
            Views = GetForPlanViews();
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private ExternalCommandData _commandData;
        public DelegateCommand SaveCommand { get; set; }
        public string SheetsCount { get; set; }
        public string Developed { get; set; }
        public ViewPlan SelectedViewPlan { get; set; }
        public List<ViewPlan> Views { get; set; }
        public List<FamilySymbol> SheetTypes { get; set; }
        public FamilySymbol SelectedSheetType { get; set; }
        public event EventHandler CloseRequest;

        private void OnSaveCommand()
        {
            if (SelectedSheetType == null || SelectedViewPlan == null)
            {
                return;
            }

            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var count = int.Parse(SheetsCount);

            using (var tr = new Transaction(doc, "Create a new ViewSheet"))
            {
                tr.Start();

                for (var i = 0; i < count; i++)
                {
                    var sheet = ViewSheet.Create(doc, SelectedSheetType.Id);
                }

                tr.Commit();
            }

            RaiseCloseRequest();
        }

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private List<FamilySymbol> GetSheetTypes()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            return new FilteredElementCollector(doc)
                    .WhereElementIsElementType()
                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .Cast<FamilySymbol>()
                    .ToList();
        }

        private List<ViewPlan> GetForPlanViews()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            return new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(p => p.ViewType == ViewType.FloorPlan)
                .ToList();
        }
    }
}
