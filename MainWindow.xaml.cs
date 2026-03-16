using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Project
{
    public partial class MainWindow : Window
    {
        //declare variables
        private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();
        private string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");
        private ICollectionView tasksView;

        public MainWindow()
        {
            InitializeComponent();

            tasks = LoadTasks();
            taskListView.ItemsSource = tasks;

            prioritybx.ItemsSource = new[] { "Low", "Medium", "High" };
            categorybx.ItemsSource = new[] { "School", "Work", "Home", "Personal" };

            tasksView = CollectionViewSource.GetDefaultView(taskListView.ItemsSource);

            UpdateProgress();
        }
        //add, done, delete, all tasks, search, focus, selection changed, update progress, load and save methods
        private void addbtn_Click(object sender, RoutedEventArgs e)
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

            tasks.Add(new TaskItem
            {
                Title = titlebx.Text,
                Description = descbx.Text,
                Priority = prioritybx.SelectedItem.ToString(),
                Category = categorybx.SelectedItem.ToString(),
                DueDate = datebx.SelectedDate.Value,
                IsCompleted = false
            });

            SaveTasks();
            UpdateProgress();
        }

        private void donebtn_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task = taskListView.SelectedItem as TaskItem;
            if (task != null)
            {
                task.IsCompleted = true;
                UpdateProgress();
                SaveTasks();
            }
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task = taskListView.SelectedItem as TaskItem;
            if (task != null)
            {
                tasks.Remove(task);
                SaveTasks();
                UpdateProgress();
            }
        }
        //updated
        private void alltaskbtn_Click(object sender, RoutedEventArgs e)
        {
            if (tasksView == null) return;
            tasksView.Filter = null;

        }

        private void Searchbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tasksView == null) return;

            string search = (searchbx.Text ?? "").Trim().ToLower();
            if (search == "search tasks...") search = "";

            tasksView.Filter = delegate (object obj)
            {
                TaskItem t = obj as TaskItem;
                if (t == null) return false;

                string title = (t.Title ?? "").ToLower();
                string desc = (t.Description ?? "").ToLower();
                string cat = (t.Category ?? "").ToLower();

                return title.Contains(search) || desc.Contains(search) || cat.Contains(search);
            };
        }
        //updated
        private void Searchbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchbx.Text == "Search tasks...")
                searchbx.Text = "";
        }
        //updated
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
      

        private ObservableCollection<TaskItem> LoadTasks()
        {
            if (!File.Exists(jsonPath))
                return new ObservableCollection<TaskItem>();

            string json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(json);
        }
      

        private void SaveTasks()
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, json);
        }
    }
}