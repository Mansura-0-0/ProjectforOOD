using System;

namespace Project
{
    public class TaskItem : ITask
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string Category { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsOverdue()
        {
            return DateTime.Now > DueDate && !IsCompleted;
        }
    }
}