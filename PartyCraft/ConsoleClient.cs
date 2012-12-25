using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Craft.Net.Server;

namespace PartyCraft
{
    public class ConsoleClient : MinecraftClient
    {
        public ConsoleClient(MinecraftServer server) : base()
        {
            Username = "[CONSOLE]";
            Tags["PartyCraft.UserGroups"] = new List<string>(new[] { "server.default", "server.op", "server.console" });
        }

        public override void SendChat(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// Writes text to the console after changing chat colors to
        /// console colors.
        /// </summary>
        public static void WriteLine(string text)
        {
            var parts = text.Split('§');
            Console.ResetColor();
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;
                if (ColorMappings.ContainsKey(part[0]))
                    Console.ForegroundColor = ColorMappings[part[0]];
                Console.Write(part.Substring(1));
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        private static Dictionary<char, ConsoleColor> ColorMappings = new Dictionary<char, ConsoleColor>();
        static ConsoleClient()
        {
            ColorMappings = new Dictionary<char, ConsoleColor>();
            ColorMappings.Add('0', ConsoleColor.Black);
            ColorMappings.Add('1', ConsoleColor.DarkBlue);
            ColorMappings.Add('2', ConsoleColor.DarkGreen);
            ColorMappings.Add('3', ConsoleColor.DarkCyan);
            ColorMappings.Add('4', ConsoleColor.DarkRed);
            ColorMappings.Add('5', ConsoleColor.DarkMagenta);
            ColorMappings.Add('6', ConsoleColor.DarkYellow);
            ColorMappings.Add('7', ConsoleColor.Gray);
            ColorMappings.Add('8', ConsoleColor.DarkGray);
            ColorMappings.Add('9', ConsoleColor.Blue);
            ColorMappings.Add('a', ConsoleColor.Green);
            ColorMappings.Add('b', ConsoleColor.Cyan);
            ColorMappings.Add('c', ConsoleColor.Red);
            ColorMappings.Add('d', ConsoleColor.Magenta);
            ColorMappings.Add('e', ConsoleColor.Yellow);
            ColorMappings.Add('f', ConsoleColor.White);
        }
    }
}
