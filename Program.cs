using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CAB301_Assignment3
{
    class Program
    {
        static void Main()
        {
            TaskManager taskManager = new TaskManager();
            Menu menu = new Menu(taskManager);
            menu.Run();

        }
    }
}