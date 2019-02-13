using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
    /// <summary>
    /// Предоставляет информацию об учебной группе
    /// </summary>
    public class Group
    {
        public int GroupId { get; set; }
        [Required]
        [MaxLength(10)]
        public string GroupName { get; set; }

        public bool SoftDeleted { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}
