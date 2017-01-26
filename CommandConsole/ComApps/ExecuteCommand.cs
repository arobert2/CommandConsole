using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace CommandConsole.ComApps
{
    class CommandEngine : AppTemplate
    {
        public override string Name => "Command Engine";
        /// <summary>
        /// Keyword to execute task. This is the top level application. No keyword.
        /// </summary>
        public override string Keyword => null;
        /// <summary>
        /// Help test.
        /// </summary>
        public override string Help => "This is the top level application. Type listcommands to list all commands.";

        public CommandEngine()
        {
            Status = "Awaiting input.";
            List<ICommand> newcommands = new List<ICommand>();
            newcommands.Add(new ListTasks());
            newcommands.Add(new EnterTask());

            foreach (ICommand ic in newcommands)
                SubCommands.Library.Add(ic.Keyword, ic);
        }
        /// <summary>
        /// Method to execute when command is called.
        /// </summary>
        /// <param name="sender">object that called it.</param>
        /// <param name="c">string that was input to console when called.</param>
        public override void Execute(Object sender, string c)
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
        /// Start a new Application in a seperate thread.
        /// </summary>
        /// <param name="App">Application to start.</param>
        /// <param name="c">input string that triggered the task.</param>
        private void StartApp(IApp App, string c)
        {
            string[] comms = c.Split();
            IApp clone = (IApp)SubCommands.Library[comms[0]];
            clone = clone.Clone();
            TaskSystem.CommandQueue.Add(clone.TaskID, new Queue<string>());
            Task newTask = Task.Run(() => clone.Execute(this, c));
            TaskSystem.RunningTasks.Add(clone.TaskID, newTask);
        }

        public override IApp Clone()
        {
            throw new NotImplementedException();
        }

        public override void Log(string stat)
        {
            //throw new NotImplementedException();
        }
        #region subcommands.
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

                Application.Current.Dispatcher.Invoke(() =>
                {
                    TaskSystem.ActiveTask.Push(commands[1]);
                    TaskSystem.WorkingDirectory = commands[1] + TaskSystem.WorkingDirectory;
                    ConsoleBuffer.Write("Task " + commands[1] + " is active.");
                });
            }
        }
        #endregion
    }
}
