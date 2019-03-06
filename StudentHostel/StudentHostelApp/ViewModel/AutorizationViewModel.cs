using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using StudentHostelApp.ViewModel.SingleEntityVM;
using StudentHostelApp.Commands;
using StudentHostelApp.DataAccess;
using StudentHostelApp.Code;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Реализует авторизацию пользователя
    /// </summary>
    public class AutorizationViewModel:INotifyPropertyChanged
    {
        private readonly StudentHostelContext context;

        private UserViewModel currentUser;
        public UserViewModel CurrentUser
        {
            get { return this.currentUser; }
            set { this.currentUser = value; OnPropertyChanged(nameof(CurrentUser)); }
        }

        public Command AutorizationCommand { get; set; }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set { this.errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public AutorizationViewModel(StudentHostelContext context)
        {
            this.context = context;
            CurrentUser = new UserViewModel();
            AutorizationCommand = new Command(Autorization, () => { return true; });
        }

        /// <summary>
        /// Выполняет авторизацию
        /// </summary>
        protected void Autorization()
        {
            try
            {
                var user = context.Users.Where(p => p.Login == CurrentUser.Login && p.Password == CurrentUser.Password).SingleOrDefault();
                if (user!=null)
                {
                    UserInfo.CurrentUser = new UserViewModel
                    {
                        UserId = user.UserId,
                        Login = user.Login,
                        Password = user.Password
                    };

                    if (user.Role==DataTypes.Role.Admin)
                    {
                        UserInfo.CurrentUser.RoleName = "Admin";
                    }
                    else if (user.Role==DataTypes.Role.User)
                    {
                        UserInfo.CurrentUser.RoleName = "User";
                    }
                }
                else
                {
                    ErrorMessage = "Неверные логин и/или пароль!";
                }
            }
            catch(Exception e)
            {
#if DEBUG
                System.Windows.MessageBox.Show(e.Message);
#endif
                ErrorMessage = "Ошибка при загрузке данных!";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
