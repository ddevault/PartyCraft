using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Server;
using Craft.Net.Data;

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
                command.AllowedGroups = new List<string>();
                if (server.SettingsProvider.ContainsKey("command." + command.DefaultCommand + ".groups"))
                {
                    command.AllowedGroups = server.SettingsProvider.Get<List<string>>(
                        "command." + command.DefaultCommand + ".groups");
                }
                else
                    command.LoadDefaultPermissions();
                Commands.Add(command);
            }
        }

        public static void ExecuteCommand(Server server, MinecraftClient user, string text)
        {
            string name = text.Substring(1);
            if (string.IsNullOrEmpty(name))
                return;
            var parameters = new string[0];
            var userText = "";
            if (name.Contains(" "))
            {
                name = name.Remove(name.IndexOf(' '));
                userText = text.Substring(text.IndexOf(' ') + 1);
                parameters = userText.Split(' ');
            }
            var command = GetCommand(name);

            if (command == null)
            {
                user.SendChat(ChatColors.Red + "That command was not found.");
                return;
            }

            if (!MayUseCommand(command, user))
            {
                user.SendChat(ChatColors.Red + "You do not have permission to use that command.");
                return;
            }
            command.Execute(server, user, userText, parameters);
        }

        public static Command GetCommand(string name)
        {
            name = name.ToLower();
            foreach (var command in Commands)
            {
                if (command.DefaultCommand == name || command.Aliases.Contains(name))
                    return command;
            }
            return null;
        }

        public static bool MayUseCommand(Command command, MinecraftClient user)
        {
            var groups = (List<string>)user.Tags["PartyCraft.UserGroups"];
            foreach (var group in groups)
            {
                if (command.AllowedGroups.Contains(group))
                    return true;
            }
            return false;
        }

        protected internal virtual void LoadDefaultPermissions()
        {
            AllowedGroups.Add("server.default");
        }

        public abstract string DefaultCommand { get; }
        public abstract string Documentation { get; }
        // TODO: CommandContext class
        public abstract void Execute(Server server, MinecraftClient user, string text, params string[] parameters);

        public List<string> Aliases { get; set; }
        public List<string> AllowedGroups { get; set; }
    }
}
