using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class HelpCommand : Command
    {
        public const int CommandsPerPage = 5;

        public override string DefaultCommand
        {
            get { return "help"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Gives documentation for the specified command.\n" +
                    "Usage: /help " + ChatColors.Italic + "command|page\n" +
                    "Example: /help tell or /help 1\n" +
                    "If a number is given, a list of all commands is shown, starting\n" +
                    "at the specified page.";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            int page = -1;
            if (parameters.Length == 0)
                page = 1;
            if (parameters.Length > 1)
            {
                user.SendChat(ChatColors.Red + "Incorrect usage. Use \"/help\" for more information.");
                return;
            }
            if (page != -1 || int.TryParse(parameters[0], out page))
            {
                page--;
                int pages = Commands.Count / CommandsPerPage;
                if (page >= pages)
                {
                    user.SendChat(ChatColors.Red + "No further commands. Use \"/help\" for more information.");
                    return;
                }
                user.SendChat(ChatColors.DarkGreen + "Command List (page " + (page + 1) + " of " + pages + ")");
                foreach (var c in Commands.Skip(page * CommandsPerPage).Take(CommandsPerPage))
                    user.SendChat(ChatColors.DarkCyan + "/" + c.DefaultCommand);
                return;
            }
            var command = GetCommand(parameters[0]);
            if (command == null)
            {
                user.SendChat(ChatColors.Red + "Command not found.");
                return;
            }
            string[] lines = command.Documentation.Split('\n');
            user.SendChat(ChatColors.DarkGreen + "Documentation for /" + command.DefaultCommand + ":");
            foreach (var line in lines)
                user.SendChat(ChatColors.DarkCyan + line);
            if (!MayUseCommand(command, user, server))
                user.SendChat(ChatColors.Red + "You are not permitted to use this command.");
        }
    }
}
