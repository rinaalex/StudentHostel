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
    /// Логика взаимодействия для StudentListView.xaml
    /// </summary>
    public partial class StudentListView : Window
    {
        StudentListViewModel viewModel = new StudentListViewModel(new StudentHostelContext());
        public StudentListView()
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
            if(MessageBox.Show("Вы дейстивительно хотите удалить информацию о студенте?","Предупреждение",
                MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                viewModel.DeleteCommand.Execute("");
            }
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            StudentsImportView studentListExcelView = new StudentsImportView();
            studentListExcelView.Show();
        }
    }
}
