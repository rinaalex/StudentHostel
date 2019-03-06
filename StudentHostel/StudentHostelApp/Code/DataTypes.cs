using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Code
{
    /// <summary>
    /// Предоставляет составные типы данных
    /// </summary>
    static public class DataTypes
    {
        /// <summary>
        /// Тип учетной записи пользователей
        /// </summary>
        public enum Role
        {
            [Display(Name = "Администратор")]
            Admin = 0,
            [Display(Name = "Пользователь")]
            User = 1
        }
    }
}
