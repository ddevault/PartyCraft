using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class TellCommand : Command
    {
        public override string DefaultCommand
        {
            get { return "tell"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Sends a private message to the specified player.\n" +
                    "Usage: /tell " + ChatColors.Italic + "player message\n" +
                    "Example: /tell notch Hello!";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            if (parameters.Length < 2)
            {
                user.SendChat(ChatColors.Red + "Insufficient parameters. Use \"/help tell\" for more information.");
                return;
            }
            var client = server.MinecraftServer.GetClient(parameters[0]);
            if (client == null)
            {
                user.SendChat(parameters[0] + " is not online.");
                return;
            }
            var format = server.SettingsProvider.Get<string>("chat.private.format");
            client.SendChat(string.Format(format, user.Username, client.Username, text));
        }
    }
}
