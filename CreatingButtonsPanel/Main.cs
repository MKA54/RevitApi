using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingButtonsPanel
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
            const string tabName = "Creating Buttons API";
            application.CreateRibbonTab(tabName);
            const string utilsFolderPath = @"C:\TestRevitApps\Revit\";

            var panel = application.CreateRibbonPanel(tabName, "Перечень команд(кнопки)");
            var button = new PushButtonData("Система",
                "Кнопки",
                Path.Combine(utilsFolderPath, "CreatingButtons.dll"),
                "CreatingButtons.Main");

            panel.AddItem(button);

            return Result.Succeeded;
        }
    }
}