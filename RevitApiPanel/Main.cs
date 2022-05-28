using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitApiPanel
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
            const string tabName = "Panel API";
            application.CreateRibbonTab(tabName);
            const string utilsFolderPath = @"C:\TestRevitApps\Revit\";

            var panel = application.CreateRibbonPanel(tabName, "Панель кнопок");
            var button1 = new PushButtonData("Кнопки",
                "Смена типа стен",
                Path.Combine(utilsFolderPath, "ChangingTypesWall.dll"),
                "ChangingTypesWall.Main");

            var button2 = new PushButtonData("Стены",
                            "Кнопки",
                            Path.Combine(utilsFolderPath, "CreatingButtons.dll"),
                            "CreatingButtons.Main");

            panel.AddItem(button1);
            panel.AddItem(button2);

            return Result.Succeeded;
        }
    }
}