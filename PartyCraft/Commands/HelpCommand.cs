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
        public override string DefaultCommand
        {
            get { return "help"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Gives documentation for the specified command.\n" +
                    "Usage: /help " + ChatColors.Italic + "command\n" +
                    "Example: /help tell";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            if (parameters.Length < 1)
            {
                user.SendChat(ChatColors.Red + "Insufficient parameters. Use \"/help\" for more information.");
                return;
            }
            if (parameters.Length > 1)
            {
                user.SendChat(ChatColors.Red + "Too many parameters. Use \"/help\" for more information.");
                return;
            }
            var command = GetCommand(parameters[0]);
            if (command == null)
            {
                user.SendChat(ChatColors.Red + "Command not found.");
                return;
            }
            string[] lines = command.Documentation.Split('\n');
            user.SendChat(ChatColors.DarkGreen + "Documentation for " + ChatColors.Bold + "/" + command.DefaultCommand + ":");
            foreach (var line in lines)
                user.SendChat(ChatColors.DarkCyan + line);
            if (!MayUseCommand(command, user, server))
                user.SendChat(ChatColors.Red + "You are not permitted to use this command.");
        }
    }
}
