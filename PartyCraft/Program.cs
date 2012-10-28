using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Craft.Net.Server;
using System.IO;

namespace PartyCraft
{
    public class Program
    {
        /// <summary>
        /// Called immediately after plugins are loaded, and before anything else happens.
        /// This is your only opportunity as a plugin to set Program.SettingsProvider.
        /// </summary>
        public static event EventHandler PreStartup;
        /// <summary>
        /// The server's settings provider. If this is null after
        /// the PreStartup event, it will be set automatically.
        /// </summary>
        public static ISettingsProvider SettingsProvider = null;

        public static void Main(string[] args)
        {
            CheckEnviornment();
            // TODO: Load plugins
            if (PreStartup != null)
                PreStartup(null, null);
            if (SettingsProvider == null)
            {
                // Select a settings provider based on the enviornment
                if (File.Exists("server.properties"))
                {
                    SettingsProvider = new VanillaSettingsProvider();
                    (SettingsProvider as VanillaSettingsProvider).Load(
                        File.Open("server.properties", FileMode.Open));
                }
                else
                {
                    // TODO: Create a better settings provider than vanilla
                    SettingsProvider = new VanillaSettingsProvider();
                    SetUpDefaultPermissions(SettingsProvider);
                }
            }

            var server = new Server(SettingsProvider);
            Command.LoadCommands(server);
            // TODO: Better logging
            var consoleLog = new ConsoleLogWriter(LogImportance.Medium);
            LogProvider.RegisterProvider(consoleLog);

            server.Start();

            Console.WriteLine("Press 'q' to quit.");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q')
                    break;
            }

            server.Stop();
        }

        private static void SetUpDefaultPermissions(ISettingsProvider SettingsProvider)
        {
            // TODO: Is this the best way to go about this?
        }

        private static void CheckEnviornment()
        {
            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");
        }
    }
}
