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
    public partial class LoadApps
    {
        /// <summary>
        /// Top level library
        /// </summary>
        public static CommandLibrary AppsToLoad{ get; set; }
        /// <summary>
        /// Initialize library.
        /// </summary>
        public LoadApps()
        {
            AppsToLoad = AppsToLoad == null ? new CommandLibrary() : AppsToLoad;
            LoadLibrary();
        }
        partial void LoadLibrary();
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
        /// <param name="sender">Parameters</param>
        /// <param name="input">complete input string</param>
        void Execute(object sender, string input);
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
        /// Name of task.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Task ID number
        /// </summary>
        string TaskID { get; set; }
        /// <summary>
        /// Sub commands for the application to function
        /// </summary>
        CommandLibrary SubCommands { get; }
        /// <summary>
        /// Log location.
        /// </summary>
        void Log(string stat);
        /// <summary>
        /// Current status.
        /// </summary>
        string Status { get; }
        /// <summary>
        /// Stops the application. Must be implemented in Execute.
        /// </summary>
        bool StopApp { get; set; }
    }
    /// <summary>
    /// defines whether a command runs in the current thread or a seperate thread.
    /// </summary>
    public enum CommandType { Command, App };
}
