using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Prism.Commands;

namespace CreatingButtons
{
    public class MainViewViewModel
    {
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            PipesCount = new DelegateCommand(CalculatePipesCount);
            WallVolume = new DelegateCommand(CalculateWallVolume);
            DoorsCount = new DelegateCommand(CalculateDoorsCount);
        }

        private ExternalCommandData _commandData;

        public DelegateCommand PipesCount { get; private set; }
        public DelegateCommand WallVolume { get; private set; }
        public DelegateCommand DoorsCount { get; private set; }

        public event EventHandler CloseRequest;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void CalculatePipesCount()
        {
            RaiseCloseRequest();

            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var pipesCount = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .ToElements()
                .OfType<Pipe>()
                .ToList();

            TaskDialog.Show("Pipes count", pipesCount.Count.ToString());
        }
        private void CalculateDoorsCount()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            var doorsCount = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            TaskDialog.Show("Doors count", doorsCount.Count.ToString());
        }

        private void CalculateWallVolume()
        {
            var uiApp = _commandData.Application;
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

            var result = UnitUtils.ConvertFromInternalUnits(volumeOfSelectedWalls, UnitTypeId.CubicMeters);

            TaskDialog.Show("Wall volume", result.ToString(CultureInfo.InvariantCulture));
        }
    }
}