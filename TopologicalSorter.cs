using System;

namespace CAB301_Assignment3
{

    class TopologicalSorter
    {
        public List<Task> TopologicalSort(List<Task> tasks)
        {
            List<Task> sortedTasks = new List<Task>();
            HashSet<Task> visited = new HashSet<Task>();

            foreach (Task task in tasks)
            {
                if (!visited.Contains(task))
                {
                    VisitTaskForTopologicalSort(task, sortedTasks, visited, tasks);
                }
            }

            return sortedTasks;
        }

        private void VisitTaskForTopologicalSort(Task task, List<Task> sortedTasks, HashSet<Task> visited, List<Task> allTasks)
        {
            visited.Add(task);

            foreach (string dependency in task.Dependencies)
            {
                Task dependentTask = allTasks.Find(t => t.TaskId == dependency);
                if (dependentTask != null && !visited.Contains(dependentTask))
                {
                    VisitTaskForTopologicalSort(dependentTask, sortedTasks, visited, allTasks);
                }
            }

            sortedTasks.Add(task);
        }
    }

}
