using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;

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