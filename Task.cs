using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAB301_Assignment3
{
    class Task
    {
        public string TaskId { get; }
        public int TimeNeeded { get; set; }
        public List<string> Dependencies { get; }

        public Task(string taskId, int timeNeeded, List<string> dependencies)
        {
            TaskId = taskId;
            TimeNeeded = timeNeeded;
            Dependencies = dependencies;
        }
    }
}
