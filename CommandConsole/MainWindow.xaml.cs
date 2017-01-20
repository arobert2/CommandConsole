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
        ConsoleBuffer Buffer = new ConsoleBuffer();
        /// <summary>
        /// Token displayed when requesting input.
        /// </summary>
        public static string InputToken => ">> ";
        /// <summary>
        /// Minimum caret position
        /// </summary>
        int MinimumCaretPosition => InputToken.Length;
        /// <summary>
        /// Current caret position
        /// </summary>
        int CaretPosition => ConsoleInput.CaretIndex;
        /// <summary>
        /// Top level CommandLibrary.
        /// </summary>
        private static TopLevelLibrary _lib = new TopLevelLibrary();
        /// <summary>
        /// Current command library.
        /// </summary>
        public CommandLibrary CurrentLibrary { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            Buffer.BufferUpdated = WriteBuffer;
            CurrentLibrary = TopLevelLibrary.Instance;

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
            Buffer.Write("COMMAND CONSOLE\n\nCreator: Allen Roberts\nemail: allen.roberts1985@gmail.com\ngit: github.com/arobert2\n");
        }
        /// <summary>
        /// Writes the test from the buffer.
        /// </summary>
        public void WriteBuffer()
        {
            TextRange tr = new TextRange(ConsoleOutput.Document.ContentEnd, ConsoleOutput.Document.ContentEnd);
            tr.Text = Buffer.OutputBuffer[Buffer.OutputBuffer.Count - 1].Text + "\n";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Buffer.OutputBuffer[Buffer.OutputBuffer.Count - 1].DisplayColor);
        }
        /// <summary>
        /// Execute when enter is pressed.
        /// </summary>
        public void OnEnter()
        {
            string CommandString = ConsoleInput.Text.Substring(MinimumCaretPosition).ToLower();
            if (CommandString.Length <= 0 || !CurrentLibrary.Library.ContainsKey(CommandString.Split()[0]))
            {
                Buffer.Error("Command not found.");
            }
            else
            {
                CurrentLibrary.Library[CommandString.Split()[0]].Execute(this, CommandString);
            }
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
