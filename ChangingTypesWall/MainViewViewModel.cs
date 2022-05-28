using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using TrainingLibrary;

namespace ChangingTypesWall
{
    public class MainViewViewModel
    {
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SaveCommand = new DelegateCommand(OnSaveCommand);
            PickedObjects = SelectionUtils.PickObjects(commandData);
            WallsTypes = SelectionUtils.GetTypesWalls(commandData);
        }

        private readonly ExternalCommandData _commandData;

        public DelegateCommand SaveCommand { get; }
        public List<Element> PickedObjects { get; }
        public List<WallType> WallsTypes { get; } = new List<WallType>();
        public WallType SelectedWallType { get; set; }

        public event EventHandler CloseRequest;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveCommand()
        {
            var uiApp = _commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            if (PickedObjects == null || SelectedWallType == null)
            {
                return;
            }

            using (var ts = new Transaction(doc, "Set wall type"))
            {
                ts.Start();

                foreach (var pickedObject in PickedObjects)
                {
                    if (pickedObject is Wall)
                    {
                        var oWall = pickedObject as Wall;

                        oWall.WallType.Name = SelectedWallType.Name;
                    }
                }

                ts.Commit();
            }

            RaiseCloseRequest();
        }
    }
}