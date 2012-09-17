using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Server;

namespace PartyCraft
{
    public abstract class Command
    {
        internal static List<Command> Commands;

        internal static void LoadCommands(Server server)
        {
            Commands = new List<Command>();
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()
                .Where(t => !t.IsAbstract && typeof(Command).IsAssignableFrom(t)));
            foreach (var type in types)
            {
                var command = (Command)Activator.CreateInstance(type);
                command.Aliases = new List<string>();
                if (server.SettingsProvider.ContainsKey("command." + command.DefaultCommand + ".aliases"))
                {
                    command.Aliases = server.SettingsProvider.Get<List<string>>(
                        "command." + command.DefaultCommand + ".aliases");
                }
            }
        }

        public static void ExecuteCommand(Server server, MinecraftClient user, string command)
        {
            
        }

        public abstract string DefaultCommand { get; }
        public abstract string Documentation { get; set; }
        public abstract void Execute(params string[] parameters);

        public List<string> Aliases { get; set; }
        public List<string> AllowedGroups { get; set; }
    }
}
