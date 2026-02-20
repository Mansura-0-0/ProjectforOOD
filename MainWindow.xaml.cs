using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Project
{
    public partial class MainWindow : Window
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private const int MaxTasks = 5;
        private ListBox taskListBox;
        private StackPanel taskdetail;

        public MainWindow()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // Only add dynamic tasks at runtime
                // Example:
                // tasklist.Children.Add(new TaskControl(...));


                prioritybx.ItemsSource = new List<string> { "Low", "Medium", "High" };

                // Initialize taskdetail StackPanel
                taskdetail = new StackPanel
                {
                    Margin = new Thickness(5)
                };
                // Add taskdetail to your layout (for example, to a parent panel)
                // Replace 'parentPanel' with the actual parent container in your XAML
                tasklist.Children.Add(taskdetail);


                // ONE ListBox for Task List
                taskListBox = new ListBox
                {
                    Margin = new Thickness(5)
                };
                //this shows the details of the task when clicked
                taskListBox.SelectionChanged += TaskListBox_SelectionChanged;
                tasklist.Children.Add(taskListBox);

                RefreshTaskList();
            }
        }

        // When a task is clickked
        private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear previouos details
            taskdetail.Children.Clear();
            // Show details of selected task
            if (taskListBox.SelectedItem is TaskItem task)
            {
                // Display task detiails in the task list pannel
                tasklist.Children.Add(new TextBlock
                {
                    Text =
                        $"Title: {task.Title}\n\n" +
                        $"Description:\n{task.Description}\n\n" +
                        $"Priority: {task.Priority}\n" +
                        $"Due Date: {task.DueDate:d}\n" +
                        $"Status: {(task.IsCompleted ? "Completed" : "Pending")}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10)
                });
            }
        }

        private void alltaskbtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaskList();
        }

        private void addbtn_Click(object sender, RoutedEventArgs e)
        {
            //this checks if there are already 5 tasks that are not completted, if so it will show a message box and return
            if (tasks.Count(t => !t.IsCompleted) >= MaxTasks)
            {
                MessageBox.Show(
                    "Please finish at least one task to add more.",
                    "Task Limit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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
            //this adds a new task to the list with the details from the input fields, then it clears the inputs and refreshes the task list
            tasks.Add(new TaskItem
            {
                Title = titlebx.Text,
                Description = descbx.Text,
                Priority = prioritybx.SelectedItem.ToString(),
                DueDate = datebx.SelectedDate.Value,
                IsCompleted = false
            });

            ClearInputs();
            RefreshTaskList();
        }

        private void donebtn_Click(object sender, RoutedEventArgs e)
        {
            if (taskListBox.SelectedItem is TaskItem task)
            {
                task.IsCompleted = true;
                RefreshTaskList();
                TaskListBox_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Select a task first.");
            }
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            if (taskListBox.SelectedItem is TaskItem task)
            {
                tasks.Remove(task);
                taskdetail.Children.Clear();
                RefreshTaskList();
            }
            else
            {
                MessageBox.Show("Select a task to delete.");
            }
            //when its deteted it will clear the details and refresh the task details
            taskdetail.Children.Clear();







        }

        private void RefreshTaskList()
        {
            //this refreshes the task list by clearing the items source and then setting it again to the current list of tasks, if there are no tasks it will show a message and disable the listbox
            taskListBox.ItemsSource = null;

            // Show only pending tasks in the list
            if (tasks.Count == 0)
            {
                taskListBox.ItemsSource = new List<string> { "No task added yet" };
                taskListBox.IsEnabled = false;
            }
            else
            {
                taskListBox.IsEnabled = true;
                taskListBox.ItemsSource = tasks;
            }
        }

        private void ClearInputs()
        {
            //this clears the input fields after adding a task
            titlebx.Clear();
            descbx.Clear();
            prioritybx.SelectedIndex = -1;
            datebx.SelectedDate = null;
        }
    }
}
