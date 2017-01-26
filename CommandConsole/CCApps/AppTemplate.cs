using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommandConsole.CCApps
{
    public abstract class AppTemplate : IApp
    {
        private static int _id = 0;
        public virtual string Name { get; }
        /// <summary>
        /// ID of task
        /// </summary>
        public string TaskID { get; set; }
        /// <summary>
        /// Keyword to execute task. This is the top level application. No keyword.
        /// </summary>
        public virtual string Keyword { get; }
        /// <summary>
        /// Help test.
        /// </summary>
        public virtual string Help { get; }
        /// <summary>
        /// Whether this is a command or an app. Apps run in seperate tasks.
        /// </summary>
        public CommandType Type => CommandType.App;
        /// <summary>
        /// Commands that this application can run.
        /// </summary>
        public CommandLibrary SubCommands { get; set; } = new CommandLibrary();
        /// <summary>
        /// Change this bool to stop the thread.
        /// </summary>
        public bool StopApp { get; set; } = false;
        /// <summary>
        /// Current status of command
        /// </summary>
        public string Status { get; private set; }

        public AppTemplate()
        {
            _id++;
            TaskID = _id.ToString("X");
            List<ICommand> newcommands = new List<ICommand>();
            newcommands.Add(new Helpc());
            newcommands.Add(new ListCommands());
            newcommands.Add(new PrintInfo());
            newcommands.Add(new Exit());

            foreach (ICommand ic in newcommands)
                SubCommands.Library.Add(ic.Keyword, ic);
        }
        /// <summary>
        /// Method to execute when command is called.
        /// </summary>
        /// <param name="sender">object that called it.</param>
        /// <param name="c">string that was input to console when called.</param>
        public abstract void Execute(Object sender, string c);
        /// <summary>
        /// Checks for instructions from the console.
        /// </summary>
        public string CheckForInstruction()
        {
            if (TaskSystem.CommandQueue[TaskID].Count == 0)
                return null;
            string rawinput = TaskSystem.CommandQueue[TaskID].Peek();
            Application.Current.Dispatcher.Invoke(() => TaskSystem.CommandQueue[TaskID].Dequeue());
            return rawinput;
        }
        /// <summary>
        /// Logs information to a file.
        /// </summary>
        public abstract void Log();
        /// <summary>
        /// Clone a copy of this object with an updated TaskID;
        /// </summary>
        /// <returns>IApp Clone</returns>
        public abstract IApp Clone();
        #region subcommands.
        /// <summary>
        /// Exit command.
        /// </summary>
        public class Exit : ICommand
        {
            /// <summary>
            /// keyword to run.
            /// </summary>
            public string Keyword => "exit";
            /// <summary>
            /// Help data for this command
            /// </summary>
            public string Help => "Exits this task.";
            /// <summary>
            /// Command or App
            /// </summary>
            public CommandType Type => CommandType.Command;
            /// <summary>
            /// Execution method
            /// </summary>
            /// <param name="sender">object that flag this.</param>
            /// <param name="c">input commandline.</param>
            public void Execute(object sender, string c)
            {
                ((CommandEngine)sender).StopApp = true;
                Application.Current.Dispatcher.Invoke(() => TaskSystem.ActiveTask.Pop());
            }
        }
        /// <summary>
        /// prints the TaskID and Name of the application.
        /// </summary>
        public class PrintInfo : ICommand
        {
            /// <summary>
            /// keyword
            /// </summary>
            public string Keyword => "printinfo";
            /// <summary>
            /// Help text
            /// </summary>
            public string Help => "gets the TaskID and Name of the Application";
            /// <summary>
            /// Command or Application
            /// </summary>
            public CommandType Type => CommandType.Command;
            /// <summary>
            /// Execution method.
            /// </summary>
            /// <param name="sender">parent task</param>
            /// <param name="c">input text</param>
            public void Execute(object sender, string c)
            {
                IApp parentapp = (IApp)sender;
                Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Write(parentapp.TaskID + "           " + parentapp.Name));
            }
        }
        /// <summary>
        /// List all currently available commands
        /// </summary>
        public class ListCommands : ICommand
        {
            /// <summary>
            /// keyword
            /// </summary>
            public string Keyword => "listcommands";
            /// <summary>
            /// Help text
            /// </summary>
            public string Help => "Lists all current available commands.";
            /// <summary>
            /// Command type
            /// </summary>
            public CommandType Type => CommandType.Command;
            /// <summary>
            /// Execution
            /// </summary>
            /// <param name="sender">parent app</param>
            /// <param name="c">commands</param>
            public void Execute(object sender, string c)
            {
                IApp parentapp = (IApp)sender;
                foreach (ICommand ic in parentapp.SubCommands.Library.Values)
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Write(ic.Keyword + "                " + ic.Type.ToString()));
            }
        }
        /// <summary>
        /// Displays help text.
        /// </summary>
        public class Helpc : ICommand
        {
            public string Keyword => "help";
            public string Help => "Lists help information. (ex: help listcommands)";
            public CommandType Type => CommandType.Command;
            public void Execute(object sender, string c)
            {
                string[] queue = c.Split();
                IApp parentapp = (IApp)sender;
                if (queue.Length == 0)
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Not enough parameters."));
                    return;
                }

                if (queue.Length == 1)
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Help(parentapp.Help));
                    return;
                }

                if (!parentapp.SubCommands.Library.ContainsKey(queue[1]))
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Command " + queue[1] + " not found!"));
                    return;
                }

                ICommand helpcom = parentapp.SubCommands.Library[queue[1]];
                Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Help(helpcom.Help));
            }
        }
        #endregion
    }
}

