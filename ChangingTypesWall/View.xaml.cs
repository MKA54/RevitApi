using System.Windows;
using Autodesk.Revit.UI;

namespace ChangingTypesWall
{
    /// <summary>
    /// Логика взаимодействия для View.xaml
    /// </summary>
    public partial class View : Window
    {
        public View(ExternalCommandData commandData)
        {
            InitializeComponent();
            var vm = new MainViewViewModel(commandData);
            vm.CloseRequest += (s, e) => this.Close();
            DataContext = vm;
        }
    }
}
