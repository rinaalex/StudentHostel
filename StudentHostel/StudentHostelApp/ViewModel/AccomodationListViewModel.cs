using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlTypes;
using StudentHostelApp.Model;
using StudentHostelApp.Commands;
using StudentHostelApp.ViewModel.SingleEntityVM;

namespace StudentHostelApp.ViewModel
{
    class AccomodationListViewModel: BaseCrudViewModel
    {
        public ObservableCollection<AccomodationViewModel> AccomodationList { get; set; }

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

        public AccomodationListViewModel()
        {
            GetData();
            if(AccomodationList.Count!=0)
            {
                CurrentAccomodation = AccomodationList.First();
            }
            else
            {
                CurrentAccomodation = null;
            }

            // Инициализация команд
            AddCommand = new Command(Add, () => { return !(IsAdding || IsEditing); });
            SaveCommand = new Command(SaveChanges, () => { return IsAdding || IsEditing; });
            CancelCommand = new Command(DiscardChanges, () => { return IsAdding || IsEditing; });
            EditCommand = new Command(Edit, () => { return !(IsAdding || IsEditing)&&(CurrentAccomodation.DateEnd==null); });
        }

        protected override void GetData()
        {
            var accomodations = context.Accomodations.Select(p => new AccomodationViewModel
            {
                AccomodationId=p.AccomodationId,
                StudentId=p.Student.StudentId,
                StudentName=p.Student.Name,
                RoomNo=p.Room.RoomNumber,
                DateStart=p.DateStart,
                DateEnd=p.DateEnd
            }).ToList();

            AccomodationList = new ObservableCollection<AccomodationViewModel>(accomodations);
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
                else if (string.IsNullOrEmpty(accomodation.RoomNo))
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
                    p => p.RoomNumber == CurrentAccomodation.RoomNo).
                    Select(p => p.Seats).SingleOrDefault();
                var studCount = context.Accomodations.
                    Where(p => p.Room.RoomNumber == CurrentAccomodation.RoomNo &&
                    p.DateEnd!=null).Count();

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
                    var student = context.Students.Single(p => p.StudentId == CurrentAccomodation.StudentId);
                    var room = context.Rooms.Single(p => p.RoomNumber == CurrentAccomodation.RoomNo);
                    student.RoomsLink = new List<Accomodation>
                    {
                        new Accomodation
                        {
                            Student=student,
                            Room=room,
                            DateStart=CurrentAccomodation.DateStart
                        }
                    };
                    context.SaveChanges();
                    IsAdding = false;

                    CurrentAccomodation.AccomodationId = student.RoomsLink.Where(p => p.Student.StudentId == CurrentAccomodation.StudentId).Select(p => p.AccomodationId).LastOrDefault();
                    CurrentAccomodation.StudentName = student.Name;
                }
                else if(IsEditing)
                {
                    var acc = context.Accomodations.Where(p => p.AccomodationId == CurrentAccomodation.AccomodationId).SingleOrDefault();
                    acc.DateEnd = CurrentAccomodation.DateEnd;
                    context.SaveChanges();
                    IsEditing = false;
                }
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
