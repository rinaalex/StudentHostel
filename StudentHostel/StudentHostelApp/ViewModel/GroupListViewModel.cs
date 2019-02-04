using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using StudentHostelApp.DataAccess;
using StudentHostelApp.Model;
using StudentHostelApp.Commands;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет доступ к коллекции групп для отображения и редактирования
    /// </summary>
    public class GroupListViewModel:BaseCrudViewModel
    {
        public ObservableCollection<Group> GroupList { get; private set; }

        private Group currentGroup;
        public Group CurrentGroup
        {
            get
            {
                return this.currentGroup;
            }
            set
            {
                this.currentGroup = value;
                OnPropertyChanged(nameof(CurrentGroup));
            }
        }

        // Конструктор модели представления
        public GroupListViewModel()
        {
            GetData();
            if (GroupList.Count == 0)
            {
                CurrentGroup = null;
            }
            else
            {
                CurrentGroup = GroupList.First();
            }

            // Инициализация команд
            AddCommand = new Command(Add, () => { return !(IsAdding || IsEditing); });
            EditCommand = new Command(Edit, () => { return !(IsAdding || IsEditing); });
            SaveCommand = new Command(SaveChanges, () => { return IsAdding || IsEditing; });
            DeleteCommand = new Command(Delete, () => { return !(IsAdding || IsEditing); });
            CancelCommand = new Command(DiscardChanges, () => { return (IsAdding || IsEditing); });
        }

        /// <summary>
        /// Загружает список групп для отображения и редактриования
        /// </summary>
        protected override void GetData()
        {
            // Загрузка из контекста необходимых для отображения данных
            var groups = context.Groups.Select(p => new
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

        #region Свойства и методы для редактирвания коллекции
        /// <summary>
        /// Выполняет переход в режим добавления нового объекта
        /// </summary>
        protected override void Add()
        {
            GroupList.Add(new Group { GroupId = 0 });
            CurrentGroup = GroupList.Last();
            IsAdding = true;
        }

        private Group oldGroup;

        /// <summary>
        /// Выполняет переход в режим редактирования текущего объекта
        /// </summary>
        protected override void Edit()
        {
            if (CurrentGroup != null)
            {
                IsEditing = true;
                oldGroup = new Group();
                oldGroup.GroupId = CurrentGroup.GroupId;
                oldGroup.GroupName = CurrentGroup.GroupName;
                //oldGroup.Students = CurrentGroup.Students;
                ErrorMessage = string.Empty;
            }
            else
                ErrorMessage = "Не выбран объект для редактирования!";            
        }

        /// <summary>
        /// Выполняет проверку корректности полей объекта
        /// </summary>
        /// <param name="group">Проверяемый объект</param>
        /// <returns></returns>
        private bool Validate(Group group)
        {
            if (string.IsNullOrEmpty(group.GroupName))
            {
                ErrorMessage = "Название учебной группы не может быть пустым!";
                return false;
            }
            else if(group.GroupName.Length>10)
            {
                ErrorMessage = "Название учебной группы не должно превышать 10 символов!";
                return false;
            }
            else
            {
                ErrorMessage = string.Empty;
                return true;
            }
        }

        /// <summary>
        /// Выполняет сохранение изменений в коллекции
        /// </summary>
        protected override void SaveChanges()
        {
            if (Validate(CurrentGroup))
            {
                // Сохранение нового объекта
                if (CurrentGroup.GroupId == 0)
                {
                    context.Groups.Add(CurrentGroup);
                    context.SaveChanges();
                    CurrentGroup.GroupId = context.Groups.OrderByDescending(p => p.GroupId).FirstOrDefault().GroupId;
                    OnPropertyChanged(nameof(CurrentGroup));
                    IsAdding = false;
                }
                // Сохранение изменений в текущем объекте
                else
                {
                    var result = context.Groups.Where(p => p.GroupId == oldGroup.GroupId).FirstOrDefault();
                    result.GroupName = CurrentGroup.GroupName;
                    context.SaveChanges();
                    OnPropertyChanged(nameof(CurrentGroup));
                    IsEditing = false;
                }
            }
        }

        /// <summary>
        /// Выполняет удаление текущего объекта из коллекции
        /// </summary>
        protected override void Delete()
        {
            if (CurrentGroup != null)
            {
                var group = context.Groups.Where(p => p.GroupId == CurrentGroup.GroupId).FirstOrDefault();
                GroupList.Remove(CurrentGroup);
                context.Groups.Remove(group);
                context.SaveChanges();
                ErrorMessage = string.Empty;
            }
            else
                ErrorMessage = "Не выбран объект для удаления!";
        }

        /// <summary>
        /// Выполняет выход из режима добавления или редактирования
        /// </summary>
        protected override void DiscardChanges()
        {
            if (IsAdding)
            {
                GroupList.Remove(CurrentGroup);
                IsAdding = false;
            }
            else if(IsEditing)
            {               
                CurrentGroup.GroupName = oldGroup.GroupName;
                IsEditing = false;
                OnPropertyChanged(nameof(CurrentGroup));
            }
            ErrorMessage = string.Empty;
        }
        #endregion       
    }
}
