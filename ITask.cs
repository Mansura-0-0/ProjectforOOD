using System;

namespace Project
{
   
    public interface ITask
    {
        // Properties
        string Title { get; set; }
        DateTime DueDate { get; set; }
        bool IsCompleted { get; set; }
        // Methods
        bool IsOverdue();
    }
}