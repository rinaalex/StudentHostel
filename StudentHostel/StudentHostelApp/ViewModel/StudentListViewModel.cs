using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using StudentHostelApp.DataAccess;
using StudentHostelApp.Model;
using StudentHostelApp.Commands;
using System.ComponentModel;
using System.Data.Entity;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет возможности для работы с таблицей Студенты
    /// </summary>
    public class StudentListViewModel : INotifyPropertyChanged
    {
        StudentHostelContext context = new StudentHostelContext();

        public ObservableCollection<StudentViewModel> StudentList { get; private set; }
        public ObservableCollection<Group> GroupList { get; private set; }

        // Текущий объект из коллекции
        private StudentViewModel currentStudent;
        public StudentViewModel CurrentStudent
        {
            get
            {
                if (StudentList!=null)
                {
                    return this.currentStudent;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.currentStudent = value;
                OnPropertyChanged(nameof(CurrentStudent));
            }
        }

        // Команды для взаимодействия с коллекцией объектов
        public Command AddCommand { get; private set; }
        public Command EditCommand { get; private set; }
        public Command SaveCommand { get; private set; }
        public Command DeleteCommand { get; private set; }
        public Command CancelCommand { get; private set; }

        // Конструктор представления модели
        public StudentListViewModel()
        {
            this.GetData();

            if (StudentList.Count == 0)
                CurrentStudent = null;
            else
                CurrentStudent = StudentList.First();

            // Инициализация команд
            AddCommand = new Command(Add, () => { return !(IsAdding || IsEditing); });
            EditCommand = new Command(Edit, () => { return !(IsAdding || IsEditing); });
            SaveCommand = new Command(SaveChanges, () => { return IsAdding || IsEditing; });
            DeleteCommand = new Command(DeleteStudent, () => { return !IsAdding && !IsEditing; });
            CancelCommand = new Command(DiscardChanges, () => { return IsAdding || IsEditing; });
        }

        /// <summary>
        /// Загружает список студентов для отображения и редактирования
        /// </summary>
        public void GetData()
        {
            // Загрузка необходимых для отображения данных из контекста
            var students = context.Students.Select(p => new
            {
                p.StudentId,
                p.Name,
                p.Phone,
                p.Description,
                p.Group.GroupName
            });

            StudentList = new ObservableCollection<StudentViewModel>();

            // Формирование списка для отображения
            foreach (var student in students)
            {
                StudentList.Add(new StudentViewModel
                {
                    StudentId = student.StudentId,
                    Name = student.Name,
                    Phone = student.Phone,
                    Description = student.Description,
                    GroupName = student.GroupName
                });
            }

            // Загрузка из контекста необходимых для отображения данных
            var groups = context.Groups.Select(p => new
            {
                p.GroupId,
                p.GroupName
            });

            GroupList = new ObservableCollection<Group>();

            // Формирование списка для отображения
            foreach (var group in groups)
            {
                GroupList.Add(new Group
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName
                });
            }
        }

        #region Свойства для управления режимами работы с коллекцией
        private bool isAdding;
        public bool IsAdding
        {
            get { return this.isAdding; }
            set
            {
                this.isAdding = value;
                OnPropertyChanged(nameof(IsAdding));
                OnPropertyChanged(nameof(IsAddingOrEditing));
                OnPropertyChanged(nameof(IsBrowsing));
            }
        }

        private bool isEditing;
        public bool IsEditing
        {
            get
            {
                return this.isEditing;
            }
            set
            {
                this.isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
                OnPropertyChanged(nameof(IsAddingOrEditing));
                OnPropertyChanged(nameof(IsBrowsing));
            }
        }

        //private bool ;
        public bool IsAddingOrEditing
        {
            get
            {
                return IsAdding || IsEditing;
            }
        }

        public bool IsBrowsing
        {
            get
            {
                return !(IsAdding || IsEditing);
            }
        }

        //последнее сообщение об ошибке
        private string errorMessage;
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
            set
            {
                this.errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        #endregion

        #region Методы, определяющие редактирование коллекции
        /// <summary>
        /// Выполняет переход в режим добавления нового объекта, втавляет новый объект в коллекцию
        /// </summary>
        protected void Add()
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
        protected void Edit()
        {
            if (CurrentStudent != null)
            {
                oldStudent = new StudentViewModel();
                oldStudent.StudentId = CurrentStudent.StudentId;
                oldStudent.Name = CurrentStudent.Name;
                oldStudent.Description = CurrentStudent.Description;
                oldStudent.Phone = CurrentStudent.Phone;
                oldStudent.GroupName = CurrentStudent.GroupName;

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
            if (String.IsNullOrWhiteSpace(student.Name))
            {
                ErrorMessage = "Поле ФИО не может быть пустым!";
                return false;
            }
            if (String.IsNullOrWhiteSpace(student.Phone))
            {
                ErrorMessage = "Поле Телефон не может быть пустым!";
                return false;
            }
            if (String.IsNullOrWhiteSpace(student.GroupName))
            {
                ErrorMessage = "Поле Учебная группа не может быть пустым!";
                return false;
            }

            ErrorMessage = String.Empty;
            return true;
        }

        /// <summary>
        /// Сохраняет изменения в текущем объекте при его добавлении или редактировании
        /// </summary>
        protected void SaveChanges()
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
                    context.Students.Add(student);
                    context.SaveChanges();
                    CurrentStudent.StudentId = context.Students.OrderByDescending(p => p.StudentId).FirstOrDefault().StudentId;
                    OnPropertyChanged(nameof(CurrentStudent));
                    IsAdding = false;
                }
                // Сохранение изменений в существующум объекте
                else
                {
                    var result = context.Students.Where(p => p.StudentId == CurrentStudent.StudentId).FirstOrDefault();
                    result.Name = student.Name;
                    result.Phone = student.Phone;
                    result.Description = student.Description;
                    result.Group = student.Group;
                    context.SaveChanges();
                    IsEditing = false;
                }
            }
        }

        /// <summary>
        /// Удаляет текущий объект из коллекции
        /// </summary>
        protected void DeleteStudent()
        {
            if (CurrentStudent != null)
            {
                // Создание объекта для удаления из контекста данных
                Student newStudent = new Student
                {
                    StudentId = CurrentStudent.StudentId,
                    Name = CurrentStudent.Name,
                    Phone = CurrentStudent.Phone,
                    Description = CurrentStudent.Description,
                    Group = context.Groups.Where(p => p.GroupName == CurrentStudent.GroupName).Single()
                };

                // Удаление объекта из контекста и коллекции
                context.Entry(newStudent).State = EntityState.Deleted;
                StudentList.Remove(CurrentStudent);
                context.SaveChanges();
                ErrorMessage = String.Empty;
            }
            else
                ErrorMessage = "Не выбран объект для удаления.";
        }

        /// <summary>
        /// Выполняет отмену изменений в текущем объекте
        /// </summary>
        protected void DiscardChanges()
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
            ErrorMessage = String.Empty;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
