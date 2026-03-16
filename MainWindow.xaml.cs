using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Project
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<TaskItem> tasks;
        private TaskDbContext db = new TaskDbContext();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                db.Database.CreateIfNotExists();

                tasks = new ObservableCollection<TaskItem>(db.Tasks.ToList());

                taskListView.ItemsSource = tasks;

                prioritybx.ItemsSource = new[] { "Low", "Medium", "High" };
                categorybx.ItemsSource = new[] { "School", "Work", "Home", "Personal" };

                UpdateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titlebx.Text))
            {
                MessageBox.Show("Title required");
                return;
            }

            TaskItem task = new TaskItem
            {
                Title = titlebx.Text,
                Description = descbx.Text,
                Priority = prioritybx.SelectedItem?.ToString(),
                Category = categorybx.SelectedItem?.ToString(),
                DueDate = datebx.SelectedDate ?? DateTime.Now,
                IsCompleted = false
            };

            db.Tasks.Add(task);
            db.SaveChanges();

            tasks.Add(task);

            UpdateProgress();
        }

        private void donebtn_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task = taskListView.SelectedItem as TaskItem;

            if (task != null)
            {
                task.IsCompleted = true;

                db.SaveChanges();

                taskListView.Items.Refresh();

                UpdateProgress();
            }
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task = taskListView.SelectedItem as TaskItem;

            if (task != null)
            {
                db.Tasks.Remove(task);
                db.SaveChanges();

                tasks.Remove(task);

                UpdateProgress();
            }
        }

        private void alltaskbtn_Click(object sender, RoutedEventArgs e)
        {
            taskListView.ItemsSource = tasks;
        }

        private void Searchbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = searchbx.Text.ToLower();

            taskListView.ItemsSource =
                tasks.Where(t =>
                t.Title.ToLower().Contains(search) ||
                t.Description.ToLower().Contains(search) ||
                t.Category.ToLower().Contains(search));
        }

        private void Searchbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchbx.Text == "Search tasks...")
                searchbx.Text = "";
        }

        private void Searchbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchbx.Text))
                searchbx.Text = "Search tasks...";
        }

        private void taskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

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
    }
}