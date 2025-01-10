using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA_Pixelizacja_Obrazu
{
    internal class Threads
    {
        List<Task> tasks;

        public Threads()
        {
            tasks = new List<Task>();
        } 
    
        public void AddTask(Action action)
        {
            tasks.Add(Task.Run(action));
        }
    }
}
