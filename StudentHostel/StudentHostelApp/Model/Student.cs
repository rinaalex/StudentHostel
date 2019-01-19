using System;
using System.ComponentModel.DataAnnotations;


namespace StudentHostelApp.Model
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        [StringLength(11)]
        public string Phone { get; set; }
        public string Description { get; set; }
        public Group Group { get; set; }
    }
}
