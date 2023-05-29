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

        public TaskManager()
        {
            tasks = new List<Task>();
            inputFileName = "";
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Project Management System");
                Console.WriteLine("-------------------------");
                Console.WriteLine("1. Load tasks from a file");
                Console.WriteLine("2. Add a new task");
                Console.WriteLine("3. Remove a task");
                Console.WriteLine("4. Change task completion time");
                Console.WriteLine("5. Save tasks to file");
                Console.WriteLine("6. Find task sequence");
                Console.WriteLine("7. Find earliest task times");
                Console.WriteLine("8. Exit");
                Console.WriteLine();

                Console.Write("Enter your choice (1-8): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LoadTasksFromFile();
                        Console.WriteLine("Tasks have been loaded from the file.");
                        Console.WriteLine();
                        break;
                    case "2":
                        AddNewTask();
                        Console.WriteLine();
                        break;
                    case "3":
                        RemoveTask();
                        Console.WriteLine();
                        break;
                    case "4":
                        ChangeTaskCompletionTime();
                        Console.WriteLine("Task completion time has been changed.");
                        Console.WriteLine();
                        break;
                    case "5":
                        SaveTasksToFile();
                        Console.WriteLine();
                        break;
                    case "6":
                        FindTaskSequence();
                        Console.WriteLine();
                        break;
                    case "7":
                        FindEarliestTimes();
                        Console.WriteLine();
                        break;
                    case "8":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.WriteLine();
                        break;
                }
            }
        }
        //ADD EXCLUDING BLAH BLAH

        private void LoadTasksFromFile()
        {
            Console.Write("Enter the filename you would like to load: ");
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

        private bool ParseTask(string line, out Task task)
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

        private void SaveTasksToFile()
        {
            Console.Write("Enter the filename you would like to save: ");
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

        private void AddNewTask()
        {
            Console.Write("Enter the task information (in the format 'T#, timetocomplete, dependencies'): ");
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




        private void RemoveTask()
        {
            Console.Write("Enter the task ID to remove: ");
            string taskId = Console.ReadLine();

            Task taskToRemove = tasks.Find(task => task.TaskId == taskId);
            if (taskToRemove != null)
            {
                tasks.Remove(taskToRemove);
                //Add "Task removed" line
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }

        private void ChangeTaskCompletionTime()
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

        private void FindTaskSequence()
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

            string fileName = PromptFileName("Enter the filename to save the task sequence: ");
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

        private bool CheckCircularDependency(List<Task> tasks, List<Task> sequence)
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

        private bool VisitTask(Task task, List<Task> sequence, HashSet<Task> visited, HashSet<Task> completed)
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

        private void FindEarliestTimes()
        {
            Dictionary<string, int> earliestTimes = new Dictionary<string, int>();

            // Create a dictionary to store task dependencies
            Dictionary<string, Task> taskDictionary = tasks.ToDictionary(task => task.TaskId, task => task);

            // Perform topological sort to find the earliest times
            List<Task> sortedTasks = TopologicalSort(tasks);
            foreach (Task task in sortedTasks)
            {
                VisitTaskEarliestTimes(task, earliestTimes, taskDictionary);
            }

            // Sort the earliest times by the task ID
            var sortedEarliestTimes = earliestTimes.OrderBy(pair => pair.Key);

            // Save the earliest times to a file
            Console.Write("Enter the filename to save the earliest times: ");
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


        private List<Task> TopologicalSort(List<Task> tasks)
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
        }

        private void VisitTaskForTopologicalSort(Task task, List<Task> sortedTasks, HashSet<Task> visited)
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
        }



        private void VisitTaskEarliestTimes(Task task, Dictionary<string, int> earliestTimes, Dictionary<string, Task> taskDictionary)
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


        private string PromptFileName(string message)
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