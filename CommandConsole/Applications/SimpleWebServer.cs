using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommandConsole
{
    class SimpleWebServer : CommandEngine
    {
        public override string Name => "Simple Web Server";
        public override string TaskID { get; set; }
        public override string Keyword => "simplewebserver";
        public override string Help => "A simple web server. use listcommands for Application commands.";
        public SimpleWebServer() : base()
        {
            SubCommands.Library.Add(new Leave().Keyword, new Leave());
        }

        public override void Execute(object sender, string c)
        {

        }

        public class Leave : ICommand
        {
            public string Keyword => "leave";
            public string Help => "Leaves the current interfaced application back to the CommandEngine application.";
            public CommandType Type => CommandType.Command;
            public void Execute(object sender, string c)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TaskSystem.ActiveTask.Pop();
                    TaskSystem.WorkingDirectory = ">> ";
                    ConsoleBuffer.Write("Exiting " + ((IApp)sender).Name + " command interface.");
                });
            }
        }

    }
}
