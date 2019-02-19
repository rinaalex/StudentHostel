using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort;
using StudentHostelApp.DataAccess;
using StudentHostelApp.ViewModel;
using StudentHostelApp.ViewModel.SingleEntityVM;

namespace UnitTestStudentHostel
{
    [TestClass]
    public class UnitTestStudentListViewModel
    {
        /// <summary>
        /// Выполняет проверку загрузки списка студентов
        /// </summary>
        [TestMethod]
        public void GetAllStudents()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new StudentHostelContext(connection);
            StudentListViewModel viewModel = new StudentListViewModel(context);

            var groupsInVM = viewModel.StudentList;
            var groupsInContext = context.Students.Where(p => !p.SoftDeleted);

            // Проверяем, что количествозаписей во VM = количеству неудаленных записей в контексте
            Assert.AreEqual(groupsInContext.Count(), groupsInVM.Count);
        }

        /// <summary>
        /// Выполняет проверку добавления новой записи в таблицу Students
        /// </summary>
        [TestMethod]
        public void AddStudent()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new StudentHostelContext(connection);
            StudentListViewModel viewModel = new StudentListViewModel(context);

            int count = viewModel.StudentList.Count;

            viewModel.AddCommand.Execute("");
            viewModel.CurrentStudent = new StudentViewModel
            {
                StudentId = 0,
                Name = "Test Student",
                Phone = "123-123",
                Description = "Test",
                GroupName="2П"
            };
            viewModel.SaveCommand.Execute("");

            // Получаем новую запись из контекста
            var student = context.Students.Where(p => p.StudentId == count+1).FirstOrDefault();

            // Проверяем, что количество записей увеличилось на 1
            Assert.AreEqual(count + 1, viewModel.StudentList.Count);
            Assert.AreEqual(count + 1, context.Students.Count());
            // Проверяем, что поле Имя сохранено верно
            Assert.AreEqual("Test Student", student.Name);
            Assert.AreEqual("Test Student", viewModel.StudentList[count].Name);
            // Проверяем, что поле Группа сохранено верно
            Assert.AreEqual("2П", student.Group.GroupName);
            Assert.AreEqual("2П", viewModel.StudentList[count].GroupName);
        }
    }
}
