using System.Linq;
using System.Windows;

namespace Project
{
    public partial class StatisticsWindow : Window
    {
        Database db = new Database();

        public StatisticsWindow()
        {
            InitializeComponent();

            var tasks = db.GetTasks();

            int total = tasks.Count;
            int completed = tasks.Count(t => t.IsCompleted);
            int overdue = tasks.Count(t => !t.IsCompleted && t.DueDate < System.DateTime.Now);

            totalTasks.Text = "Total Tasks: " + total;
            completedTasks.Text = "Completed Tasks: " + completed;
            overdueTasks.Text = "Overdue Tasks: " + overdue;
        }
    }
}