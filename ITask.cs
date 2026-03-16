using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public interface ITask
    {
        string Title { get; set; }
        DateTime DueDate { get; set; }
        bool IsCompleted { get; set; }
        bool IsOverdue();
        //this th jj
    }
}
