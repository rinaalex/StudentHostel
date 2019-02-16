using System;
using System.Linq;
using System.Collections.ObjectModel;
using StudentHostelApp.Model;
using StudentHostelApp.Commands;
using System.Data.Entity.Infrastructure;
using StudentHostelApp.Code;
using StudentHostelApp.ViewModel.SingleEntityVM;
using StudentHostelApp.DataAccess;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет CRUD-функции для работы со списком студентов
    /// </summary>
    public class StudentListViewModel : BaseCrudViewModel
    {
        public ObservableCollection<StudentViewModel> StudentList { get; private set; }
        public ObservableCollection<Group> GroupList { get; private set; }

        // Текущий объект из коллекции
        private StudentViewModel currentStudent;
        public StudentViewModel CurrentStudent
        {
            get
            {
                return this.currentStudent;
            }
            set
            {
                this.currentStudent = value;
                OnPropertyChanged(nameof(CurrentStudent));
            }
        }

        // Конструктор представления модели
        public StudentListViewModel(StudentHostelContext context):base(context)
        {
            GetData();
            GetStudentsList();

            if (StudentList.Count == 0)
            {
                CurrentStudent = null;
            }
            else
            {
                CurrentStudent = StudentList.First();
            }

            // Инициализация команд
            AddCommand = new Command(Add, () => { return !(IsAdding || IsEditing) && context != null; });
            EditCommand = new Command(Edit, () => { return !(IsAdding || IsEditing) && context != null; });
            SaveCommand = new Command(SaveChanges, () => { return (IsAdding || IsEditing) && context != null; });
            DeleteCommand = new Command(Delete, () => { return !(IsAdding && !IsEditing) && context != null; });
            CancelCommand = new Command(DiscardChanges, () => { return (IsAdding || IsEditing) && context != null; });
        }

        /// <summary>
        /// Загружает список студентов для отображения и редактирования
        /// </summary>
        protected override void GetData()
        {
            try
            {
                //Загрузка необходимых для отображения данных из контекста   
                var students = context.Students.Where(q => !q.SoftDeleted).Select(p => new StudentViewModel
                {
                    StudentId = p.StudentId,
                    Name = p.Name,
                    Phone = p.Phone,
                    Description = p.Description,
                    GroupName = p.Group.GroupName,
                    RoomNo = p.RoomsLink.Select(q => q.Room.RoomNumber).FirstOrDefault().ToString()
                }).ToList();

                StudentList = new ObservableCollection<StudentViewModel>(students);
            }
            catch (Exception e)
            {
                ErrorMessage = "Невозможно выполнить операцию!";
#if DEBUG
                ErrorMessage = e.Message;
#endif
            }
        }

        /// <summary>
        /// Выполняет загрузку списка учебных групп
        /// </summary>
        private void GetStudentsList()
        {
            try
            {
                var groups = context.Groups.Where(q => !q.SoftDeleted).Select(p => new
                {
                    p.GroupId,
                    p.GroupName
                }).ToList().Select(c => new Group
                {
                    GroupId = c.GroupId,
                    GroupName = c.GroupName
                }).ToList();

                GroupList = new ObservableCollection<Group>(groups);
            }
            catch(Exception e)
            {
                ErrorMessage = "Невозможно загрузить данные!";
#if DEBUG
                ErrorMessage = e.Message;
#endif
            }
        }

        #region Методы, определяющие редактирование коллекции
        /// <summary>
        /// Выполняет переход в режим добавления нового объекта, втавляет новый объект в коллекцию
        /// </summary>
        protected override void Add()
        {
            StudentViewModel newStudent = new StudentViewModel
            {
                StudentId = 0
            };

            StudentList.Add(newStudent);
            CurrentStudent = newStudent;
            IsAdding = true;
        }

        //копия исходного значения текущего объекта
        private StudentViewModel oldStudent;

        /// <summary>
        /// Выполняет перевод в режим редактирования текущего объекта
        /// </summary>
        protected override void Edit()
        {
            if (CurrentStudent != null)
            {
                oldStudent = new StudentViewModel
                {
                    StudentId = CurrentStudent.StudentId,
                    Name = CurrentStudent.Name,
                    Description = CurrentStudent.Description,
                    Phone = CurrentStudent.Phone,
                    GroupName = CurrentStudent.GroupName,
                    RoomNo = CurrentStudent.RoomNo
                };

                IsEditing = true;
                ErrorMessage = string.Empty;
            }
            else
                ErrorMessage = "Не выбран объект для редактирования.";
        }

        /// <summary>
        /// Выполняет проверку корректности значений полей объекта
        /// </summary>
        /// <param name="student">Проверяемый объект</param>
        /// <returns></returns>
        private bool Validate(StudentViewModel student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
            {
                ErrorMessage = "Поле ФИО не может быть пустым!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(student.Phone))
            {
                ErrorMessage = "Поле Телефон не может быть пустым!";
                return false;
            }
            if (student.Phone.Length>11)
            {
                ErrorMessage = "Поле Телефон не может содержать более 11 символов!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(student.GroupName))
            {
                ErrorMessage = "Поле Учебная группа не может быть пустым!";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Сохраняет изменения в текущем объекте при его добавлении или редактировании
        /// </summary>
        protected override void SaveChanges()
        {
            if (Validate(CurrentStudent))
            {
                // Создание объекта для добавления в контекст данных
                Student student = new Student
                {
                    StudentId = CurrentStudent.StudentId,
                    Name = CurrentStudent.Name,
                    Phone = CurrentStudent.Phone,
                    Description = CurrentStudent.Description,
                    Group = context.Groups.Where(p => p.GroupName == CurrentStudent.GroupName).Single()                  
                };

                // Сохранение нового объекта
                if (student.StudentId == 0)
                {
                    try
                    {
                        context.Students.Add(student);
                        context.SaveChanges();
                        CurrentStudent.StudentId = context.Students.OrderByDescending(p => p.StudentId).FirstOrDefault().StudentId;
                        //OnPropertyChanged(nameof(CurrentStudent));
                        IsAdding = false;
                    }
                    catch(DbUpdateException e)
                    {
                        ErrorMessage = "Невозможно выполнить операцию!";
#if DEBUG
                        ErrorMessage = e.Message;
#endif
                    }
                }
                // Сохранение изменений в существующум объекте
                else
                {
                    try
                    {
                        var result = context.Students.Where(p => p.StudentId == CurrentStudent.StudentId).FirstOrDefault();
                        result.Name = student.Name;
                        result.Phone = student.Phone;
                        result.Description = student.Description;
                        result.Group = student.Group;
                        context.SaveChanges();
                        IsEditing = false;
                    }
                    catch (DbUpdateException e)
                    {
                        ErrorMessage = "Невозможно выполнить операцию!";
#if DEBUG
                        ErrorMessage = e.Message;
#endif
                    }
                }
            }
        }

        protected override void Delete()
        {
            if (CurrentStudent != null)
            {
                try
                {
                    var student = context.Students.Where(p => p.StudentId == CurrentStudent.StudentId).SingleOrDefault();
                    // Помечаем объект как удаленный
                    student.SoftDeleted = true;

                    // Определяем, есть ли у удаляемого студента незавершенные размещения
                    var accomodation = context.Accomodations.Where(p => p.Student.StudentId == CurrentStudent.StudentId && p.DateEnd == null).SingleOrDefault();
                    if (accomodation != null)
                    {
                        // Завершаем размещение
                        accomodation.DateEnd = DateTime.Now;
                    }

                    StudentList.Remove(CurrentStudent);
                    context.SaveChanges();
                    ErrorMessage = string.Empty;
                }
                catch (DbUpdateException e)
                {
                    ErrorMessage = "Невозможно выполнить операцию!";
#if DEBUG
                    ErrorMessage = e.Message;
#endif
                }
            }
            else
                ErrorMessage = "Не выбран объект для удаления.";
        }

        /// <summary>
        /// Выполняет отмену изменений в текущем объекте
        /// </summary>
        protected override void DiscardChanges()
        {
            // Отмена режима добавления
            if (IsAdding)
            {
                StudentList.Remove(CurrentStudent);
                IsAdding = false;
            }
            // Отмена режима добавления
            else if (IsEditing)
            {
                CurrentStudent.StudentId = oldStudent.StudentId;
                CurrentStudent.Name = oldStudent.Name;
                CurrentStudent.Description = oldStudent.Description;
                CurrentStudent.Phone = oldStudent.Phone;
                CurrentStudent.GroupName = oldStudent.GroupName;
                OnPropertyChanged(nameof(CurrentStudent));
                IsEditing = false;
            }
            ErrorMessage = string.Empty;
        }
        #endregion
    }
}
