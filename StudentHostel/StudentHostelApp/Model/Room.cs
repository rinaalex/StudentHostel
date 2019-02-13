using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
    /// <summary>
    /// Предоставляет информацию о комнате
    /// </summary>
    public class Room
    {
        public int RoomId { get; set; }
        [Required]
        [MaxLength(5)]
        public string RoomNumber { get; set; }
        public byte Seats { get; set; }
        public bool SoftDeleted { get; set; }

        public virtual ICollection<Accomodation> StudentsLink { get; set; }
    }
}
