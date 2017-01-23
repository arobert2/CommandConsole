using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommandConsole
{
    class CommandEngine : IApp
    {
        private int _id;
        public string Name => "Command Engine";
        /// <summary>
        /// ID of task
        /// </summary>
        public string TaskID { get; set; }
        /// <summary>
        /// Keyword to execute task. This is the top level application. No keyword.
        /// </summary>
        public string Keyword => null;
        /// <summary>
        /// Help test.
        /// </summary>
        public string Help => "This is the top level application. Type listcommands to list all commands.";
        /// <summary>
        /// Whether this is a command or an app. Apps run in seperate tasks.
        /// </summary>
        public CommandType Type => CommandType.App;
        protected static CommandLibrary _subcommands = new CommandLibrary();
        /// <summary>
        /// Commands that this application can run.
        /// </summary>
        public CommandLibrary SubCommands { get { return _subcommands; } set { _subcommands = value; } }
        /// <summary>
        /// Change this bool to stop the thread.
        /// </summary>
        public bool StopApp { get; set; } = false;
        /// <summary>
        /// Current status of command
        /// </summary>
        public string Status { get; private set; } = "Waiting for input.";

        protected CommandEngine()
        {
            List<ICommand> newcommands = new List<ICommand>();
            newcommands.Add(new Helpc());
            newcommands.Add(new ListCommands());
            newcommands.Add(new PrintInfo());
            newcommands.Add(new Exit());

            foreach (ICommand ic in newcommands)
                SubCommands.Library.Add(ic.Keyword, ic);
        }
        /// <summary>
        /// Create a new command shell
        /// </summary>
        /// <param name="id"></param>
        public CommandEngine(int id)
        {
            _id = id;
            TaskID = id.ToString("X");

            List<ICommand> newcommands = new List<ICommand>();
            newcommands.Add(new Helpc());
            newcommands.Add(new ListCommands());
            newcommands.Add(new ListTasks());
            newcommands.Add(new EnterTask());
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
        public void Execute(Object sender, string c)
        {
            while(!StopApp)
            {
                string newinstruction = CheckForInstruction();
                string[] queue = newinstruction?.Split();
                if (queue != null)
                {
                    if (SubCommands.Library.ContainsKey(queue[0]))
                    {
                        switch (SubCommands.Library[queue[0]].Type)
                        {
                            case CommandType.Command:
                                SubCommands.Library[queue[0]].Execute(this, newinstruction);
                                break;
                            case CommandType.App:
                                StartApp(this, newinstruction);
                                break;
                        }
                    }
                    else
                        Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Command " + queue[0] + " not found!"));
                }
            }
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }
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
        /// Start a new Application in a seperate thread.
        /// </summary>
        /// <param name="App">Application to start.</param>
        /// <param name="c">input string that triggered the task.</param>
        private void StartApp(IApp App, string c)
        {
            _id++;
            App.TaskID = _id.ToString("X");
            Task newTask = Task.Run(() => App.Execute(this, c));
            TaskSystem.RunningTasks.Add(App.TaskID, newTask);
            TaskSystem.CommandQueue.Add(App.TaskID, new Queue<string>());     
        }

        public void Log() { }
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
        /// Queues all running tasks to print TaskID and Name.
        /// </summary>
        private class ListTasks : ICommand
        {
            /// <summary>
            /// keyword to run.
            /// </summary>
            public string Keyword => "listtasks";
            /// <summary>
            /// Help data for this command
            /// </summary>
            public string Help => "Lists all running tasks.";
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
                foreach (string k in TaskSystem.RunningTasks.Keys)
                    Application.Current.Dispatcher.Invoke(() => TaskSystem.CommandQueue[k].Enqueue("printinfo"));
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
        /// 
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
                if(queue.Length == 0)
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Not enough parameters."));
                    return;
                }

                if(queue.Length == 1)
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Help(parentapp.Help));
                    return;
                }

                if(!parentapp.SubCommands.Library.ContainsKey(queue[1]))
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Command " + queue[1] + " not found!"));
                    return;
                }

                ICommand helpcom = parentapp.SubCommands.Library[queue[1]];
                Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Help(helpcom.Help));
            }
        }
        /// <summary>
        /// Enters a task to input commands
        /// </summary>
        private class EnterTask : ICommand
        {
            /// <summary>
            /// command keyword
            /// </summary>
            public string Keyword => "enter";
            /// <summary>
            /// Help text
            /// </summary>
            public string Help => "Enters an Application to interact directly.";
            /// <summary>
            /// Type of command
            /// </summary>
            public CommandType Type => CommandType.Command;
            /// <summary>
            /// Method to execute. 
            /// </summary>
            /// <param name="sender">parent app</param>
            /// <param name="c">command input string</param>
            public void Execute(object sender, string c)
            {
                string[] commands = c.Split();

                if(commands.Length < 2)
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Not enough parameters."));
                    return;
                }

                if (!TaskSystem.RunningTasks.ContainsKey(commands[1]))
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Task " + commands[1] + " does not exist."));
                    return;
                }

                if (TaskSystem.ActiveTask.Peek() == commands[1])
                {
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Write("Task " + commands[1] + " already active."));
                    return;
                }

                TaskSystem.ActiveTask.Push(commands[1]);
                TaskSystem.WorkingDirectory = commands[1] + TaskSystem.WorkingDirectory;
                Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Write("Task " + commands[1] + " is active."));
            }
        }
        #endregion
    }
}
