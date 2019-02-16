using System.ComponentModel;
using StudentHostelApp.DataAccess;
using StudentHostelApp.Commands;

namespace StudentHostelApp.Code
{
    /// <summary>
    /// Предоставляет базовый CRUD интерфейс модели представления
    /// </summary>
    public class BaseCrudViewModel:INotifyPropertyChanged
    {
        protected readonly StudentHostelContext context;// = new StudentHostelContext();

        // Команды для взаимодействия с коллекцией объектов
        public Command AddCommand { get; protected set; }
        public Command EditCommand { get; protected set; }
        public Command SaveCommand { get; protected set; }
        public Command DeleteCommand { get; protected set; }
        public Command CancelCommand { get; protected set; }


        /// <summary>
        /// Конструктор представления модели
        /// </summary>
        /// <param name="context">Контекст данных</param>
        public BaseCrudViewModel(StudentHostelContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Загружает список студентов для отображения
        /// </summary>
        protected virtual void GetData() { }

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
        protected virtual void Add() { }

        /// <summary>
        /// Выполняет перевод в режим редактирования текущего объекта
        /// </summary>
        protected virtual void Edit() { }

        /// <summary>
        /// Сохраняет изменения в текущем объекте при его добавлении или редактировании
        /// </summary>
        protected virtual void SaveChanges() { }

        /// <summary>
        /// Удаляет текущий объект из коллекции
        /// </summary>
        protected virtual void Delete() { }

        /// <summary>
        /// Выполняет отмену изменений в текущем объекте
        /// </summary>
        protected virtual void DiscardChanges() { }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
