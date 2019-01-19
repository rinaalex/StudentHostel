using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
    public class Group
    {
        public int GroupId { get; set; }
        [MaxLength(10)]
        public string Name { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
