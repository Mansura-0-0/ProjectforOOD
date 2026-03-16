
using System;

namespace Project
{
    public interface ITask
    {
        string Title { get; set; }
        DateTime DueDate { get; set; }
        bool IsCompleted { get; set; }

        bool IsOverdue();
    }
}