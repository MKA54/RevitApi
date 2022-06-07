using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace ExportToImage
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApplication = commandData.Application;
            var uiDoc = uiApplication.ActiveUIDocument;
            var doc = uiDoc.Document;

            using (var ts = new Transaction(doc, "export IMAGE"))
            {
                ts.Start();

                var img = new ImageExportOptions
                {
                    ExportRange = ExportRange.CurrentView,
                    HLRandWFViewsFileType = ImageFileType.PNG
                };

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                img.FilePath = desktopPath + @"\" + doc.ActiveView.Name;
                img.ShadowViewsFileType = ImageFileType.PNG;

                doc.ExportImage(img);

                ts.Commit();
            }

            return Result.Succeeded;
        }
    }
}