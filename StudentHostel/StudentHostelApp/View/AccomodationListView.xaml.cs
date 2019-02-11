using System;
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
using System.Windows.Shapes;
using StudentHostelApp.ViewModel;

namespace StudentHostelApp.View
{
    /// <summary>
    /// Логика взаимодействия для AccomodationView.xaml
    /// </summary>
    public partial class AccomodationListView : Window
    {
        AccomodationListViewModel viewModel = new AccomodationListViewModel();
        public AccomodationListView()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
