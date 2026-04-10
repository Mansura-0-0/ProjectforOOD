using System.Data.SQLite;
using System;
using System.Collections.Generic;

namespace Project
{
    public class Database
    {
        private string connectionString = "Data Source=tasks.db";

        public void Initialize()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();

                command.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS Tasks(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Description TEXT,
                    Priority TEXT,
                    Category TEXT,
                    DueDate TEXT,
                    IsCompleted INTEGER
                );
                ";

                command.ExecuteNonQuery();
            }
        }

        public List<TaskItem> GetTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Tasks";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            Priority = reader.GetString(3),
                            Category = reader.GetString(4),
                            DueDate = DateTime.Parse(reader.GetString(5)),
                            IsCompleted = reader.GetInt32(6) == 1
                        });
                    }
                }
            }

            return tasks;
        }

        public void AddTask(TaskItem task)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();

                command.CommandText =
                @"
                INSERT INTO Tasks 
                (Title,Description,Priority,Category,DueDate,IsCompleted)
                VALUES (@title,@desc,@priority,@category,@date,@done);
                SELECT last_insert_rowid();
                ";

                command.Parameters.AddWithValue("@title", task.Title);
                command.Parameters.AddWithValue("@desc", task.Description);
                command.Parameters.AddWithValue("@priority", task.Priority);
                command.Parameters.AddWithValue("@category", task.Category);
                command.Parameters.AddWithValue("@date", task.DueDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@done", task.IsCompleted ? 1 : 0);

                task.Id = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void DeleteTask(int id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Tasks WHERE Id=@id";
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        public void CompleteTask(int id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Tasks SET IsCompleted=1 WHERE Id=@id";
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }
    }
}