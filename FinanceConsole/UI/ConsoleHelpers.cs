using System;
using FinanceConsole.Exceptions;

namespace FinanceConsole.UI
{
    /// <summary>Static helpers for reading and validating console input.</summary>
    public static class ConsoleHelpers
    {
        public static string ReadString(string prompt, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine() ?? string.Empty;
                if (allowEmpty || !string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("  (value required)");
            }
        }

        public static decimal ReadDecimal(string prompt, decimal? min = null, decimal? max = null)
        {
            while (true)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine();
                if (decimal.TryParse(raw, out var v))
                {
                    if (min.HasValue && v < min.Value) { Console.WriteLine($"  Must be >= {min}"); continue; }
                    if (max.HasValue && v > max.Value) { Console.WriteLine($"  Must be <= {max}"); continue; }
                    return v;
                }
                Console.WriteLine("  Invalid number, try again.");
            }
        }

        public static int ReadInt(string prompt, int? min = null, int? max = null)
        {
            while (true)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine();
                if (int.TryParse(raw, out var v))
                {
                    if (min.HasValue && v < min.Value) { Console.WriteLine($"  Must be >= {min}"); continue; }
                    if (max.HasValue && v > max.Value) { Console.WriteLine($"  Must be <= {max}"); continue; }
                    return v;
                }
                Console.WriteLine("  Invalid integer.");
            }
        }

        public static void Pause()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        /// <summary>Wraps an action in try/catch/finally — demonstrates EXCEPTION HANDLING.</summary>
        public static void Safe(Action action)
        {
            try { action(); }
            catch (FinanceException ex) { Console.WriteLine($"  [Error] {ex.Message}"); }
            catch (FormatException ex) { Console.WriteLine($"  [Format] {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"  [Unexpected] {ex.Message}"); }
            finally { /* place for cleanup hooks (logging, etc.) */ }
        }
    }
}