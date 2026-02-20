using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Project
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<TaskItem> tasks;
        private const int MaxTasks = 5;
        private readonly string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");

        public MainWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // Load tasks from JSON
                tasks = LoadTasks();

                // Bind ListView
                taskListView.ItemsSource = tasks;

                // Populate priority ComboBox
                prioritybx.ItemsSource = new[] { "Low", "Medium", "High" };

                // Connect events
                taskListView.SelectionChanged += taskListView_SelectionChanged;
                searchbx.TextChanged += Searchbx_TextChanged;
                searchbx.GotFocus += Searchbx_GotFocus;
                searchbx.LostFocus += Searchbx_LostFocus;

                // Update progress bar initially
                UpdateProgressBar();
            }
        }

        private void Searchbx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchbx.Text == "Search tasks...")
            {
                searchbx.Text = "";
                searchbx.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void Searchbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchbx.Text))
            {
                searchbx.Text = "Search tasks...";
                searchbx.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        // Add task
        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            if (tasks.Count(t => !t.IsCompleted) >= MaxTasks)
            {
                MessageBox.Show("Please finish at least one task to add more.", "Task Limit", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(titlebx.Text) ||
                string.IsNullOrWhiteSpace(descbx.Text) ||
                prioritybx.SelectedItem == null ||
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
                DueDate = datebx.SelectedDate.Value,
                IsCompleted = false
            };

            tasks.Add(newTask);
            SaveTasks();
            ClearInputs();
            RefreshListView();
            UpdateProgressBar();
        }

        // Mark done
        private void donebtn_Click(object sender, RoutedEventArgs e)
        {
            if (taskListView.SelectedItem is TaskItem task)
            {
                task.IsCompleted = true;
                RefreshListView();
                SaveTasks();
                UpdateProgressBar();
            }
            else
            {
                MessageBox.Show("Select a task first.");
            }
        }

        // Delete task
        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            if (taskListView.SelectedItem is TaskItem task)
            {
                tasks.Remove(task);
                taskDetailsPanel.Children.Clear();
                SaveTasks();
                RefreshListView();
                UpdateProgressBar();
            }
            else
            {
                MessageBox.Show("Select a task to delete.");
            }
        }

        // Show task details
        private void taskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            taskDetailsPanel.Children.Clear();
            if (taskListView.SelectedItem is TaskItem task)
            {
                taskDetailsPanel.Children.Add(new TextBlock
                {
                    Text = $"Title: {task.Title}\n" +
                           $"Description: {task.Description}\n" +
                           $"Priority: {task.Priority}\n" +
                           $"Due: {task.DueDate:d}\n" +
                           $"Status: {(task.IsCompleted ? "Completed" : "Pending")}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10)
                });
            }
        }

        // Search/filter tasks
        private void Searchbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = searchbx.Text.Trim().ToLower();
            CollectionViewSource.GetDefaultView(taskListView.ItemsSource).Filter = obj =>
            {
                if (obj is TaskItem task)
                    return task.Title.ToLower().Contains(search);
                return false;
            };
        }

        private void alltaskbtn_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(taskListView.ItemsSource).Filter = null;
        }

        private void ClearInputs()
        {
            titlebx.Text = "";
            descbx.Text = "";
            prioritybx.SelectedIndex = -1;
            datebx.SelectedDate = null;
        }

        private ObservableCollection<TaskItem> LoadTasks()
        {
            if (!File.Exists(jsonPath))
                return new ObservableCollection<TaskItem>();

            try
            {
                string json = File.ReadAllText(jsonPath);
                return JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(json) ?? new ObservableCollection<TaskItem>();
            }
            catch
            {
                MessageBox.Show("Error reading tasks.json, starting fresh.");
                return new ObservableCollection<TaskItem>();
            }
        }

        private void SaveTasks()
        {
            try
            {
                string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, json);
            }
            catch
            {
                MessageBox.Show("Error saving tasks.");
            }
        }

        private void RefreshListView()
        {
            taskListView.Items.Refresh();
        }

        private void UpdateProgressBar()
        {
            if (tasks.Count == 0)
            {
                progressFill.Width = 0;
                return;
            }

            double percent = tasks.Count(t => t.IsCompleted) / (double)tasks.Count;
            progressFill.Width = percent * 200; // Adjust width according to XAML
        }
    }
}