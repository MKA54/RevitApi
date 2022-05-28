﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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