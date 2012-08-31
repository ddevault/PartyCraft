using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Craft.Net.Server;

namespace PartyCraft
{
    public class Program
    {
        public static event EventHandler PreStartup;
        /// <summary>
        /// The server's settings provider. If this is null after
        /// the PreStartup event, it will be set automatically.
        /// </summary>
        public static ISettingsProvider SettingsProvider = null;

        public static void Main(string[] args)
        {
            if (PreStartup != null)
                PreStartup(null, null);
            if (SettingsProvider == null)
                SettingsProvider = new VanillaSettingsProvider();

            Server server = new Server(SettingsProvider);
            // TODO: Better logging
            ConsoleLogWriter consoleLog = new ConsoleLogWriter(LogImportance.High);
            server.MinecraftServer.AddLogProvider(consoleLog);

            server.Start();

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q')
                    break;
            }

            server.Stop();
        }
    }
}
