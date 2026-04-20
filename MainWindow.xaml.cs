using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Project
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();
        private ICollectionView tasksView;
        //calling the database 
        private Database db = new Database();

        public MainWindow()
        {
           
            InitializeComponent();
            // Initialize the database and loading tasks
            db.Initialize();

            tasks = new ObservableCollection<TaskItem>(db.GetTasks());

            taskListView.ItemsSource = tasks;

            prioritybx.ItemsSource = new[] { "Low", "Medium", "High" };
            categorybx.ItemsSource = new[] { "School", "Work", "Home", "Personal" };

            tasksView = CollectionViewSource.GetDefaultView(taskListView.ItemsSource);
            //updatig progress
            UpdateProgress();
        }
        // maximum five tasks can be aded to the list unless one is completed or deleted you cant add more tasks to the list
        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            // Check for maximun active tasks
            int activeTasks = tasks.Count(t => !t.IsCompleted);

            if (activeTasks >= 5)
            {
                MessageBox.Show("Maximum of 5 active tasks allowed. Complete or delete a task before adding another.");
                return;
            }
            if (string.IsNullOrWhiteSpace(titlebx.Text) ||
                string.IsNullOrWhiteSpace(descbx.Text) ||
                prioritybx.SelectedItem == null ||
                categorybx.SelectedItem == null ||
                datebx.SelectedDate == null )
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            TaskItem newTask = new TaskItem
            {
                Title = titlebx.Text,
                Description = descbx.Text,
                Priority = prioritybx.SelectedItem.ToString(),
                Category = categorybx.SelectedItem.ToString(),
                DueDate = datebx.SelectedDate.Value,
                IsCompleted = false
            };

            db.AddTask(newTask);

            tasks.Add(newTask);
            SortTasks();

            ClearInputs();

            UpdateProgress();
        }
        // Marking a task as completed and refreshing the list to show changes

        private void donebtn_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task = taskListView.SelectedItem as TaskItem;

            if (task != null)
            {
                task.IsCompleted = true;

                db.CompleteTask(task.Id);

                taskListView.Items.Refresh();
                SortTasks();

                UpdateProgress();
            }
        }
        // Deleting a task from the list and database, then refreshing the list to show change

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task = taskListView.SelectedItem as TaskItem;

            if (task != null)
            {
                db.DeleteTask(task.Id);

                tasks.Remove(task);

                UpdateProgress();
            }
        }
        // Filtering the list to show only complete taskss
        private void alltaskbtn_Click(object sender, RoutedEventArgs e)
        {
            tasksView.Filter = null;
        }
        //search for tasks by title, description or category and showing only the matching results in the lisst
        private void Searchbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tasksView == null) return;

            string search = (searchbx.Text ?? "").Trim().ToLower();

            tasksView.Filter = obj =>
            {
                TaskItem t = obj as TaskItem;

                if (t == null) return false;

                return (t.Title ?? "").ToLower().Contains(search) ||
                       (t.Description ?? "").ToLower().Contains(search) ||
                       (t.Category ?? "").ToLower().Contains(search);
            };
        }
        //clear aearching box
        private void Searchbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchbx.Text == "Search tasks...")
                searchbx.Text = "";
        }
        //if the search box is empty , placeholder text

        private void Searchbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchbx.Text))
                searchbx.Text = "Search tasks...";
        }
        

        private void taskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        //clear all input fields after ading a task

        private void ClearInputs()
        {
            titlebx.Text = "";
            descbx.Text = "";
            prioritybx.SelectedIndex = -1;
            categorybx.SelectedIndex = -1;
            datebx.SelectedDate = null;
        }

        // Update the progress bar based on the parcantage of completed tasks
        private void UpdateProgress()
        {
            if (tasks.Count == 0)
            {
                progressBar.Value = 0;
                return;
            }

            double percent = tasks.Count(t => t.IsCompleted) * 100.0 / tasks.Count;

            progressBar.Value = percent;
        }
        // Sort the tasks by priority and then by completion status 
        private void SortTasks()
        {
            var sorted = tasks
                .OrderBy(t => t.Priority == "Low")
                .ThenBy(t => t.Priority == "Medium")
                .ThenBy(t => t.Priority == "High" ? 0 : 1)
                .ToList();

            tasks.Clear();

            foreach (var task in sorted)
                tasks.Add(task);
        }
    }
}