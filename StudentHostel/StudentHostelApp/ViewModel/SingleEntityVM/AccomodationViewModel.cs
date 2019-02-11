using System;
using StudentHostelApp.Code;

namespace StudentHostelApp.ViewModel.SingleEntityVM
{
    /// <summary>
    /// Предоставляет информацию о размещении для отображения
    /// </summary>
    public class AccomodationViewModel :BaseViewModel
    {
        private int accomodationId;
        public int AccomodationId
        {
            get { return this.accomodationId; }
            set { this.accomodationId = value; OnPropertyChanged(nameof(AccomodationId)); }
        }
        private int studentId;
        public int StudentId
        {
            get { return this.studentId; }
            set { this.studentId = value; OnPropertyChanged(nameof(StudentId)); }
        }
        private string studentName;
        public string StudentName
        {
            get { return this.studentName; }
            set { this.studentName = value; OnPropertyChanged(nameof(StudentName)); }
        }
        private int roomId;
        public int RoomId
        {
            get { return this.roomId; }
            set { this.roomId = value; OnPropertyChanged(nameof(RoomId)); }
        }
        private string roomNo;
        public string RoomNo
        {
            get { return this.roomNo; }
            set { this.roomNo = value; OnPropertyChanged(nameof(RoomNo)); }
        }
        private DateTime dateStart;
        public DateTime DateStart
        {
            get { return this.dateStart; }
            set { this.dateStart = value; OnPropertyChanged(nameof(DateStart)); }
        }
        private DateTime? dateEnd;
        public DateTime? DateEnd { get { return dateEnd; } set { this.dateEnd = value; OnPropertyChanged(nameof(DateEnd)); } }
    }
}
