using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommandConsole
{
    /// <summary>
    /// Singleton instance of CommandLibrary to serve as the top level library
    /// </summary>
    public class TopLevelLibrary
    {
        /// <summary>
        /// Top level library
        /// </summary>
        public static CommandLibrary Instance { get; set; }
        /// <summary>
        /// Initialize library.
        /// </summary>
        public TopLevelLibrary()
        {
            Instance = Instance == null ? new CommandLibrary() : Instance;
        }
    }
    /// <summary>
    /// Command Library
    /// </summary>
    public class CommandLibrary
    {
        /// <summary>
        /// library of commands
        /// </summary>
        public Dictionary<string, ICommand> Library { get; set; }

        public CommandLibrary()
        {
            Library = new Dictionary<string, ICommand>();
            ICommand c = new ListCommands();
            Library.Add(c.Keyword, c);
            c = new Helpc();
            Library.Add(c.Keyword, c);
            c = new Exit();
            Library.Add(c.Keyword, c);
        }

        /// <summary>
        /// List all commands available. Command formated to add to CommandLibrary.
        /// </summary>
        public class ListCommands : ICommand
        {
            /// <summary>
            /// Help information
            /// </summary>
            public string Help => "Lists all available commands";
            /// <summary>
            /// keyword used for call
            /// </summary>
            public string Keyword => "listcommands";
            /// <summary>
            /// Command type. (Command or App. Apps run on their own thread.)
            /// </summary>
            public CommandType Type => CommandType.Command;
            /// <summary>
            /// Method that will be executed.
            /// </summary>
            /// <param name="o">generic parameter</param>
            public void Execute(object o, string s)
            {
                ConsoleBuffer buffer = new ConsoleBuffer();
                MainWindow main = (MainWindow)o;
                CommandLibrary lib = main.CurrentLibrary;
                foreach(ICommand c in lib.Library.Values)
                {
                    string gap = "                                          ";
                    gap = gap.Substring(c.Keyword.Length);
                    System.Diagnostics.Debug.WriteLine(gap.Length);
                    buffer.Write(c.Keyword + gap + c.Type.ToString());
                }
            }
        }
        /// <summary>
        /// displays the help information.
        /// </summary>
        public class Helpc : ICommand
        {
            /// <summary>
            /// Help information
            /// </summary>
            public string Help => "Get help info for all commands. (ex: help listcommands)";
            /// <summary>
            /// keyword used for call
            /// </summary>
            public string Keyword => "help";
            /// <summary>
            /// Type of command (Command or App. Apps run on their own threads.);
            /// </summary>
            public CommandType Type => CommandType.Command;
            /// <summary>
            /// Method that will be executed
            /// </summary>
            /// <param name="o">Command that will be help.</param>
            public void Execute(object o, string c)
            {
                ConsoleBuffer buffer = new ConsoleBuffer();
                if (c.Split().Length < 2)
                {
                    buffer.Error("Not enough parameters.");
                    return;
                }
                MainWindow main = (MainWindow)o;
                CommandLibrary lib = main.CurrentLibrary;
                buffer.Help(lib.Library[c.Split()[1]].Help);
            }
        }

        public class Exit : ICommand
        {
            public string Help => "Exit Command Console";
            public string Keyword => "exit";
            public CommandType Type => CommandType.Command;
            public void Execute(object sender, string c)
            {
                if(sender.GetType() == typeof(MainWindow))
                    Application.Current.Shutdown();

                if (((ICommand)sender).Type == CommandType.App)
                    ((IApp)sender).Cancel();

            }
        }
    }
    /// <summary>
    /// Command template for commands that run in the primary thread.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Help information
        /// </summary>
        string Help { get; }
        /// <summary>
        /// Keyword to execute
        /// </summary>
        string Keyword { get; }
        /// <summary>
        /// Method to execute
        /// </summary>
        /// <param name="Param">Parameters</param>
        /// <param name="input">complete input string</param>
        void Execute(object Param, string input);
        /// <summary>
        /// Type of command.
        /// </summary>
        CommandType Type { get; }
    }
    /// <summary>
    /// Application template for apps that run in their own thread.
    /// </summary>
    public interface IApp : ICommand
    {
        /// <summary>
        /// Sub commands for the application to function
        /// </summary>
        CommandLibrary SubCommands { get; }
        /// <summary>
        /// Command to cancel task.
        /// </summary>
        void Cancel();
        /// <summary>
        /// Log location.
        /// </summary>
        void Log();
        /// <summary>
        /// Current status.
        /// </summary>
        string Status {get;}
    }

    public enum CommandType { Command, App };
}
