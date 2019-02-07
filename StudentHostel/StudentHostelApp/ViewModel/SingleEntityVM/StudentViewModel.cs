using System.ComponentModel;

namespace StudentHostelApp.ViewModel.SingleEntityVM
{
    /// <summary>
    /// Предоставляет информацию о студенте для отображения
    /// </summary>
    public class StudentViewModel: BaseViewModel
    {
        private int studentId;
        public int StudentId
        {
            get { return this.studentId; }
            set { this.studentId = value; OnPropertyChanged(nameof(StudentId)); }
        }
        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; OnPropertyChanged(nameof(Name)); }
        }
        private string phone;
        public string Phone
        {
            get { return this.phone; }
            set { this.phone = value; OnPropertyChanged(nameof(Phone)); }
        }
        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.description = value; OnPropertyChanged(nameof(Description)); }
        }
        private string groupName;
        public string GroupName
        {
            get { return this.groupName; }
            set { this.groupName = value; OnPropertyChanged(nameof(GroupName)); }
        }
        public string roomNo;
        public string RoomNo
        {
            get { return this.roomNo; }
            set { this.roomNo = value; OnPropertyChanged(nameof(RoomNo)); }
        }
    }
}
