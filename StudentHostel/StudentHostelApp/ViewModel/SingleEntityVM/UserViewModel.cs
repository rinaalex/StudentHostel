using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentHostelApp.Code;

namespace StudentHostelApp.ViewModel.SingleEntityVM
{
    /// <summary>
    /// Реализует модель представления сущности Пользователь
    /// </summary>
    public class UserViewModel: BaseViewModel
    {
        private int userId;
        public int UserId
        {
            get { return this.userId; }
            set { this.userId = value; OnPropertyChanged(nameof(UserId)); }
        }
        private string login;
        public string Login
        {
            get { return this.login; }
            set { this.login = value; OnPropertyChanged(nameof(Login)); }
        }
        private string password;
        public string Password
        {
            get { return this.password; }
            set { this.password = value; OnPropertyChanged(nameof(Password)); }
        }
        private string roleName;
        public string RoleName
        {
            get { return this.roleName; }
            set { this.roleName = value; OnPropertyChanged(nameof(RoleName)); }
        }
    }
}
