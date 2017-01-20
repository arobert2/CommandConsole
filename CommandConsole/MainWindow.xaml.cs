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
        ConsoleBuffer Buffer = new ConsoleBuffer();
        string InputToken => ">> ";

        int MinimumCaretPosition => InputToken.Length;
        int CaretPosition => ConsoleInput.CaretIndex;

        public Action TaskSystem;

        public MainWindow()
        {
            InitializeComponent();
            Buffer.BufferUpdated = WriteBuffer;

            //setup ConsoleInput textbox.
            ConsoleInput.Text = InputToken;
            ConsoleInput.CaretBrush = new SolidColorBrush(Colors.Black);
            ConsoleInput.CaretIndex = MinimumCaretPosition;

            //focus ConsoleInput and stay there
            ConsoleInput.Focus();
            ConsoleInput.LostKeyboardFocus += (object o, KeyboardFocusChangedEventArgs e) => ConsoleInput.Focus();

            //Launch text
            Buffer.Write("COMMAND CONSOLE\n\nCreator: Allen Roberts\nemail: allen.roberts1985@gmail.com\ngit: github.com/arobert2");
        }

        public void WriteBuffer()
        {
            TextRange tr = new TextRange(ConsoleOutput.Document.ContentEnd, ConsoleOutput.Document.ContentEnd);
            tr.Text = Buffer.OutputBuffer[Buffer.OutputBuffer.Count - 1].Text + "\n";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, Buffer.OutputBuffer[Buffer.OutputBuffer.Count - 1].DisplayColor);
        }

        public void CommandInputed()
        {
            ConsoleInput.Text = InputToken;
        }

        public void OnEnter()
        {
            string CommandString = ConsoleInput.Text.Substring(MinimumCaretPosition).ToLower();
        }

        public void event_KeyPressed(object sender, KeyEventArgs e)
        {
            if (CaretPosition <= MinimumCaretPosition)
                if(e.Key == Key.Left || e.Key == Key.Back)
                    e.Handled = true;

            if (e.Key == Key.Enter)
                OnEnter();
        }

        private void ConsoleInput_Click(object sender, MouseButtonEventArgs e)
        {
            if (CaretPosition < MinimumCaretPosition)
                ConsoleInput.CaretIndex = MinimumCaretPosition;
        }
    }
}
