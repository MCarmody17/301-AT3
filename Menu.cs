using System;

namespace CAB301_Assignment3
{
    class Menu
    {
        private TaskManager taskManager;

        public Menu(TaskManager taskManager)
        {
            this.taskManager = taskManager;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("+---------------------------+");
                Console.WriteLine("| Project Management System |");
                Console.WriteLine("+---------------------------+");
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
                        taskManager.LoadTasksFromFile();
                        Console.WriteLine();
                        break;
                    case "2":
                        taskManager.AddNewTask();
                        Console.WriteLine();
                        break;
                    case "3":
                        taskManager.RemoveTask();
                        Console.WriteLine();
                        break;
                    case "4":
                        taskManager.ChangeTaskCompletionTime();
                        Console.WriteLine("Task completion time has been changed.");
                        Console.WriteLine();
                        break;
                    case "5":
                        taskManager.SaveTasksToFile();
                        Console.WriteLine();
                        break;
                    case "6":
                        taskManager.FindTaskSequence();
                        Console.WriteLine();
                        break;
                    case "7":
                        taskManager.FindEarliestTimes();
                        Console.WriteLine();
                        break;
                    case "8":
                        Environment.Exit(0);
                        Console.WriteLine("+---------+");
                        Console.WriteLine("| Goodbye |");
                        Console.WriteLine("+---------+");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.WriteLine();
                        break;
                }
            }
        }
    }
}