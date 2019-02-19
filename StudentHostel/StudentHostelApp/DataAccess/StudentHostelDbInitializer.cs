using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using StudentHostelApp.Model;

namespace StudentHostelApp.DataAccess
{
    /// <summary>
    /// Реализует инициализатор базы данных для первого запуска приложения
    /// </summary>
    public class StudentHostelDbInitializer: CreateDatabaseIfNotExists<StudentHostelContext>
    {
        protected override void Seed(StudentHostelContext context)
        {
            // Добавление учебных групп
            List<Group> groups = new List<Group>
            {
                new Group() { GroupId = 1, GroupName = "1П", SoftDeleted = false },
                new Group() { GroupId = 2, GroupName = "2П", SoftDeleted = false },
                new Group() { GroupId = 3, GroupName = "2М", SoftDeleted = false }
            };
            context.Groups.AddRange(groups);

            // Добавление студентов
            List<Student> students = new List<Student>
            {
                new Student()
                {
                    StudentId = 1,
                    Name = "Иванов Иван Геннадьевич",
                    Phone = "89990001234",
                    Group = groups.Where(p=>p.GroupId==1).FirstOrDefault(),
                    Description = "Тестовая запись",
                    SoftDeleted = false
                },
                new Student()
                {
                    StudentId = 2,
                    Name = "Петров Петр Максимович",
                    Phone = "89990001223",
                    Group = groups.Where(p=>p.GroupId==1).FirstOrDefault(),
                    Description = "Тестовая запись",
                    SoftDeleted = false
                },
                new Student()
                {
                    StudentId = 3,
                    Name = "Семёнов Семён Васильевич",
                    Phone = "89991111234",
                    Group = groups.Where(p=>p.GroupId==2).FirstOrDefault(),
                    Description = "Тестовая запись",
                    SoftDeleted = false
                },
                new Student()
                {
                    StudentId = 4,
                    Name = "Викторов Виктор Евгеньевич",
                    Phone = "89990001333",
                    Group = groups.Where(p=>p.GroupId==2).FirstOrDefault(),
                    Description = "Тестовая запись",
                    SoftDeleted = false
                }
            };
            context.Students.AddRange(students);

            // Добавление комнат
            List<Room> rooms = new List<Room>
            {
                new Room
                {
                    RoomId=1,
                    RoomNumber="101",
                    Seats=5,
                    SoftDeleted=false
                },
                new Room
                {
                    RoomId=2,
                    RoomNumber="102",
                    Seats=5,
                    SoftDeleted=false
                },
                new Room
                {
                    RoomId=3,
                    RoomNumber="103",
                    Seats=3,
                    SoftDeleted=false
                },
                new Room
                {
                    RoomId=4,
                    RoomNumber="104",
                    Seats=3,
                    SoftDeleted=false
                },
                new Room
                {
                    RoomId=5,
                    RoomNumber="105",
                    Seats=3,
                    SoftDeleted=false
                }
            };
            context.Rooms.AddRange(rooms);

            // Добавление размещений
            List<Accomodation> accomodations = new List<Accomodation>
            {
                new Accomodation
                {
                    AccomodationId=1,
                    Student=students.Where(p=>p.StudentId==1).FirstOrDefault(),
                    Room=rooms.Where(p=>p.RoomId==1).FirstOrDefault(),
                    DateStart=DateTime.Now
                },
                new Accomodation
                {
                    AccomodationId=2,
                    Student=students.Where(p=>p.StudentId==2).FirstOrDefault(),
                    Room=rooms.Where(p=>p.RoomId==1).FirstOrDefault(),
                    DateStart=DateTime.Now
                },
                new Accomodation
                {
                    AccomodationId=3,
                    Student=students.Where(p=>p.StudentId==3).FirstOrDefault(),
                    Room=rooms.Where(p=>p.RoomId==2).FirstOrDefault(),
                    DateStart=DateTime.Now
                }
            };
            context.Accomodations.AddRange(accomodations);

            base.Seed(context);
        }
    }
}
