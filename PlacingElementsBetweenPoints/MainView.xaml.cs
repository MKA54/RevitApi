using System.Windows;
using Autodesk.Revit.UI;

namespace PlacingElementsBetweenPoints
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(ExternalCommandData commandData)
        {
            InitializeComponent();
            var vm = new MainViewViewModel(commandData);
            DataContext = vm;
        }
    }
}