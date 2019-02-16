using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StudentHostelApp.Model;
using StudentHostelApp.ViewModel;
using StudentHostelApp.DataAccess;

namespace UnitTestStudentHostel
{
    [TestClass]
    public class UnitTestGroupsListViewModel
    {
        [TestMethod]
        public void GetAllGroups()
        {
            var data = new List<Group>
            {
                new Group {GroupId=1, GroupName="TestGroup", SoftDeleted=false},
                new Group{GroupId=2, GroupName="AnotherTestGroup", SoftDeleted=false}
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
            Assert.AreEqual("TestGroup", groups[0].GroupName);
            Assert.AreEqual("AnotherTestGroup", groups[1].GroupName);            
        }
    }
}
