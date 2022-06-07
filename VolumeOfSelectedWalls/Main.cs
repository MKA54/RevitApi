using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Globalization;
using Autodesk.Revit.UI.Selection;

namespace VolumeOfSelectedWalls
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var selectedElementsRefList = uiDoc.Selection.PickObjects(ObjectType.Element, "Выберите элементы");

            var volumeOfSelectedWalls = 0.0;

            foreach (var e in selectedElementsRefList)
            {
                var element = doc.GetElement(e);

                var volumeParameter = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);

                if (volumeParameter.StorageType == StorageType.Double)
                {
                    volumeOfSelectedWalls += volumeParameter.AsDouble();
                }
            }

            var result = UnitUtils.ConvertFromInternalUnits(volumeOfSelectedWalls, UnitTypeId.CubicMeters);

            TaskDialog.Show("Total length pipes", result.ToString(CultureInfo.InvariantCulture));

            return Result.Succeeded;
        }
    }
}