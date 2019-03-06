using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using StudentHostelApp.Code;

namespace StudentHostelApp.Model
{
    /// <summary>
    /// Предоставляет информацию об учетной записи пользователя
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DataTypes.Role Role { get; set; }
    }
}
