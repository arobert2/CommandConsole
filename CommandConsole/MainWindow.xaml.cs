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
        ConsoleBuffer buff = new ConsoleBuffer();
        string InputToken => ">> ";

        public MainWindow()
        {
            InitializeComponent();
            buff.BufferUpdated = WriteBuffer;

            //setup ConsoleInput textbox.
            ConsoleInput.Text = InputToken;
            ConsoleInput.CaretBrush = new SolidColorBrush(Colors.Black);

            //focus ConsoleInput and stay there
            ConsoleInput.Focus();
            ConsoleInput.LostKeyboardFocus += (object o, KeyboardFocusChangedEventArgs e) => ConsoleInput.Focus();

            //Launch text
            buff.Write("COMMAND CONSOLE\n\nCreator: Allen Roberts\nemail: allen.roberts1985@gmail.com\ngit: github.com/arobert2");
        }

        public void WriteBuffer()
        {
            TextRange tr = new TextRange(ConsoleOutput.Document.ContentEnd, ConsoleOutput.Document.ContentEnd);
            tr.Text = buff.OutputBuffer[buff.OutputBuffer.Count - 1].Text + "\n";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, buff.OutputBuffer[buff.OutputBuffer.Count - 1].DisplayColor);
        }

        public void CommandInputed()
        {
            ConsoleInput.Text = InputToken;
        }

        public void OnEnter()
        {
            string CommandString = ConsoleInput.Text.Substring(3).ToLower();
        }

        public class CBuffer
        {
            public static ConsoleBuffer Instance { get; set; }

            public CBuffer()
            {
                Instance = Instance == null ? new ConsoleBuffer() : Instance;
            }
        }
    }
}
