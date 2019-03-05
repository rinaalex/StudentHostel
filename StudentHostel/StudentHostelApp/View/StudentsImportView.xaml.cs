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
using StudentHostelApp.Code;
using StudentHostelApp.ViewModel;
using StudentHostelApp.DataAccess;
using Microsoft.Win32;

namespace StudentHostelApp.View
{
    /// <summary>
    /// Логика взаимодействия для StudentListExcelView.xaml
    /// </summary>
    public partial class StudentsImportView : Window
    {
        private StudentsImportViewModel viewModel;
        public StudentsImportView()
        {
            InitializeComponent();
            viewModel = new StudentsImportViewModel(new StudentHostelContext());
            this.DataContext = viewModel;
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter += "Лист Microsoft Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog()==true)
            {
                this.FileNameTxt.Text = openFileDialog.FileName;
            }
        }
    }
}
