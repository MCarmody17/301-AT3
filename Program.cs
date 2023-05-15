using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Task
{
    public string TaskId { get; set; }
    public int TimeNeeded { get; set; }
    public List<string> Dependencies { get; set; }

    public Task(string taskId, int timeNeeded, List<string> dependencies)
    {
        TaskId = taskId;
        TimeNeeded = timeNeeded;
        Dependencies = dependencies;
    }
}

class Program
{
    private static List<Task> tasks = new List<Task>();
    private static string inputFileName = "";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Project Management System");
            Console.WriteLine("-------------------------");
            Console.WriteLine("1. Read tasks from a file");
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
                  
                    ReadTasksFromFile();
                    Console.WriteLine("Tasks have been read from the file.");
                    Console.WriteLine();
                    break;
                case "2":
                    AddNewTask();
                    Console.WriteLine("New task has been added.");
                    Console.WriteLine();
                    break;
                case "3":
                    RemoveTask();
                    Console.WriteLine("Task has been removed.");
                    Console.WriteLine();
                    break;
                case "4":
                    ChangeTaskCompletionTime();
                    Console.WriteLine("Task completion time has been changed.");
                    Console.WriteLine();
                    break;
                case "5":
                    SaveTasksToFile();
                    Console.WriteLine("Tasks have been saved to the file.");
                    Console.WriteLine();
                    break;
                case "6":
                    FindTaskSequence();
                    Console.WriteLine();
                    break;
                case "7":
                    FindEarliestTimes();
                    Console.WriteLine("Earliest task times have been found and saved to EarliestTimes.txt.");
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

    static void ReadTasksFromFile()
    {
        tasks.Clear();

        Console.Write("Enter the name of the text file: ");
        string fileName = Console.ReadLine();

        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');

                    string taskId = parts[0].Trim();
                    int timeNeeded = int.Parse(parts[1].Trim());

                    List<string> dependencies = new List<string>();
                    for (int i = 2; i < parts.Length; i++)
                    {
                        dependencies.Add(parts[i].Trim());
                    }

                    Task task = new Task(taskId, timeNeeded, dependencies);
                    tasks.Add(task);
                }
            }

            // Display the tasks
            Console.WriteLine("Task List:");
            foreach (Task task in tasks)
            {
                Console.WriteLine($"Task ID: {task.TaskId}, Time Needed: {task.TimeNeeded}, Dependencies: {string.Join(", ", task.Dependencies)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while reading the file: " + ex.Message);
        }
    }


    static void AddNewTask()
    {
        Console.Write("Enter the task ID: ");
        string taskId = Console.ReadLine();

        Console.Write("Enter the time needed to complete the task: ");
        int timeNeeded = int.Parse(Console.ReadLine());

        Console.Write("Enter the dependencies (comma-separated): ");
        string[] dependencyArray = Console.ReadLine().Split(',');

        List<string> dependencies = new List<string>();
        foreach (string dependency in dependencyArray)
        {
            dependencies.Add(dependency.Trim());
        }

        Task task = new Task(taskId, timeNeeded, dependencies);
        tasks.Add(task);
    }

    static void RemoveTask()
    {
        Console.Write("Enter the task ID to remove: ");
        string taskId = Console.ReadLine();

        Task taskToRemove = tasks.Find(task => task.TaskId == taskId);
        if (taskToRemove != null)
        {
            tasks.Remove(taskToRemove);
        }
        else
        {
            Console.WriteLine("Task not found.");
        }
    }

    static void ChangeTaskCompletionTime()
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

    static void SaveTasksToFile()
    {
        Console.Write("Enter the filename to save the tasks: ");
        string fileName = Console.ReadLine();
        fileName += ".txt";


        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (Task task in tasks)
                {
                    string line = task.TaskId + ", " + task.TimeNeeded;
                    if (task.Dependencies.Count > 0)
                    {
                        line += ", " + string.Join(", ", task.Dependencies);
                    }
                    writer.WriteLine(line);
                }
            }
            Console.WriteLine("Tasks have been saved to the file: " + fullPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving the file: " + ex.Message);
        }
    }
    static void FindTaskSequence()
    {
        List<Task> sequence = new List<Task>();

        // Create a dictionary to store task dependencies
        Dictionary<string, Task> taskDictionary = tasks.ToDictionary(task => task.TaskId, task => task);

        // Perform depth-first search to build the task sequence
        foreach (Task task in tasks)
        {
            bool success = VisitTask(task, sequence, taskDictionary, new HashSet<string>());
            if (!success)
            {
                Console.WriteLine("Error: Circular dependency detected involving task " + task.TaskId);
                return;
            }
        }

        // Check if there are any remaining tasks with unresolved dependencies
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

        // Save the task sequence to a file
        Console.Write("Enter the filename to save the task sequence: ");
        string fileName = Console.ReadLine();
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (Task task in sequence)
                {
                    writer.WriteLine(task.TaskId);
                }
            }
            Console.WriteLine("Task sequence has been saved to the file: " + fullPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving the file: " + ex.Message);
        }
    }

    static bool VisitTask(Task task, List<Task> sequence, Dictionary<string, Task> taskDictionary, HashSet<string> visited)
    {
        if (visited.Contains(task.TaskId))
        {
            return false; // Circular dependency detected
        }

        visited.Add(task.TaskId);

        foreach (string dependency in task.Dependencies)
        {
            if (taskDictionary.ContainsKey(dependency))
            {
                Task dependentTask = taskDictionary[dependency];
                bool success = VisitTask(dependentTask, sequence, taskDictionary, visited);
                if (!success)
                {
                    return false; // Propagate circular dependency detection
                }
            }
        }

        if (!sequence.Contains(task))
        {
            sequence.Add(task);
        }

        visited.Remove(task.TaskId);

        return true;
    }


    static void FindEarliestTimes()
    {
        Dictionary<string, int> earliestTimes = new Dictionary<string, int>();

        // Create a dictionary to store task dependencies
        Dictionary<string, Task> taskDictionary = tasks.ToDictionary(task => task.TaskId, task => task);

        // Perform depth-first search to find the earliest times
        foreach (Task task in tasks)
        {
            VisitTaskEarliestTimes(task, earliestTimes, taskDictionary);
        }

        // Check if there are any remaining tasks with unresolved dependencies
        List<Task> unresolvedTasks = tasks.Where(task => !earliestTimes.ContainsKey(task.TaskId)).ToList();
        if (unresolvedTasks.Count > 0)
        {
            Console.WriteLine("Error: Dependency not found for tasks:");
            foreach (Task task in unresolvedTasks)
            {
                Console.WriteLine(task.TaskId);
            }
            return;
        }

        // Save the earliest times to a file
        Console.Write("Enter the filename to save the earliest times: ");
        string fileName = Console.ReadLine();
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (var pair in earliestTimes)
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

    static void VisitTaskEarliestTimes(Task task, Dictionary<string, int> earliestTimes, Dictionary<string, Task> taskDictionary)
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
            earliestTimes.Add(task.TaskId, earliestTime);
        }
    }



    static void CalculateEarliestTime(Task task, Dictionary<string, int> earliestTimes)
    {
        int maxDependencyTime = 0;

        foreach (string dependency in task.Dependencies)
        {
            if (earliestTimes.ContainsKey(dependency))
            {
                int dependencyTime = earliestTimes[dependency];
                if (dependencyTime > maxDependencyTime)
                {
                    maxDependencyTime = dependencyTime;
                }
            }
            else
            {
                Console.WriteLine("Error: Dependency not found for task " + task.TaskId);
                return;
            }
        }

        int earliestTime = maxDependencyTime + task.TimeNeeded;
        earliestTimes[task.TaskId] = earliestTime;
    }
}
