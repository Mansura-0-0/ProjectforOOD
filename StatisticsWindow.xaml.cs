using System.Linq;
using System.Windows;

namespace Project
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();

            using (TaskDbContext db = new TaskDbContext())
            {
                int total = db.Tasks.Count();
                int completed = db.Tasks.Count(t => t.IsCompleted);
                int overdue = db.Tasks.Count(t => !t.IsCompleted && t.DueDate < System.DateTime.Now);

                totalTasks.Text = "Total Tasks: " + total;
                completedTasks.Text = "Completed Tasks: " + completed;
                overdueTasks.Text = "Overdue Tasks: " + overdue;
            }
        }
    }
}