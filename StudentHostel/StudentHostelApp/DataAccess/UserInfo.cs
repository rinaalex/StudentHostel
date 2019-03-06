using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using StudentHostelApp.ViewModel.SingleEntityVM;

namespace StudentHostelApp.DataAccess
{
    /// <summary>
    /// Предоставляет информацию об авторизованном пользователе
    /// </summary>
    static public class UserInfo
    {
        static public UserViewModel CurrentUser { get; set; } = new UserViewModel();
    }
}
