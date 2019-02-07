using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StudentHostelApp.Model
{
    public class Accomodation/*: INotifyPropertyChanged*/
    {
        public int AccomodationId { get; set; }
        
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        [Required]
        public Student Student { get; set; }
        [Required]
        public Room Room { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(string property)
        //{
        //    if(PropertyChanged!=null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(property));
        //    }
        //}
    }
}
