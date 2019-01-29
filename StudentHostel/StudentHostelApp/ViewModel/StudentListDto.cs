using System;

namespace StudentHostelApp.ViewModel
{
    /// <summary>
    /// Предоставляет информацию о студенте для отображения
    /// </summary>
    public class StudentListDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
    }
}
