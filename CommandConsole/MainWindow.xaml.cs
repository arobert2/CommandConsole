using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using CommandConsole.ComApps;

namespace CommandConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Output buffer information
        /// </summary>
        //ConsoleBuffer Buffer = new ConsoleBuffer();
        /// <summary>
        /// Token displayed when requesting input.
        /// </summary>
        public static string InputToken => TaskSystem.WorkingDirectory;
        /// <summary>
        /// Minimum caret position
        /// </summary>
        int MinimumCaretPosition => TaskSystem.WorkingDirectory.Length;
        /// <summary>
        /// Current caret position
        /// </summary>
        int CaretPosition => ConsoleInput.CaretIndex;
        /// <summary>
        /// Current command library.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Cursor newCursor = new Cursor(new MemoryStream(Properties.Resources.CustomIBeam));
            ConsoleInput.Cursor = newCursor;
            ConsoleOutput.Cursor = newCursor;
            LoadApps la = new LoadApps();
            ConsoleBuffer.BufferUpdated = WriteBuffer;
            ConsoleBuffer.Clear = () => { ConsoleOutput.Document.Blocks.Clear(); ConsoleOutput.AppendText(Environment.NewLine); };
            CommandEngine commandengine = new CommandEngine();
            foreach (ICommand ic in LoadApps.AppsToLoad.Library.Values)
                commandengine.SubCommands.Library.Add(ic.Keyword, ic);

            TaskSystem.ActiveTask.Push(commandengine.TaskID);
            TaskSystem.RunningTasks.Add(commandengine.TaskID, Task.Run(() => commandengine.Execute(this, null)));
            TaskSystem.CommandQueue.Add(commandengine.TaskID, new Queue<string>());
            TaskSystem.EnterSubCommand = () => ResetInput();

            //setup ConsoleInput textbox.
            ConsoleInput.Text = InputToken;
            ConsoleInput.CaretBrush = new SolidColorBrush(Colors.Black);
            ConsoleInput.CaretIndex = MinimumCaretPosition;

            //focus ConsoleInput and stay there
            ConsoleInput.Focus();
            ConsoleOutput.IsReadOnly = true;

            //ConsoleOutput scroll down on output
            ConsoleOutput.TextChanged += (object sender, TextChangedEventArgs e) => ConsoleOutput.ScrollToEnd();

            //Launch text
            ConsoleBuffer.WriteColor("COMMAND CONSOLE\n\nCreator: Allen Roberts\nemail: allen.roberts1985@gmail.com\ngit: github.com/arobert2\n", new SolidColorBrush(Colors.Blue));
            ConsoleBuffer.Help("type listcommands to get started.");
        }
        /// <summary>
        /// Writes the test from the buffer.
        /// </summary>
        public void WriteBuffer()
        {
            TextRange tr = new TextRange(ConsoleOutput.Document.ContentEnd, ConsoleOutput.Document.ContentEnd);
            tr.Text = ConsoleBuffer.OutputBuffer[ConsoleBuffer.OutputBuffer.Count - 1].Text + "\n";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, ConsoleBuffer.OutputBuffer[ConsoleBuffer.OutputBuffer.Count - 1].DisplayColor);
        }
        /// <summary>
        /// Execute when enter is pressed.
        /// </summary>
        public void OnEnter()
        {
            string CommandString = ConsoleInput.Text.Substring(MinimumCaretPosition);
            TaskSystem.EnterCommand(CommandString);
            ResetInput();
        }

        public void ResetInput()
        {
            ConsoleInput.Text = InputToken;
            ConsoleInput.CaretIndex = MinimumCaretPosition;
        }
        /// <summary>
        /// Check for key press
        /// </summary>
        /// <param name="sender">Key press sender</param>
        /// <param name="e">Key press event args</param>
        public void event_KeyPressed(object sender, KeyEventArgs e)
        {
            if (CaretPosition <= MinimumCaretPosition)
                if(e.Key == Key.Left || e.Key == Key.Back)
                    e.Handled = true;

            if (e.Key == Key.Enter)
                OnEnter();
        }
        /// <summary>
        /// Left click release.
        /// </summary>
        /// <param name="sender">click object sender</param>
        /// <param name="e">Click even args.</param>
        private void ConsoleInput_Click(object sender, MouseButtonEventArgs e)
        {
            if (CaretPosition <= MinimumCaretPosition)
                ConsoleInput.CaretIndex = MinimumCaretPosition;
        }
    }
}
