using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                var img = new ImageExportOptions();

                img.ExportRange = ExportRange.CurrentView;
                img.HLRandWFViewsFileType = ImageFileType.PNG;

                var DEsktoppath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                img.FilePath = DEsktoppath + @"\" + doc.ActiveView.Name;
                img.ShadowViewsFileType = ImageFileType.PNG;

                doc.ExportImage(img);

                ts.Commit();
            }

            return Result.Succeeded;
        }
    }
}