using System.Windows;
using Autodesk.Revit.UI;

namespace CreatingButtons
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
            vm.HideRequest += (s, e) => this.Hide();
            vm.ShowRequest += (s, e) => this.Show();
            DataContext = vm;
        }
    }
}
