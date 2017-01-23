using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CommandConsole
{
    public static class TaskSystem
    {
        /// <summary>
        /// Input token.
        /// </summary>
        public static string WorkingDirectory { get; set; } = ">> ";
        /// <summary>
        /// Task currently being manipulated.
        /// </summary>
        public static Stack<string> ActiveTask { get; set; } = new Stack<string>();
        /// <summary>
        /// CurrentLibrary
        /// </summary>
        public static CommandLibrary CurrentLibrary { get; set; }
        /// <summary>
        /// Current running tasks.
        /// </summary>
        public static Dictionary<string, Task> RunningTasks = new Dictionary<string, Task>();
        /// <summary>
        /// Command Queue.
        /// </summary>
        public static Dictionary<string, Queue<string>> CommandQueue = new Dictionary<string, Queue<string>>();
        /// <summary>
        /// Enter a command.
        /// </summary>
        /// <param name="c"></param>
        public static void EnterCommand(string c)
        {
            CommandQueue[ActiveTask.Peek()].Enqueue(c);
        }
    }
}
