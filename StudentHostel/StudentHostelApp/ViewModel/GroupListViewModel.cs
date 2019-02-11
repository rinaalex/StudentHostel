using System.Linq;
using System.Collections.ObjectModel;
using StudentHostelApp.Model;
using StudentHostelApp.Commands;
using StudentHostelApp.ViewModel.SingleEntityVM;
using StudentHostelApp.Code;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет CRUD-функции для работы со списком учебных групп
    /// </summary>
    public class GroupListViewModel:BaseCrudViewModel
    {
        public ObservableCollection<GroupViewModel> GroupList { get; private set; }

        private GroupViewModel currentGroup;
        public GroupViewModel CurrentGroup
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
            var groups = context.Groups.Select(p => new GroupViewModel
            {
                GroupId = p.GroupId,
                GroupName = p.GroupName
            }).ToList();

            GroupList = new ObservableCollection<GroupViewModel>(groups);           
        }        

        #region Свойства и методы для редактирвания коллекции
        /// <summary>
        /// Выполняет переход в режим добавления нового объекта
        /// </summary>
        protected override void Add()
        {
            GroupList.Add(new GroupViewModel { GroupId = 0 });
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
                oldGroup = new Group
                {
                    GroupId = CurrentGroup.GroupId,
                    GroupName = CurrentGroup.GroupName
                };
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
        private bool Validate(GroupViewModel group)
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
                    Group group = new Group
                    {
                        GroupId = CurrentGroup.GroupId,
                        GroupName = CurrentGroup.GroupName
                    };
                    context.Groups.Add(group);
                    context.SaveChanges();
                    CurrentGroup.GroupId = context.Groups.OrderByDescending(p => p.GroupId).FirstOrDefault().GroupId;
                    IsAdding = false;
                }
                // Сохранение изменений в текущем объекте
                else
                {
                    var result = context.Groups.Where(p => p.GroupId == oldGroup.GroupId).FirstOrDefault();
                    result.GroupName = CurrentGroup.GroupName;
                    context.SaveChanges();
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
            }
            ErrorMessage = string.Empty;
        }
        #endregion       
    }
}
