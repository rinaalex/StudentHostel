using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

using ClosedXML.Excel;

using StudentHostelApp.ViewModel.SingleEntityVM;
using StudentHostelApp.DataAccess;
using StudentHostelApp.Commands;
using StudentHostelApp.Model;


namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет возможность импорта списка студентов их файла Excel
    /// </summary>
    public class StudentsImportViewModel:INotifyPropertyChanged
    {
        // Список записей, загруженных из файла
        public ObservableCollection<StudentViewModel> StudentsList { get; set; }

        // Команды
        public Command GetStudentsCommand { get; set; }
        public Command SaveStudentsCommand { get; set; }

        private readonly StudentHostelContext context;

        public StudentsImportViewModel(StudentHostelContext context)
        {
            this.context = new StudentHostelContext();
            StudentsList = new ObservableCollection<StudentViewModel>();
            GetStudentsCommand = new Command(GetStudents, ()=> { return !string.IsNullOrEmpty(FileName); });
            SaveStudentsCommand = new Command(SaveStudents, () => { return StudentsList.Count != 0; });
        }

        /// <summary>
        /// Имя файла для импорта
        /// </summary>
        private string fileName;
        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; OnPropertyChanged(nameof(FileName)); }
        }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        private string errorMessage;
        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set { this.errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        /// <summary>
        /// Выполняет проверку корректности имени файла
        /// </summary>
        /// <returns></returns>
        private bool ValidateFileName()
        {
            if (File.Exists(FileName))
            {
                var extension = Path.GetExtension(FileName);
                if (extension == ".xls" || extension == ".xlsx")
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                else
                {
                    ErrorMessage = "Неверный тип файла!";
                    return false;
                }
            }
            else
            {
                ErrorMessage = "Неверное имя файла!";
                return false;
            }
        }

        /// <summary>
        /// Выпоняет загрузку списка студентов из файла
        /// </summary>
        public void GetStudents()
        {
            if (ValidateFileName())
            {
                try
                {
                    using (var workbook = new XLWorkbook(FileName))
                    {
                        var worksheet = workbook.Worksheets.Worksheet(1);
                        for (int row = 2; ; row++)
                        {
                            if (string.IsNullOrEmpty(worksheet.Cell(row, 1).GetValue<string>()))
                            {
                                break;
                            }
                            else
                            {
                                var student = new StudentViewModel
                                {
                                    StudentId = 0,
                                    Name = worksheet.Cell(row, 1).GetValue<string>(),
                                    Phone = worksheet.Cell(row, 2).GetValue<string>(),
                                    Description = worksheet.Cell(row, 3).GetValue<string>(),
                                    GroupName = worksheet.Cell(row, 4).GetValue<string>()
                                };
                                StudentsList.Add(student);
                            }
                        }
                    }
                    OnPropertyChanged(nameof(StudentsList));
                }
                catch(Exception)
                {
                    ErrorMessage = "Неверный формат файла!";
                }
                if (StudentsList.Count==0)
                {
                    ErrorMessage = "Неверный формат файла!";
                }
            }
        }
        
        /// <summary>
        /// Выполняет сохранение списка студентов в базу данных
        /// </summary>
        protected void SaveStudents()
        {
            try
            {
                foreach (StudentViewModel student in StudentsList)
                {
                    context.Students.Add(new Student
                    {
                        StudentId = 0,
                        Name = student.Name,
                        Phone = student.Phone,
                        Description = student.Description,
                        Group = context.Groups.Where(p => p.GroupName == student.GroupName).FirstOrDefault()
                    });
                }
                context.SaveChanges();
                StudentsList.Clear();
                OnPropertyChanged(nameof(StudentsList));
                FileName = string.Empty;
                ErrorMessage = string.Empty;
            }
            catch(Exception)
            {
                ErrorMessage = "Произошла ошибка при сохранении.";
            }            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
