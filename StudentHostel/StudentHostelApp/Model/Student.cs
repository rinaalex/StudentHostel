﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace StudentHostelApp.Model
{
    /// <summary>
    /// Таблица Студенты
    /// </summary>
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(11)]
        public string Phone { get; set; }
        public string Description { get; set; }
        public virtual Group Group { get; set; }

        public virtual ICollection<Accomodation> RoomsLink { get; set; }
    }
}