using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
    /// <summary>
    /// Таблица Группы
    /// </summary>
    public class Group
    {
        public int GroupId { get; set; }
        [MaxLength(10)]
        public string GroupName { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}
