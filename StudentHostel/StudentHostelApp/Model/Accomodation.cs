using System;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
    /// <summary>
    /// Предоставляет информацию о размещении студента в комнате
    /// </summary>
    public class Accomodation
    {
        public int AccomodationId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        [Required]
        public virtual Student Student { get; set; }
        [Required]
        public virtual Room Room { get; set; }
    }
}
