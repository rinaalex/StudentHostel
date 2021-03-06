﻿using StudentHostelApp.Code;

namespace StudentHostelApp.ViewModel.SingleEntityVM
{
    /// <summary>
    /// Предоставляет информацию об учебной группе для отображения
    /// </summary>
    public class GroupViewModel: BaseViewModel
    {
        private int groupId;
        public int GroupId
        {
            get { return this.groupId; }
            set { this.groupId = value; OnPropertyChanged(nameof(GroupId)); }
        }
        private string groupName;
        public string GroupName
        {
            get { return this.groupName; }
            set { this.groupName = value; OnPropertyChanged(nameof(GroupName)); }
        }
    }
}
