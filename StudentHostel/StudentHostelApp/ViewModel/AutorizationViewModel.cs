using System;
using System.Linq;
using System.Security;
using System.Runtime.InteropServices;
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

        public RelayCommand LoginCommand { get; set; }

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
            LoginCommand = new RelayCommand(Login);
        }

        /// <summary>
        /// Выполняет авторизацию
        /// </summary>
        protected void Login(object parameter)
        {
            // Получение пароля 
            string password = string.Empty;
            var passwordContainer = parameter as IHavePassword;
            if (passwordContainer != null)
            {
                var secureString = passwordContainer.Password;
                password = ConvertToUnsecureString(secureString);

                try
                {
                    var user = context.Users.Where(p => p.Login == CurrentUser.Login && p.Password == password).SingleOrDefault();
                    if (user != null)
                    {
                        UserInfo.CurrentUser = new UserViewModel
                        {
                            UserId = user.UserId,
                            Login = user.Login
                        };

                        if (user.Role == DataTypes.Role.Admin)
                        {
                            UserInfo.CurrentUser.RoleName = "Admin";
                        }
                        else if (user.Role == DataTypes.Role.User)
                        {
                            UserInfo.CurrentUser.RoleName = "User";
                        }
                        ErrorMessage = "Success!";
                    }
                    else
                    {
                        ErrorMessage = "Неверные логин и/или пароль!";
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    System.Windows.MessageBox.Show(e.Message);
#endif
                    ErrorMessage = "Ошибка при загрузке данных!";
                }
            }
        }

        /// <summary>
        /// Преобразует защищенную строку в незащищенную
        /// </summary>
        /// <param name="securePassword">Защищенный пароль</param>
        /// <returns></returns>
        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
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
