using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB.Plumbing;
using NPOI.XSSF.UserModel;

namespace WritingToExcel
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            var pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .ToElements()
                .OfType<Pipe>()
                .ToList();

            var excelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                "ducts.xlsx");

            using (var stream = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet("Лист 1");

                var rowIndex = 0;

                foreach (var pipe in pipes)
                {
                    sheet.SetCellValue(rowIndex, columnIndex: 0, pipe.get_Parameter
                        (BuiltInParameter.ALL_MODEL_TYPE_NAME).AsString());
                    sheet.SetCellValue(rowIndex, columnIndex: 1, pipe.get_Parameter
                        (BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble());
                    sheet.SetCellValue(rowIndex, columnIndex: 2, pipe.get_Parameter
                        (BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble());
                    sheet.SetCellValue(rowIndex, columnIndex: 3, pipe.get_Parameter
                        (BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble());

                    rowIndex++;
                }

                workbook.Write(stream);
                workbook.Close();
            }

            System.Diagnostics.Process.Start(excelPath);

            return Result.Succeeded;
        }
    }
}