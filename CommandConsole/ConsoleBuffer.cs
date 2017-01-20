using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CommandConsole
{
    public class ConsoleBuffer
    {
        /// <summary>
        /// Update console delegate
        /// </summary>
        public static Action _bufferupdated;

        public Action BufferUpdated { get { return _bufferupdated; } set { _bufferupdated = value; } }
        /// <summary>
        /// Output buffer
        /// </summary>
        private static List<StringData> _obuffer = new List<StringData>();
        /// <summary>
        /// Public output buffer. Used to invoke BufferUpdated when updated.
        /// </summary>
        public List<StringData> OutputBuffer { get { return _obuffer; } private set { _obuffer = value; } }

        public ConsoleBuffer()
        {
        }

        /// <summary>
        /// Write to OutputBuffer as an error
        /// </summary>
        /// <param name="t">text to write</param>
        public void Error(string t)
        {
            WriteColor("!!! " + t, new SolidColorBrush(Colors.DarkRed));
        }
        /// <summary>
        /// Write to OutputBuffer as Help text
        /// </summary>
        /// <param name="t">Text to write</param>
        public void Help(string t)
        {
            WriteColor("??? " + t, new SolidColorBrush(Colors.Green));
        }
        /// <summary>
        /// Write to OutputBuffer as standard info.
        /// </summary>
        /// <param name="t">Text to write.</param>
        public void Write(string t)
        {
            WriteColor(t, new SolidColorBrush(Colors.Black));
        }
        /// <summary>
        /// Write to console as a specific color
        /// </summary>
        /// <param name="t">Text</param>
        /// <param name="b">Color</param>
        public void WriteColor(string t, Brush b)
        {
            OutputBuffer.Add(new StringData(b, t));
            BufferUpdated?.Invoke();
        }
        /// <summary>
        /// Holds string data
        /// </summary>
        public struct StringData
        {
            /// <summary>
            /// Color of the string
            /// </summary>
            public Brush DisplayColor { get; set; }
            /// <summary>
            /// Text to be displayed
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// Generate a new set of StringData
            /// </summary>
            /// <param name="cc">string color</param>
            /// <param name="t">text info</param>
            public StringData(Brush cc, string t) { Text = t;  DisplayColor = cc; }
        }
    }
}
