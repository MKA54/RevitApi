using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;

namespace ParameterEntry
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var selectedRefList = uiDoc.Selection.PickObjects(ObjectType.Element, "Выберите элементы");

            foreach (var e in selectedRefList)
            {
                using (var ts = new Transaction(doc, "Set Parameters"))
                {
                    ts.Start();

                    var element = doc.GetElement(e);

                    if (!(element is Pipe))
                    {
                        continue;
                    }

                    var lengthParameter = element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);

                    if (lengthParameter.StorageType == StorageType.Double)
                    {
                        var stockLength = UnitUtils.ConvertFromInternalUnits(lengthParameter.AsDouble(), UnitTypeId.Meters);
                        stockLength *= 1.1;

                        var parameter = element.LookupParameter("Длина с запасом");
                        parameter.Set(stockLength);
                    }

                    ts.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}