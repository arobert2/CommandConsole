using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace CommandConsole.ComApps
{
    class SimpleWebServer : AppTemplate
    {
        public override string Name => "Simple Web Server";
        public override string Keyword => "simplewebserver";
        public override string Help => "A simple web server. use listcommands for Application commands.";
        private string stat;
        public override string Status { get { return stat; } set { stat = value;  Log(stat); } }

        protected List<string> Routing { get; set; } = new List<string>();
        protected int ListeningPort { get; set; }

        public SimpleWebServer()
        {
            SubCommands.Library.Add("exit", new Leave());
        }

        public override void Execute(object sender, string c)
        {
            while(!StopApp)
            {
                string newinstruction = CheckForInstruction();
                string[] queue = newinstruction?.Split();
                if (queue != null)
                {
                    if (SubCommands.Library.ContainsKey(queue[0]) && SubCommands.Library[queue[0]].Type == CommandType.Command)
                        SubCommands.Library[queue[0]].Execute(this, newinstruction);         
                    else
                        Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error("Command " + queue[0] + " not found!"));
                }
            }
        }

        public override IApp Clone()
        {
            return new SimpleWebServer();
        }

        public override void Log(string output)
        {
            string d = "SimpleWebServer";
            string f = "App.log";

            if (!Directory.Exists(d))
                Directory.CreateDirectory("SimpleWebServer");

            if (!File.Exists(d + @"\" + f))
                File.Create(f);

            using (StreamWriter sr = new StreamWriter(d + @"\" + f))
            {
                sr.WriteLine(Status);
            }
        }

        public class Leave : ICommand
        {
            public string Keyword => "exit";
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

        public class setport : ICommand
        {
            public string Keyword => "setport";
            public string Help => "Set the port number to listen on.";
            public CommandType Type => CommandType.Command;
            public void Execute(object sender, string c)
            {
                string[] comms = c.Split();
                SimpleWebServer parent = (SimpleWebServer)sender;
                if(comms.Length < 1)
                {
                    parent.Status = "setport: No port number supplied.";
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error(parent.Status));
                    return;
                }

                int parse = -1;
                int.TryParse(comms[1], out parse);
                parent.ListeningPort = parse;

                if(parse < 0)
                {
                    parent.Status = "setport: No port number supplied.";
                    Application.Current.Dispatcher.Invoke(() => ConsoleBuffer.Error(parent.Status));
                    return;
                }
            }
        }
    }
}
