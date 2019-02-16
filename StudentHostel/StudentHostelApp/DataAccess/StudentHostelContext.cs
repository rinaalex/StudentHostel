using System;
using System.Data.Entity;
using System.Linq;
using StudentHostelApp.Model;


namespace StudentHostelApp.DataAccess
{
    public class StudentHostelContext: DbContext
    {
        public StudentHostelContext():base("StudentHostelDb")
        {
            Database.SetInitializer<StudentHostelContext>(new StudentHostelDbInitializer());
        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Accomodation> Accomodations { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().
                HasRequired<Group>(p => p.Group).
                WithMany(q => q.Students).
                WillCascadeOnDelete(true);
        }
    }
}
