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
        private ObservableCollection<TaskItem> tasks;
        private ICollectionView tasksView;

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

                tasksView = CollectionViewSource.GetDefaultView(taskListView.ItemsSource);

                UpdateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tasks: " + ex.Message);
            }
        }

        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titlebx.Text) ||
                    string.IsNullOrWhiteSpace(descbx.Text) ||
                    prioritybx.SelectedItem == null ||
                    categorybx.SelectedItem == null ||
                    datebx.SelectedDate == null)
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

                db.Tasks.Add(newTask);
                db.SaveChanges();

                tasks.Add(newTask);

                ClearInputs();

                UpdateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding task: " + ex.Message);
            }
        }

        private void donebtn_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Error completing task: " + ex.Message);
            }
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting task: " + ex.Message);
            }
        }

        private void alltaskbtn_Click(object sender, RoutedEventArgs e)
        {
            tasksView.Filter = null;
        }

        private void Searchbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tasksView == null) return;

            string search = (searchbx.Text ?? "").Trim().ToLower();

            if (search == "search tasks...")
                search = "";

            tasksView.Filter = obj =>
            {
                TaskItem t = obj as TaskItem;

                if (t == null) return false;

                return (t.Title ?? "").ToLower().Contains(search) ||
                       (t.Description ?? "").ToLower().Contains(search) ||
                       (t.Category ?? "").ToLower().Contains(search);
            };
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

        private void ClearInputs()
        {
            titlebx.Text = "";
            descbx.Text = "";
            prioritybx.SelectedIndex = -1;
            categorybx.SelectedIndex = -1;
            datebx.SelectedDate = null;
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