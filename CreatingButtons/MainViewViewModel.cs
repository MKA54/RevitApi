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
using TrainingLibrary;

namespace CreatingButtons
{
    public class MainViewViewModel
    {
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            PipesCount = new DelegateCommand(GetPipesCount);
            WallVolume = new DelegateCommand(GetWallVolume);
            DoorsCount = new DelegateCommand(GetDoorsCount);

            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;
        }

        private readonly ExternalCommandData _commandData;
        
        public DelegateCommand PipesCount { get; private set; }
        public DelegateCommand WallVolume { get; private set; }
        public DelegateCommand DoorsCount { get; private set; }

        public event EventHandler HideRequest;
        public event EventHandler ShowRequest;

        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        private void GetPipesCount()
        {
            RaiseHideRequest();

            var pipesCount = SelectionUtils.CalculatePipesCount(_commandData);
            TaskDialog.Show("Pipes count", pipesCount.ToString());

            RaiseShowRequest();
        }

        private void GetDoorsCount()
        {
            RaiseHideRequest();

            var doorsCount = SelectionUtils.CalculateDoorsCount(_commandData);
            TaskDialog.Show("Doors count", doorsCount.ToString());

            RaiseShowRequest();
        }

        private void GetWallVolume()
        {
            RaiseHideRequest();

            var result = UnitUtils.ConvertFromInternalUnits(SelectionUtils.CalculateWallVolume(_commandData),
                UnitTypeId.CubicMeters);
            TaskDialog.Show("Wall volume", result.ToString(CultureInfo.InvariantCulture));

            RaiseShowRequest();
        }
    }
}