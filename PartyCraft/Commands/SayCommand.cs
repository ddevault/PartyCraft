using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class SayCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "say"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Broadcasts a message to the entire server.\n" +
                    "Usage: /say " + ChatColors.Italic + "message";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            if (parameters.Length == 0)
            {
                user.SendChat(ChatColors.Red + "Incorrect usage. Use /help say for more information.");
                return;
            }
            var format = server.SettingsProvider.Get<string>("chat.broadcast.format");
            server.MinecraftServer.SendChat(string.Format(format, text));
        }
    }
}
