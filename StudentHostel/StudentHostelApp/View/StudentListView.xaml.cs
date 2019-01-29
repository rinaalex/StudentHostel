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
    /// Логика взаимодействия для StudentListView.xaml
    /// </summary>
    public partial class StudentListView : Window
    {
        public StudentListView()
        {
            InitializeComponent();
            this.DataContext = new StudentListViewModel();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //костыль для обновления значения в ячейке после отмены изменений
            dataGrid.Items.Refresh();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //костыль для обновления значения в ячейке после отмены изменений
            dataGrid.Items.Refresh();
        }
    }
}
