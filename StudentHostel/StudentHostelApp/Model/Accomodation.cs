using System;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
    public class Accomodation
    {
        public int AccomodationId { get; set; }
        //[Required]
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        [Required]
        public Student Student { get; set; }
        [Required]
        public Room Room { get; set; }
    }
}
