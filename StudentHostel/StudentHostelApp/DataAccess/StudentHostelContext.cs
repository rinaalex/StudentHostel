using System;
using System.Data.Entity;
using StudentHostelApp.Model;


namespace StudentHostelApp.DataAccess
{
    public class StudentHostelContext: DbContext
    {
        public StudentHostelContext():base("StudentHostelDb")
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().
                HasRequired<Group>(p => p.Group).
                WithMany(q => q.Students).
                WillCascadeOnDelete(true);    
        }
    }
}
