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
using StudentHostelApp.DataAccess;

namespace StudentHostelApp.View
{
    /// <summary>
    /// Логика взаимодействия для RoomsListView.xaml
    /// </summary>
    public partial class RoomsListView : Window
    {
        RoomsListViewModel viewModel = new RoomsListViewModel(new StudentHostelContext());
        public RoomsListView()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы действительно хотите удалить информацию о выбранном объекте?",
                "Предупреждение",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
            {
                viewModel.DeleteCommand.Execute("");
            }
        }
    }
}
