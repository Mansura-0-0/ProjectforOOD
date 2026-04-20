using System;

namespace Project
{
    public class TaskItem
    {
        // Properties
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string Category { get; set; }
       
        public DateTime DueDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}