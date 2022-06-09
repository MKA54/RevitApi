using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;

namespace TotalLengthPipes
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData externalCommand, ref string message, ElementSet elements)
        {
            var uiApplication = externalCommand.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var familyInstances = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfClass(typeof(Pipe))
                .ToElements()
                .OfType<Pipe>()
                .ToList();

            var totalLength = 0.0;

            familyInstances.ForEach(pipe =>
            {
                var lengthParameter = pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);

                if (lengthParameter.StorageType == StorageType.Double)
                {
                    totalLength += lengthParameter.AsDouble();
                }
            });

            var result = UnitUtils.ConvertFromInternalUnits(totalLength, UnitTypeId.Meters);

            TaskDialog.Show("Total length pipes", result.ToString(CultureInfo.InvariantCulture));

            return Result.Succeeded;
        }
    }
}