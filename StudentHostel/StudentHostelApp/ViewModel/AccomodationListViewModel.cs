using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlTypes;
using StudentHostelApp.Model;
using StudentHostelApp.Commands;

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

            //
            AddCommand = new Command(Add, () => { return !(IsAdding || IsEditing); });
            SaveCommand = new Command(SaveChanges, () => { return IsAdding || IsEditing; });
        }

        protected override void GetData()
        {
            var accomodations = context.Accomodations.Select(p => new AccomodationViewModel
            {
                AccomodationId=p.AccomodationId,
                StudentName=p.Student.Name,
                RoomNo=p.Room.RoomNumber,
                DateStart=p.Start,
                DateEnd=p.End
            }).ToList();

            AccomodationList = new ObservableCollection<AccomodationViewModel>(accomodations);
        }

        protected override void Add()
        {
            AccomodationList.Add(new AccomodationViewModel { AccomodationId=0 });
            CurrentAccomodation = AccomodationList.Last();
            IsAdding = true;
        }

        private AccomodationViewModel oldAccomodation;

        protected override void Edit()
        {
            if (CurrentAccomodation != null)
            {
                oldAccomodation = new AccomodationViewModel
                {
                    AccomodationId = CurrentAccomodation.AccomodationId,
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
            if (string.IsNullOrEmpty(accomodation.StudentName))
            {
                ErrorMessage = "Поле Студент не может быть пустым!";
                return false;
            }
            else if (string.IsNullOrEmpty(accomodation.RoomNo))
            {
                ErrorMessage = "Поле Номер комнаты не может быть пустым!";
                return false;
            }
            //else if(accomodation.DateStart==null)
            //{
            //    ErrorMessage = "Поле Дата заселения не может быть пустым!";
            //    return false;
            //}
            else
                return true;
        }

        protected override void SaveChanges()
        {
            if (Validate(CurrentAccomodation))
            {
                if (IsAdding)
                {
                    //Accomodation accomodation = new Accomodation
                    //{
                    //    AccomodationId = 0,
                    //    Student = context.Students.Where(p => p.Name == CurrentAccomodation.StudentName).FirstOrDefault(),
                    //    Room = context.Set<Room>().Where(p=>p.RoomNumber==CurrentAccomodation.RoomNo).FirstOrDefault(),
                    //    Start = DateTime.Today,//CurrentAccomodation.DateStart,
                    //    End = CurrentAccomodation.DateEnd
                    //};

                    var student = context.Students.Single(p => p.Name == CurrentAccomodation.StudentName);
                    var room = context.Rooms.Single(p => p.RoomNumber == CurrentAccomodation.RoomNo);
                    student.RoomsLink = new List<Accomodation>
                    {
                        new Accomodation
                        {
                            Student=student,
                            Room=room,
                            Start=DateTime.Now
                        }
                    };
                    //context.Accomodations.Add(accomodation);
                    context.SaveChanges();
                }
                else if(IsEditing)
                {
                    
                }
            }
        }
    }
}
