using System;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
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
