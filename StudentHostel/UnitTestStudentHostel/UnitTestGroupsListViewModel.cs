using System;
using System.Data;
using System.Data.Common;
using Effort;
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
        [TestMethod]
        public void GetAllGroupsMock()
        {
            var data = new List<Group>
            {
                new Group {GroupId=1, GroupName="Test", SoftDeleted=false},
                new Group{GroupId=2, GroupName="Another", SoftDeleted=false}
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

        [TestMethod]
        public void AddGroup()
        {
            StudentHostelContext context = new StudentHostelContext();
            GroupListViewModel viewModel = new GroupListViewModel(context);
            int count = viewModel.GroupList.Count;
            viewModel.AddCommand.Execute("");
            viewModel.CurrentGroup = new GroupViewModel{ GroupId = 0, GroupName = "00" };
            viewModel.SaveCommand.Execute("");
            Assert.AreEqual(count + 1, viewModel.GroupList.Count);
            Assert.AreEqual("00", viewModel.CurrentGroup.GroupName);
        }

        [TestMethod]
        public void AddGroupEffort()
        {
            var connection = DbConnectionFactory.CreateTransient();
            var context = new StudentHostelContext(connection);

            GroupListViewModel viewModel = new GroupListViewModel(context);
            int count = viewModel.GroupList.Count;
            viewModel.AddCommand.Execute("");
            viewModel.CurrentGroup = new GroupViewModel { GroupId = 0, GroupName = "Test" };
            viewModel.SaveCommand.Execute("");

            var group = context.Groups.Where(p => p.GroupId == count + 1).FirstOrDefault();

            Assert.AreEqual(count+1, viewModel.GroupList.Count);
            Assert.AreEqual(count+1, group.GroupId);
            Assert.AreEqual("Test", group.GroupName);
        }

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

            Group group = context.Groups.Where(p => p.GroupId == 1).FirstOrDefault();

            Assert.AreEqual(group.GroupName, viewModel.CurrentGroup.GroupName);
        }
    }
}
