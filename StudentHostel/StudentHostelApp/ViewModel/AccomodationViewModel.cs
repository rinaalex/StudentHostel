using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHostelApp.ViewModel
{
    public class AccomodationViewModel
    {
        public int AccomodationId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string RoomNo { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
