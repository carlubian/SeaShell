﻿using Pastel;
using System;

namespace SeaShell.Core
{
    public static class ConsoleIO
    {
        internal static void Initialize()
        {
            Console.Write(" ".Pastel("#F0F5FF").PastelBg("#283038"));
            Console.Clear();
        }

        internal static void Restore()
        {
            Console.Write(" ".Pastel("#DCE1EB").PastelBg("#000000"));
        }

        internal static void ShowPrompt()
        {
            var env = "SSS";
            if (SeaShellHost.Env != "_system")
                env = $"[{SeaShellHost.Env}]";

            Console.Write($"{env} {Environment.CurrentDirectory}>".Pastel("#F0F5FF"));
            Console.Write(" ".Pastel("#DCE1EB"));
        }

        public static void WriteLine(string message)
        {
            Console.WriteLine(message.Pastel("#DCE1EB"));
        }

        public static void WriteInfo(string message)
        {
            Console.WriteLine(message.Pastel("#336CC2"));
        }

        public static void WriteError(string message)
        {
            Console.WriteLine(message.Pastel("#EA4015"));
        }

        public static void WriteWarning(string message)
        {
            Console.WriteLine(message.Pastel("#ECB310"));
        }
    }
}