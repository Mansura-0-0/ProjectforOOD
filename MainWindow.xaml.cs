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
        //declare max tasks constant
        private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();
        private string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");
        private ICollectionView tasksView;

        public MainWindow()
        {
            InitializeComponent();

            
        }

        //serchbox focus events
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


        // Add task
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
        //this should be showin in task list view $$$$$$$$$
        private void taskListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


          
          

            //but if i want to show in task list view then how can i do that?
            if (taskListView.SelectedItem is TaskItem selectedTask)
            {
                // Update the ListView item to show details
                int index = taskListView.Items.IndexOf(selectedTask);
                if (index >= 0)
                {
                    ListViewItem item = (ListViewItem)taskListView.ItemContainerGenerator.ContainerFromIndex(index);
                    if (item != null)
                    {
                        item.Content = $"{selectedTask.Title} - {selectedTask.Description} (Due: {selectedTask.DueDate:d}) {selectedTask.Priority}";
                    }
                }
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
        //updated
        private void alltaskbtn_Click(object sender, RoutedEventArgs e)
        {
            if (tasksView == null) return;
            tasksView.Filter = null;
        }

        private void ClearInputs()
        {
            titlebx.Text = "";
            descbx.Text = "";
            prioritybx.SelectedIndex = -1;
            datebx.SelectedDate = null;
        }


        //updated
        private ObservableCollection<TaskItem> LoadTasks()
        {
            if (!File.Exists(jsonPath))
                return new ObservableCollection<TaskItem>();

            string json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(json);
        }


        //updated
        private void SaveTasks()
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, json);
        }
        

        private void RefreshListView()
        {
            taskListView.Items.Refresh();
        }
        //updated
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