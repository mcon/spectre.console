using System;
using Spectre.Console.Rendering;

namespace Spectre.Console
{
    /// <summary>
    /// Contains extension methods for <see cref="IAnsiConsole"/>.
    /// </summary>
    public static partial class AnsiConsoleExtensions
    {
        /// <summary>
        /// Creates a recorder for the specified console.
        /// </summary>
        /// <param name="console">The console to record.</param>
        /// <returns>A recorder for the specified console.</returns>
        public static Recorder CreateRecorder(this IAnsiConsole console)
        {
            return new Recorder(console);
        }

        /// <summary>
        /// Writes the specified string value to the console.
        /// </summary>
        /// <param name="console">The console to write to.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="style">The text style.</param>
        public static void Write(this IAnsiConsole console, string text, Style style)
        {
            if (console is null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            console.Write(new Segment(text, style));
        }

        /// <summary>
        /// Writes an empty line to the console.
        /// </summary>
        /// <param name="console">The console to write to.</param>
        public static void WriteLine(this IAnsiConsole console)
        {
            if (console is null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            console.Write(Environment.NewLine, Style.Plain);
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the console.
        /// </summary>
        /// <param name="console">The console to write to.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="style">The text style.</param>
        public static void WriteLine(this IAnsiConsole console, string text, Style style)
        {
            if (console is null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            console.Write(new Segment(text, style));
            console.WriteLine();
        }
    }
}