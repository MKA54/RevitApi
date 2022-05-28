using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangingTypesWallPanel
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            const string tabName = "Changing Types Wall API";
            application.CreateRibbonTab(tabName);
            const string utilsFolderPath = @"C:\TestRevitApps\Revit\";

            var panel = application.CreateRibbonPanel(tabName, "Кнопки");
            var button = new PushButtonData("Система",
                "Смена типа стен",
                Path.Combine(utilsFolderPath, "ChangingTypesWall.dll"),
                "ChangingTypesWall.Main");

            panel.AddItem(button);

            return Result.Succeeded;
        }
    }
}