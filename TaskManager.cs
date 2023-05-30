using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAB301_Assignment3
{
    class TaskManager
    {
        private List<Task> tasks;
        private string inputFileName;
        private TopologicalSorter sorter;

        public TaskManager()
        {
            sorter = new TopologicalSorter();
            tasks = new List<Task>();
            inputFileName = "";
        }

        public void LoadTasksFromFile()
        {
            Console.Write("Enter the filename you would like to load (Excluding File Extension e.g. .txt): ");
            string fileName = Console.ReadLine() + ".txt";

            try
            {
                string[] lines = File.ReadAllLines(fileName);
                tasks.Clear();

                foreach (string line in lines)
                {
                    bool success = ParseTask(line, out Task task);
                    if (!success)
                    {
                        Console.WriteLine("Error: Invalid task format in the file: " + line);
                        return;
                    }

                    tasks.Add(task);
                }

                Console.WriteLine("Tasks loaded successfully from the file: " + fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while loading tasks from the file: " + ex.Message);
            }
        }

        public bool ParseTask(string line, out Task task)
        {
            task = null;

            string[] parts = line.Split(',');

            if (parts.Length < 2)
                return false;

            string taskId = parts[0].Trim();

            if (!int.TryParse(parts[1].Trim(), out int timeNeeded))
                return false;

            List<string> dependencies = new List<string>();

            for (int i = 2; i < parts.Length; i++)
            {
                dependencies.Add(parts[i].Trim());
            }

            task = new Task(taskId, timeNeeded, dependencies);
            return true;
        }

        public void SaveTasksToFile()
        {
            Console.Write("Enter the filename you would like to save (Excluding File Extension e.g. .txt): ");
            string fileName = Console.ReadLine() + ".txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (Task task in tasks)
                    {
                        writer.Write(task.TaskId + ", " + task.TimeNeeded);

                        if (task.Dependencies.Count > 0)
                        {
                            writer.Write(", " + string.Join(", ", task.Dependencies));
                        }

                        writer.WriteLine();
                    }
                }

                Console.WriteLine("Tasks saved successfully to the file: " + fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while saving tasks to the file: " + ex.Message);
            }
        }

        public void AddNewTask()
        {
            Console.Write("Enter the task information (in the format 'Task, timetocomplete, dependencies'): ");
            string input = Console.ReadLine();

            string[] taskData = input.Split(',');

            if (taskData.Length < 2)
            {
                Console.WriteLine("Invalid task information. Please try again.");
                return;
            }

            string taskId = taskData[0].Trim();
            int timeNeeded;

            if (!int.TryParse(taskData[1].Trim(), out timeNeeded))
            {
                Console.WriteLine("Invalid time needed. Please enter a valid number.");
                return;
            }

            List<string> dependencies = new List<string>();

            for (int i = 2; i < taskData.Length; i++)
            {
                dependencies.Add(taskData[i].Trim());
            }

            Task newTask = new Task(taskId, timeNeeded, dependencies);
            tasks.Add(newTask);

            Console.WriteLine("Task added successfully!");
        }


        public void RemoveTask()
        {
            Console.Write("Enter the task ID to remove: ");
            string taskId = Console.ReadLine();

            Task taskToRemove = tasks.Find(task => task.TaskId == taskId);
            if (taskToRemove != null)
            {
                tasks.Remove(taskToRemove);
                Console.Write("Task Removed Successfully ");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        public void ChangeTaskCompletionTime()
        {
            Console.Write("Enter the task ID to change completion time: ");
            string taskId = Console.ReadLine();

            Console.Write("Enter the new completion time: ");
            int newTimeNeeded = int.Parse(Console.ReadLine());

            Task taskToUpdate = tasks.Find(task => task.TaskId == taskId);
            if (taskToUpdate != null)
            {
                taskToUpdate.TimeNeeded = newTimeNeeded;
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        public void FindTaskSequence()
        {
            List<Task> sequence = new List<Task>();
            bool success = CheckCircularDependency(tasks, sequence);

            if (!success)
            {
                Console.WriteLine("Error: Circular dependency detected.");
                return;
            }

            List<Task> unresolvedTasks = tasks.Where(task => !sequence.Contains(task)).ToList();
            if (unresolvedTasks.Count > 0)
            {
                Console.WriteLine("Error: Dependency not found for tasks:");
                foreach (Task task in unresolvedTasks)
                {
                    Console.WriteLine(task.TaskId);
                }
                return;
            }

            string fileName = PromptFileName("Enter the filename to save the task sequence (Excluding File Extension e.g. .txt): ");
            if (fileName != null)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName + ".txt");

                try
                {
                    using (StreamWriter writer = new StreamWriter(fullPath))
                    {
                        string sequenceString = string.Join(", ", sequence.Select(task => task.TaskId));
                        writer.WriteLine(sequenceString);
                    }
                    Console.WriteLine("Task sequence has been saved to the file: " + fullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while saving the file: " + ex.Message);
                }
            }
        }

        public bool CheckCircularDependency(List<Task> tasks, List<Task> sequence)
        {
            HashSet<Task> visited = new HashSet<Task>();
            HashSet<Task> completed = new HashSet<Task>();

            foreach (Task task in tasks)
            {
                bool success = VisitTask(task, sequence, visited, completed);
                if (!success)
                {
                    return false;
                }
            }

            return true;
        }

        public bool VisitTask(Task task, List<Task> sequence, HashSet<Task> visited, HashSet<Task> completed)
        {
            if (completed.Contains(task))
            {
                return true; // Task already completed, no need to visit again
            }

            if (visited.Contains(task))
            {
                return false; // Circular dependency detected
            }

            visited.Add(task);

            foreach (string dependency in task.Dependencies)
            {
                Task dependentTask = tasks.Find(t => t.TaskId == dependency);
                if (dependentTask != null)
                {
                    bool success = VisitTask(dependentTask, sequence, visited, completed);
                    if (!success)
                    {
                        return false; // Propagate circular dependency detection
                    }
                }
                else
                {
                    Console.WriteLine("Error: Dependency not found for task " + dependency);
                    return false;
                }
            }

            sequence.Add(task);
            completed.Add(task);
            visited.Remove(task);

            return true;
        }

        public void FindEarliestTimes()
        {
            Dictionary<string, int> earliestTimes = new Dictionary<string, int>();

            // Create a dictionary to store task dependencies
            Dictionary<string, Task> taskDictionary = tasks.ToDictionary(task => task.TaskId, task => task);

            // Perform topological sort to find the earliest times
            List<Task> sortedTasks = sorter.TopologicalSort(tasks);
            foreach (Task task in sortedTasks)
            {
                VisitTaskEarliestTimes(task, earliestTimes, taskDictionary);
            }

            // Sort the earliest times by the task ID
            var sortedEarliestTimes = earliestTimes.OrderBy(pair => pair.Key);

            // Save the earliest times to a file
            Console.Write("Enter the filename to save the earliest times (Excluding File Extension e.g. .txt): ");
            string fileName = Console.ReadLine();
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName + ".txt");

            try
            {
                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    foreach (var pair in sortedEarliestTimes)
                    {
                        writer.WriteLine(pair.Key + ", " + pair.Value);
                    }
                }
                Console.WriteLine("Earliest times have been saved to the file: " + fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while saving the file: " + ex.Message);
            }
        }


        /*        public List<Task> TopologicalSort(List<Task> tasks)
                {
                    List<Task> sortedTasks = new List<Task>();
                    HashSet<Task> visited = new HashSet<Task>();

                    foreach (Task task in tasks)
                    {
                        if (!visited.Contains(task))
                        {
                            VisitTaskForTopologicalSort(task, sortedTasks, visited);
                        }
                    }

                    return sortedTasks;
                }*/

        /*public void VisitTaskForTopologicalSort(Task task, List<Task> sortedTasks, HashSet<Task> visited)
        {
            visited.Add(task);

            foreach (string dependency in task.Dependencies)
            {
                Task dependentTask = tasks.Find(t => t.TaskId == dependency);
                if (dependentTask != null && !visited.Contains(dependentTask))
                {
                    VisitTaskForTopologicalSort(dependentTask, sortedTasks, visited);
                }
            }

            sortedTasks.Add(task);
        }*/


        public void VisitTaskEarliestTimes(Task task, Dictionary<string, int> earliestTimes, Dictionary<string, Task> taskDictionary)
        {
            if (!earliestTimes.ContainsKey(task.TaskId))
            {
                // Find the earliest times of the task dependencies
                int maxDependencyTime = 0;
                foreach (string dependency in task.Dependencies)
                {
                    if (taskDictionary.ContainsKey(dependency))
                    {
                        Task dependentTask = taskDictionary[dependency];
                        VisitTaskEarliestTimes(dependentTask, earliestTimes, taskDictionary);
                        if (earliestTimes.ContainsKey(dependency))
                        {
                            int dependencyTime = earliestTimes[dependency] + dependentTask.TimeNeeded;
                            maxDependencyTime = Math.Max(maxDependencyTime, dependencyTime);
                        }
                    }
                }

                // Calculate the earliest time for the current task
                int earliestTime = maxDependencyTime;
                earliestTimes[task.TaskId] = earliestTime;
            }
        }


        public string PromptFileName(string message)
        {
            Console.Write(message);
            string fileName = Console.ReadLine();
            if (string.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Invalid filename.");
                return null;
            }
            return fileName;
        }
    }


}