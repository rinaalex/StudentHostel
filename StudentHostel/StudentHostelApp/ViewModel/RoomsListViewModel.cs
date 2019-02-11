using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using StudentHostelApp.DataAccess;
using StudentHostelApp.Code;
using StudentHostelApp.ViewModel.SingleEntityVM;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет CRUD-функции для работы со списком комнат
    /// </summary>
    public class RoomsListViewModel: BaseCrudViewModel
    {
        public ObservableCollection<RoomViewModel> RoomsList { get; set; }

        public RoomViewModel CurrentRoom { get; set; }

        private ObservableCollection<StudentViewModel> studentsInRoomList;
        public ObservableCollection<StudentViewModel> StudentsInRoomList
        {
            get
            {
                if (CurrentRoom != null)
                {
                    var students = context.Accomodations.Where(p => p.Room.RoomId == CurrentRoom.RoomId).
                        Select(q => new StudentViewModel
                        {
                            StudentId = q.Student.StudentId,
                            Name = q.Student.Name
                        }).ToList();
                    studentsInRoomList= new ObservableCollection<StudentViewModel>(students);
                    return studentsInRoomList;
                }
                else return null;
            }
        }

        public RoomsListViewModel()
        {
            GetData();
        }

        protected override void GetData()
        {
            var rooms = context.Rooms.Select(p => new RoomViewModel
            {
                RoomId = p.RoomId,
                RoomNo = p.RoomNumber,
                Seats = p.Seats
            }).ToList();

            RoomsList = new ObservableCollection<RoomViewModel>(rooms);

            foreach (RoomViewModel room in RoomsList)
            {
                var count = context.Accomodations.Where(p => p.Room.RoomId == room.RoomId && p.DateEnd==null).Count();
                room.FreeSeats = room.Seats - count;
            }
        }
    }
}
