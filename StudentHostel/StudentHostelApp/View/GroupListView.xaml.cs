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
    /// Логика взаимодействия для GroupListView.xaml
    /// </summary>
    public partial class GroupListView : Window
    {
        GroupListViewModel viewModel = new GroupListViewModel(new DataAccess.StudentHostelContext());

        public GroupListView()
        {
            InitializeComponent();
            this.DataContext = viewModel;      
        }

        // Выполняет вывод подтверждения удаления объекта
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить выбранную группу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                viewModel.DeleteCommand.Execute("");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
