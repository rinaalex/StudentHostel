using System.Linq;
using System.Collections.ObjectModel;
using StudentHostelApp.Code;
using StudentHostelApp.Commands;
using StudentHostelApp.ViewModel.SingleEntityVM;
using StudentHostelApp.Model;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет CRUD-функции для работы со списком комнат
    /// </summary>
    public class RoomsListViewModel: BaseCrudViewModel
    {
        public ObservableCollection<RoomViewModel> RoomsList { get; set; }

        private RoomViewModel currentRoom;
        public RoomViewModel CurrentRoom
        {
            get { return this.currentRoom; }
            set
            {
                this.currentRoom = value;
                OnPropertyChanged(nameof(CurrentRoom));
                OnPropertyChanged(nameof(StudentsInRoomList));
            }
        }

        // Список студентов, проживающих в комнате на текущий момент
        private ObservableCollection<StudentViewModel> studentsInRoomList;
        public ObservableCollection<StudentViewModel> StudentsInRoomList
        {
            get
            {
                if (CurrentRoom != null)
                {
                    var students = context.Accomodations.Where(p => p.Room.RoomId == CurrentRoom.RoomId && p.DateEnd==null).
                        Select(q => new StudentViewModel
                        {
                            StudentId = q.Student.StudentId,
                            Name = q.Student.Name
                        }).ToList();
                    studentsInRoomList= new ObservableCollection<StudentViewModel>(students);
                    return studentsInRoomList;
                }
                else
                    return null;
            }
        }

        public RoomsListViewModel()
        {
            GetData();

            // Инициализация команд
            AddCommand = new Command(Add, () => { return !(IsAdding || IsEditing); });
            EditCommand=new Command(Edit, () => { return !(IsAdding || IsEditing); });
            SaveCommand = new Command(SaveChanges, () => { return IsAdding || IsEditing; });
            CancelCommand = new Command(DiscardChanges, () => { return IsAdding || IsEditing; });
            DeleteCommand = new Command(Delete, () => { return !(IsAdding || IsEditing) && canDelete; });
        }

        protected override void GetData()
        {
            var rooms = context.Rooms.Where(q=>!q.SoftDeleted).Select(p => new RoomViewModel
            {
                RoomId = p.RoomId,
                RoomNo = p.RoomNumber,
                Seats = p.Seats
            }).ToList();

            RoomsList = new ObservableCollection<RoomViewModel>(rooms);

            // Определение количества свободных мест в комнате
            foreach (RoomViewModel room in RoomsList)
            {
                var count = context.Accomodations.Where(p => p.Room.RoomId == room.RoomId && p.DateEnd==null).Count();
                room.FreeSeats = room.Seats - count;
            }
        }

        protected override void Add()
        {
            CurrentRoom = new RoomViewModel
            {
                RoomId = 0,
                Seats=0,
                FreeSeats=0
            };
            RoomsList.Add(CurrentRoom);
            IsAdding = true;
        }

        protected bool Validate(RoomViewModel room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomNo))
            {
                ErrorMessage = "Поле Номер комнаты не может быть пустым!";
                return false;
            }
            if (IsAdding)
            {                
                if (room.Seats <= 0)
                {
                    ErrorMessage = "Поле Вместимость должно иметь положительное значение!";
                    return false;
                }
            }
            if (IsEditing)
            {
                if(room.Seats<oldRoom.Seats-oldRoom.FreeSeats)
                {
                    ErrorMessage = "Вместимость не может быть меньше текущего количества проживающих!";
                    return false;
                }
            }
            return true;
        }

        private RoomViewModel oldRoom;

        protected override void Edit()
        {
            oldRoom = new RoomViewModel
            {
                RoomId = CurrentRoom.RoomId,
                RoomNo = CurrentRoom.RoomNo,
                Seats = CurrentRoom.Seats
            };
            IsEditing = true;
        }

        protected override void SaveChanges()
        {
            if (Validate(CurrentRoom))
            {
                if (IsAdding)
                {
                    Room room = new Room
                    {
                        RoomId = 0,
                        RoomNumber = CurrentRoom.RoomNo,
                        Seats = CurrentRoom.Seats
                    };
                    context.Rooms.Add(room);
                    context.SaveChanges();
                    CurrentRoom.FreeSeats = CurrentRoom.Seats;
                    IsAdding = false;
                    ErrorMessage = string.Empty;
                }
                else if (IsEditing)
                {
                    var room = context.Rooms.Where(p => p.RoomId == CurrentRoom.RoomId).FirstOrDefault();
                    room.RoomNumber = CurrentRoom.RoomNo;
                    room.Seats = CurrentRoom.Seats;
                    context.SaveChanges();
                    var count = context.Accomodations.Where(p => p.Room.RoomId == CurrentRoom.RoomId && p.DateEnd == null).Count();
                    CurrentRoom.FreeSeats = CurrentRoom.Seats - count;
                    IsEditing = false;
                    ErrorMessage = string.Empty;
                }
            }
        }
        protected override void DiscardChanges()
        {
            if(IsAdding)
            {
                RoomsList.Remove(CurrentRoom);
                IsAdding = false;
                ErrorMessage = string.Empty;
            }
            else if(IsEditing)
            {
                CurrentRoom.RoomNo = oldRoom.RoomNo;
                CurrentRoom.Seats = oldRoom.Seats;
                CurrentRoom.FreeSeats = oldRoom.Seats;
                IsEditing = false;
                ErrorMessage = string.Empty;
            }
        }

        /// <summary>
        /// Определяет, можно ли удалить текущий объект
        /// </summary>
        private bool canDelete
        {
            get
            {
                if (CurrentRoom != null)
                {
                    if (CurrentRoom.FreeSeats == CurrentRoom.Seats)
                    {
                        return true;
                    }
                    else
                    {
                        // Невозможно удалить информацию о комнате, если в ней проживают студенты
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        protected override void Delete()
        {
            var room = context.Rooms.Where(p => p.RoomId == CurrentRoom.RoomId).SingleOrDefault();
            // Помечаем объект как удаленный
            room.SoftDeleted = true;
            context.SaveChanges();
            RoomsList.Remove(CurrentRoom);
        }
    }
}
