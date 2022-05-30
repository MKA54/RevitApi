using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using TrainingLibrary;

namespace ApplicationInWpf
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public List<DuctType> DuctTypes { get;  set; } = new List<DuctType>();
        public List<Level> Levels { get;  set; } = new List<Level>();
        public DelegateCommand SaveCommand { get;  set; }
        public double Displacement { get; set; }
        public List<XYZ> Points { get;  set; } = new List<XYZ>();
        public DuctType SelectedDuctType { get; set; }
        public Level SelectedLevel { get; set; }
        public event EventHandler CloseRequest;

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            DuctTypes = SelectionUtils.GetDuctTypes(commandData);
            Levels = SelectionUtils.GetLevels(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            Displacement = 0.0;
            Points = SelectionUtils.GetPoints(commandData, "Выберите точки", ObjectSnapTypes.Endpoints);
        }

        private void OnSaveCommand()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            if (Points.Count < 2 || SelectedDuctType == null || SelectedLevel == null)
            {
                return;
            }

            var curves = new List<Curve>();

            for (var i = 0; i < Points.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                var prevPoints = Points[i - 1];
                var currentPoints = Points[i];

                var curve = Line.CreateBound(prevPoints, currentPoints);

                curves.Add(curve);
            }

            var mepSystemType = new FilteredElementCollector(doc)
                .OfClass(typeof(MEPSystemType))
                .Cast<MEPSystemType>()
                .FirstOrDefault(sysType => sysType.SystemClassification == MEPSystemClassification.SupplyAir);

            using (var ts = new Transaction(doc, "Create duct"))
            {
                ts.Start();

                foreach (var curve in curves)
                {
                    Duct.Create(doc, mepSystemType.Id, SelectedDuctType.Id, SelectedLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                }

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
