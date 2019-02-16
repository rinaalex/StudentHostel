using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Entity;
using StudentHostelApp.Model;
using StudentHostelApp.Commands;
using StudentHostelApp.Code;
using StudentHostelApp.ViewModel.SingleEntityVM;
using StudentHostelApp.DataAccess;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет CRUD-функции для работы со списком размещений
    /// </summary>
    class AccomodationListViewModel: BaseCrudViewModel
    {
        public ObservableCollection<AccomodationViewModel> AccomodationList { get; set; }

        // Список доступных для заселения комнат
        public ObservableCollection<RoomViewModel> RoomsList { get; private set; }

        // Список незаселенных студентов
        public ObservableCollection<StudentViewModel> StudentsList { get;  private set; }

        private AccomodationViewModel currentAccomodation;
        public AccomodationViewModel CurrentAccomodation
        {
            get
            {
                return this.currentAccomodation;
            }
            set
            {
                this.currentAccomodation = value;
                OnPropertyChanged(nameof(CurrentAccomodation));
            }
        }

        /// <summary>
        /// Определяет, доступна ли запись для редактирования
        /// </summary>     
        private bool isEditable
        {
            get
            {
                if (this.CurrentAccomodation == null)
                    return false;
                if (this.CurrentAccomodation.DateEnd == null)
                    return true;
                return false;
            }
        }

        public AccomodationListViewModel(StudentHostelContext context):base(context)
        {
            GetData();
            GetStudentsList();
            GetRoomsList();

            if (AccomodationList.Count==0)
            {
                CurrentAccomodation = null;
            }
            else
            {
                CurrentAccomodation = AccomodationList.First(); 
            }

            // Инициализация команд
            AddCommand = new Command(Add, () => { return !(IsAdding || IsEditing) && context != null; });
            SaveCommand = new Command(SaveChanges, () => { return (IsAdding || IsEditing) && context != null; });
            CancelCommand = new Command(DiscardChanges, () => { return (IsAdding || IsEditing) && context != null; });
            EditCommand = new Command(Edit, () => { return !(IsAdding || IsEditing)&&isEditable && context != null; });
        }

        protected override void GetData()
        {
            try
            {
                // Загрузка списка размещений
                var accomodations = context.Accomodations.Select(p => new AccomodationViewModel
                {
                    AccomodationId = p.AccomodationId,
                    StudentId = p.Student.StudentId,
                    StudentName = p.Student.Name,
                    RoomId = p.Room.RoomId,
                    RoomNo = p.Room.RoomNumber,
                    DateStart = p.DateStart,
                    DateEnd = p.DateEnd
                }).ToList();

                AccomodationList = new ObservableCollection<AccomodationViewModel>(accomodations);
            }
            catch (Exception e)
            {
                ErrorMessage = "Невозможно загрузить данные!";
#if DEBUG
                ErrorMessage = e.Message;
#endif
            }
        }

        /// <summary>
        /// Выполняет загрузку списка незаселенных студентов
        /// </summary>
        private void GetStudentsList()
        {
            try
            {
                var students = context.Students.Where(s => !s.SoftDeleted).Select(p => new
                {
                    p.StudentId,
                    p.Name,
                    p.RoomsLink
                }).Where(q => q.RoomsLink.Where(c => c.DateEnd == null).Count() == 0);

                StudentsList = new ObservableCollection<StudentViewModel>();

                foreach (var student in students)
                {
                    StudentsList.Add(new StudentViewModel
                    {
                        StudentId = student.StudentId,
                        Name = student.Name
                    });
                }
                OnPropertyChanged(nameof(StudentsList));
            }
            catch (Exception e)
            {
                ErrorMessage = "Невозможно загрузить данные!";
#if DEBUG
                ErrorMessage = e.Message;
#endif
            }
        }

        /// <summary>
        /// Выполняет загрузку списка комнат, доступных для заселения
        /// </summary>
        private void GetRoomsList()
        {
            try
            {
                var rooms = context.Set<Room>().Where(s => !s.SoftDeleted).Select(p => new
                {
                    p.RoomId,
                    p.RoomNumber,
                    p.Seats,
                    p.StudentsLink
                }).Where(q => q.StudentsLink.Where(c => c.DateEnd == null).Count() < q.Seats);

                RoomsList = new ObservableCollection<RoomViewModel>();

                foreach (var room in rooms)
                {
                    RoomsList.Add(new RoomViewModel
                    {
                        RoomId = room.RoomId,
                        RoomNo = room.RoomNumber
                    });
                }
                OnPropertyChanged(nameof(RoomsList));
            }
            catch (Exception e)
            {
                ErrorMessage = "Невозможно загрузить данные!";
#if DEBUG
                ErrorMessage = e.Message;
#endif
            }
        }

        protected override void Add()
        {
            AccomodationList.Add(new AccomodationViewModel
            {
                AccomodationId =0,
                DateStart=DateTime.Now
            });
            CurrentAccomodation = AccomodationList.Last();
            IsAdding = true;
        }

        private AccomodationViewModel oldAccomodation;

        protected override void Edit()
        {
            if (CurrentAccomodation != null)
            {
                CurrentAccomodation.DateEnd = DateTime.Now;

                oldAccomodation = new AccomodationViewModel
                {
                    AccomodationId = CurrentAccomodation.AccomodationId,
                    StudentId = CurrentAccomodation.StudentId,
                    StudentName = CurrentAccomodation.StudentName,
                    RoomId=CurrentAccomodation.RoomId,
                    RoomNo = CurrentAccomodation.RoomNo,
                    DateStart = CurrentAccomodation.DateStart,
                    DateEnd = CurrentAccomodation.DateEnd
                };
                IsEditing = true;
                ErrorMessage = string.Empty;
            }
            else
            {
                ErrorMessage = "Не выбран объект для редактирования!";
            }
        }

        protected bool Validate(AccomodationViewModel accomodation)
        {
            // При добавлении новой записи 
            if (IsAdding)
            {
                // Проверяем корректность заполнения полей
                if (accomodation.StudentId == 0)
                {
                    ErrorMessage = "Поле Студент не может быть пустым!";
                    return false;
                }
                else if (accomodation.RoomId == 0)
                {
                    ErrorMessage = "Поле Номер комнаты не может быть пустым!";
                    return false;
                }

                else if (accomodation.DateStart == null)
                {
                    ErrorMessage = "Поле Дата заселения не может быть пустым!";
                    return false;
                }

                // Проверяем, нет ли незавершенных записей для этого студента
                var acc = context.Accomodations.Where(
                    p => p.Student.StudentId == CurrentAccomodation.StudentId &&
                    p.DateEnd == null).FirstOrDefault();
                if (acc != null)
                {
                    ErrorMessage = "Студент еще не выселен!";
                    return false;
                }

                // Проверяем, есть ли свободные места в комнате
                var count = context.Rooms.Where(
                    p => p.RoomId == CurrentAccomodation.RoomId).
                    Select(p => p.Seats).SingleOrDefault();
                var studCount = context.Accomodations.
                    Where(p => p.Room.RoomId == CurrentAccomodation.RoomId &&
                    p.DateEnd==null).Count();

                if (count == studCount)
                {
                    ErrorMessage = "В этой комнате нет свободных мест!";
                    return false;
                }
            }

            // При редактировании записи 
            if (IsEditing)
            {
                // Проверяем, заполнена ли дата выселения
                if (accomodation.DateEnd == null)
                {
                    ErrorMessage = "Поле Дата выселения не может быть пустым!";
                    return false;
                }
            }

            ErrorMessage = string.Empty;
            return true;            
        }

        protected override void SaveChanges()
        {
            if (Validate(CurrentAccomodation))
            {
                if (IsAdding)
                {
                    try
                    {
                        var student = context.Students.Single(p => p.StudentId == CurrentAccomodation.StudentId);
                        var room = context.Rooms.Single(p => p.RoomId == CurrentAccomodation.RoomId);
                        student.RoomsLink.Add(

                            new Accomodation
                            {
                                Student = student,
                                Room = room,
                                DateStart = CurrentAccomodation.DateStart
                            }
                        );
                        context.SaveChanges();
                        IsAdding = false;

                        //!!!нужно подправить это
                        CurrentAccomodation.AccomodationId = student.RoomsLink.Where(p => p.Student.StudentId == CurrentAccomodation.StudentId).Select(p => p.AccomodationId).LastOrDefault();
                        CurrentAccomodation.StudentName = student.Name;
                        CurrentAccomodation.RoomNo = student.RoomsLink.Where(p => p.AccomodationId == CurrentAccomodation.AccomodationId).Select(p => p.Room.RoomNumber).LastOrDefault();
                    }
                    catch (Exception e)
                    {
                        ErrorMessage = "Невозможно выполнить операцию!";
#if DEBUG
                        ErrorMessage = e.Message;
#endif
                    }
                }
                else if(IsEditing)
                {
                    try
                    {
                        var acc = context.Accomodations.Where(p => p.AccomodationId == CurrentAccomodation.AccomodationId).SingleOrDefault();
                        acc.DateEnd = CurrentAccomodation.DateEnd;
                        context.SaveChanges();
                        IsEditing = false;
                    }
                    catch (Exception e)
                    {
                        ErrorMessage = "Невозможно загрузить данные!";
#if DEBUG
                        ErrorMessage = e.Message;
#endif
                    }
                }
                GetRoomsList();
                GetStudentsList();
            }
        }

        protected override void DiscardChanges()
        {
            if(IsAdding)
            {
                AccomodationList.Remove(CurrentAccomodation);
                IsAdding = false;
                ErrorMessage = string.Empty;
            }
            else if (IsEditing)
            {
                IsEditing = false;
                CurrentAccomodation.DateEnd = null;
                ErrorMessage = string.Empty;
            }
        }
    }
}
