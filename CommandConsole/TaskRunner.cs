using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CommandConsole
{
    public class CommandRunner
    {
        /// <summary>
        /// Task ID number
        /// </summary>
        private int _id = 0;
        /// <summary>
        /// Classes that correspond to running tasks.
        /// </summary>
        public Dictionary<string, Task> RunningTasks = new Dictionary<string, Task>();
        /// <summary>
        /// Top Level Library
        /// </summary>
        private CommandLibrary TopLevel => TopLevelLibrary.Instance;
        /// <summary>
        /// Current Library.
        /// </summary>
        public CommandLibrary CurrentLibrary { get; set; }
        /// <summary>
        /// Console output.
        /// </summary>
        private ConsoleBuffer Buffer = new ConsoleBuffer();

        public CommandRunner()
        {
            _id++;
            CurrentLibrary = TopLevel;
            ICommand c = new TestTask();
            TopLevelLibrary.Instance.Library.Add(c.Keyword, c);
        }
        /// <summary>
        /// Call to run a command.
        /// </summary>
        /// <param name="s">input string.</param>
        public void RunCommand(string s)
        {
            if (s.Length <= 0 || !CurrentLibrary.Library.ContainsKey(s.Split()[0]))
            {
                Buffer.Error("Command not found");
                return;
            }

            ICommand refcom = CurrentLibrary.Library[s.Split()[0]];

            switch (refcom.Type)
            {
                case CommandType.App:
                    NewTask((IApp)refcom, s);
                    break;
                case CommandType.Command:
                    refcom.Execute(this, s);
                    break;
            }
        }
        /// <summary>
        /// Run a new System.Threading.Task of IApp.Execute() passed.
        /// </summary>
        /// <param name="App">App to run</param>
        /// <param name="c"></param>
        private void NewTask(IApp App, string c)
        {
            RunningTasks.Add(_id.ToString("X"), Task.Run(() => App.Execute(this,c)));
            Task.Run(() => App.Execute(this, c));
            _id++;
        }
    }
    /*
    public class Abort : ICommand
    {
        public string Help => "Abort a task. (ex: abort <TaskID>, abort 200";
        public string Keyword => "abort";
        public CommandType Type => CommandType.Command;
        public void Execute(object sender, string c)
        {
            CommandRunner cr = (CommandRunner)sender;
            string[] cs = c.Split();
            if(cs.Length < 2 && cr.RunningTasks.ContainsKey(cs[1]))
                cr.RunningTasks[cs[2]].Abort
        }
    }
    */
    public class TestTask : IApp
    { 
        public string Help => "This is a test task. It will run in a seperate thread.";
        public CommandLibrary SubCommands { get; set; }
        public string Keyword => "test";
        public void Cancel() { }
        public void Log() { }
        public string Status => "Status test";
        public CommandType Type => CommandType.App;
        public void Execute(object sender, string s)
        {
            while (true)
            {
                ConsoleBuffer buffer = new ConsoleBuffer();
                buffer.Write("This is a test!");
                Thread.Sleep(1000);
            }
        }
    }
}
