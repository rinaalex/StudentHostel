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
using StudentHostelApp.Code;

namespace StudentHostelApp.View
{
    /// <summary>
    /// Логика взаимодействия для AutorizationView.xaml
    /// </summary>
    public partial class AutorizationView : Window, IHavePassword
    {
        public System.Security.SecureString Password
        {
            get { return UserPassword.SecurePassword; }
        }

        private AutorizationViewModel viewModel;
        public AutorizationView()
        {
            InitializeComponent();
            viewModel = new AutorizationViewModel(new StudentHostelContext());
            this.DataContext = viewModel;
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AutorizationCommand.Execute("");
            if (UserInfo.CurrentUser.RoleName!=null)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
