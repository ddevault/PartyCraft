using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Craft.Net.Server;
using System.IO;
using PartyCraft.API;
using Mono.CSharp;

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

        public static ConsoleClient ConsoleClient { get; set; }

        public static void Main(string[] args)
        {
            CheckEnviornment();
            // TODO: Load certain plugins earlier
            if (PreStartup != null)
                PreStartup(null, null);
            if (SettingsProvider == null)
            {
                // Select a settings provider based on the enviornment
                if (File.Exists("server.properties"))
                    SettingsProvider = new VanillaSettingsProvider("server.properties");
                else
                {
                    // TODO: Create a better settings provider than vanilla
                    SettingsProvider = new VanillaSettingsProvider("server.properties");
                    SetUpDefaultPermissions(SettingsProvider);
                }
            }

            var server = new Server(SettingsProvider);
            LoadPlugins(server);
            Command.LoadCommands(server);
            // TODO: Better logging
            var consoleLog = new ConsoleLogWriter(LogImportance.Medium);
            LogProvider.RegisterProvider(consoleLog);

            server.Start();

            Console.WriteLine("Use /stop to kill the server.");
            ConsoleClient = new ConsoleClient(server.MinecraftServer);
            while (true)
            {
                var command = Console.ReadLine();
                try
                {
                    Command.ExecuteCommand(server, ConsoleClient, command);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            server.Stop();
        }

        private static void LoadPlugins(Server server)
        {
            // TODO: Make this better
            // Dynamic plugins
            var plugins = Directory.GetFiles(Path.Combine("plugins", "scripts"), "*.csx");
            var eval = new Evaluator(new CompilerContext(new CompilerSettings(), new ConsoleReportPrinter()));
            eval.ReferenceAssembly(typeof(Plugin).Assembly);
            eval.ReferenceAssembly(typeof(MinecraftServer).Assembly);
            eval.InteractiveBaseClass = typeof(ScriptPluginBase);
            ScriptPluginBase.Server = server;
            foreach (var plugin in plugins)
                eval.Run(File.ReadAllText(plugin));

            var types = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetTypes())
                    .Aggregate((a, b) => a.Concat(b).ToArray()).Where(t => !t.IsAbstract && typeof(Plugin).IsAssignableFrom(t));
            foreach (var type in types)
            {
                var plugin = (Plugin)Activator.CreateInstance(type);
                plugin.OnInitialize(server);
            }
        }

        private static void SetUpDefaultPermissions(ISettingsProvider SettingsProvider)
        {
            // TODO: Is this the best way to go about this?

        }

        private static void CheckEnviornment()
        {
            if (!Directory.Exists("plugins"))
                Directory.CreateDirectory("plugins");
            if (!Directory.Exists(Path.Combine("plugins", "scripts")))
                Directory.CreateDirectory(Path.Combine("plugins", "scripts"));
        }
    }
}
