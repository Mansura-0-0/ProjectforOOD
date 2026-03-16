using System.Data.Entity;

namespace Project
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext() : base("TaskDb")
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
    }
}