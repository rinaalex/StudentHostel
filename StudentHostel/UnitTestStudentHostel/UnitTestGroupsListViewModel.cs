﻿using Effort;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StudentHostelApp.Model;
using StudentHostelApp.ViewModel;
using StudentHostelApp.DataAccess;
using StudentHostelApp.ViewModel.SingleEntityVM;

namespace UnitTestStudentHostel
{
    [TestClass]
    public class UnitTestGroupsListViewModel
    {
        /// <summary>
        /// Выполняет проверку правильности загрузки данных из таблицы Groups
        /// (только группы, не помеченные как удаленные)
        /// </summary>
        [TestMethod]
        public void GetAllGroupsMock()
        {
            var data = new List<Group>
            {
                new Group {GroupId=1, GroupName="Test", SoftDeleted=false},
                new Group {GroupId=2, GroupName="Another", SoftDeleted=false},
                new Group{GroupId=3, GroupName="Deleted", SoftDeleted=true}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Group>>();
            mockSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<StudentHostelContext>();
            mockContext.Setup(c => c.Groups).Returns(mockSet.Object);
            var viewModel = new GroupListViewModel(mockContext.Object);
            var groups = viewModel.GroupList;

            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual("Test", groups[0].GroupName);
            Assert.AreEqual("Another", groups[1].GroupName);            
        }

        /// <summary>
        /// Выполняет проверку добавления новой записи в таблицу Groups
        /// </summary>
        [TestMethod]
        public void AddGroupMock()
        {
            var data = new List<Group>
            {
                new Group {GroupId=1, GroupName="Test", SoftDeleted=false},
                new Group {GroupId=2, GroupName="Another", SoftDeleted=false}
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Group>>();
            mockSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<StudentHostelContext>();
            mockContext.Setup(c => c.Groups).Returns(mockSet.Object);
            var viewModel = new GroupListViewModel(mockContext.Object);

            viewModel.AddCommand.Execute("");
            viewModel.CurrentGroup = new GroupViewModel { GroupId = 0, GroupName = "NewTest" };
            viewModel.SaveCommand.Execute("");

            var groups = viewModel.GroupList;

            Assert.AreEqual(3, groups.Count);
            Assert.AreEqual("NewTest", groups[2].GroupName);
        }

        //[TestMethod]
        //public void AddGroup()
        //{
        //    StudentHostelContext context = new StudentHostelContext();
        //    GroupListViewModel viewModel = new GroupListViewModel(context);
        //    int count = viewModel.GroupList.Count;
        //    viewModel.AddCommand.Execute("");
        //    viewModel.CurrentGroup = new GroupViewModel{ GroupId = 0, GroupName = "00" };
        //    viewModel.SaveCommand.Execute("");
        //    Assert.AreEqual(count + 1, viewModel.GroupList.Count);
        //    Assert.AreEqual("00", viewModel.CurrentGroup.GroupName);
        //}

        /// <summary>
        /// Выполняет проверку добавления новой записи в таблицу Groups
        /// </summary>
        [TestMethod]
        public void AddGroupEffort()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new StudentHostelContext(connection);
            GroupListViewModel viewModel = new GroupListViewModel(context);

            // Определяем текущее количество записей в таблице
            int count = viewModel.GroupList.Count;

            viewModel.AddCommand.Execute("");
            viewModel.CurrentGroup = new GroupViewModel { GroupId = 0, GroupName = "Test" };
            viewModel.SaveCommand.Execute("");

            // Получаем добавленную запись
            var group = context.Groups.Where(p => p.GroupId == count + 1).FirstOrDefault();

            // Проверяем, что количество записей в таблице увеличилось на 1
            Assert.AreEqual(count + 1, context.Groups.Count());
            Assert.AreEqual(count+1, viewModel.GroupList.Count);
            // Проверяем поле GroupId новой записи 
            Assert.AreEqual(count+1, group.GroupId);
            Assert.AreEqual(count + 1, viewModel.GroupList[count].GroupId);
            // Проверяем поле GroupName новой записи
            Assert.AreEqual("Test", group.GroupName);
            Assert.AreEqual("Test", viewModel.GroupList[count].GroupName);
        }

        /// <summary>
        /// Выпоняет проверку изменения записи в таблице Groups
        /// </summary>
        [TestMethod]
        public void EditGroupEffort()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new StudentHostelContext(connection);
            GroupListViewModel viewModel = new GroupListViewModel(context);
            
            viewModel.CurrentGroup = viewModel.GroupList.Where(p => p.GroupId == 1).FirstOrDefault();
            viewModel.EditCommand.Execute("");
            viewModel.CurrentGroup.GroupName = "NewName";
            viewModel.SaveCommand.Execute("");

            // Получаем измененную запись
            Group group = context.Groups.Where(p => p.GroupId == 1).FirstOrDefault();

            // Проверяем, что поле GroupName изменено
            Assert.AreEqual("NewName", group.GroupName);
            Assert.AreEqual("NewName", viewModel.CurrentGroup.GroupName);
            Assert.AreEqual("NewName", viewModel.GroupList[0].GroupName);
        }

        /// <summary>
        /// Выполняет проверку удаления неиспользуемой записи из таблицы Groups
        /// </summary>
        [TestMethod]
        public void DeleteGroupEffort()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new StudentHostelContext(connection);
            GroupListViewModel viewModel = new GroupListViewModel(context);

            // Определяем текущее количество записей в таблице
            int count = context.Groups.Where(p=>!p.SoftDeleted).Count();

            // Выбираем группу, в которой нет студентов
            viewModel.CurrentGroup = viewModel.GroupList.Where(p => p.GroupId == 3).FirstOrDefault();

            viewModel.DeleteCommand.Execute("");

            // Проверяем, что количество записей стало меньше на 1
            Assert.AreEqual(count - 1, context.Groups.Where(p=>!p.SoftDeleted).Count());
            Assert.AreEqual(count - 1, viewModel.GroupList.Count());
            // Проверяем, что записи больше нет в списке представления модели
            Assert.AreEqual(null, viewModel.GroupList.Where(p => p.GroupId == 3).FirstOrDefault());
            // Проверяем, что в таблице запись помечена как удаленная
            Assert.AreEqual(true, context.Groups.Where(p => p.GroupId == 3).Select(p => p.SoftDeleted).FirstOrDefault());
        }

        /// <summary>
        /// Выполняет проверку удаления группы, в которой числятся 
        /// проживающие студенты
        /// </summary>
        [TestMethod]
        public void CannotDeleteGroup()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new StudentHostelContext(connection);

            GroupListViewModel viewModel = new GroupListViewModel(context);

            // Определяем текущее количество записей в таблице
            int count = context.Groups.Where(p => !p.SoftDeleted).Count();

            // Выбираем группу, в которой числятся студенты
            viewModel.CurrentGroup = viewModel.GroupList.Where(p => p.GroupId == 2).FirstOrDefault();

            viewModel.DeleteCommand.Execute("");

            // Проверяем, что количество записей не изменилось
            Assert.AreEqual(count, context.Groups.Where(p => !p.SoftDeleted).Count());
            Assert.AreEqual(count, viewModel.GroupList.Count);
            // Проверяем, что запись не удалилась
            Assert.AreNotEqual(null, viewModel.GroupList.Where(p => p.GroupId == 2).FirstOrDefault());
            Assert.AreNotEqual(null, context.Groups.Where(p => p.GroupId == 2 && !p.SoftDeleted).FirstOrDefault());
            // Проверяем, что сообщение об ошибке появилось
            Assert.AreEqual("Невозможно удалить информацию о группе, в которой числятся проживающие студенты!", viewModel.ErrorMessage);
        }
    }
}
