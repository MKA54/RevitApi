using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;

namespace ProjectParameter
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var familyInstances = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfClass(typeof(Pipe))
                .ToElements()
                .OfType<Pipe>()
                .ToList();

            foreach (var e in familyInstances)
            {
                using (var ts = new Transaction(doc, "Set Parameters"))
                {
                    ts.Start();

                    var outsideDiameterParameter = e.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                    var insideDiameterParameter = e.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);

                    if (outsideDiameterParameter.StorageType == StorageType.Double && insideDiameterParameter.StorageType == StorageType.Double)
                    {
                        var outsideDiameter = UnitUtils.ConvertFromInternalUnits(outsideDiameterParameter.AsDouble(), UnitTypeId.Millimeters);
                        var insideDiameter = UnitUtils.ConvertFromInternalUnits(insideDiameterParameter.AsDouble(), UnitTypeId.Millimeters);
                        var parameter = e.LookupParameter("Наименование");
                        parameter.Set("Труба " + outsideDiameter + "/" + insideDiameter);
                    }

                    ts.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}