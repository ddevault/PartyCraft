using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class DeopCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "deop"; }
        }

        public override string Documentation
        {
            get
            {
                return "Removes the specified player to the server.op group.\n" +
                    "Usage: /deop " + ChatColors.Italic + "player\n" +
                    "Example: /deop notch";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                user.SendChat(ChatColors.Red + "Invalid parameters. Use /help op for more information.");
                return;
            }
            var groups = server.GetUserGroups(parameters[0]);
            if (!groups.Contains("server.op"))
            {
                user.SendChat(ChatColors.Red + "User is not an op.");
                return;
            }
            groups.Remove("server.op");
            server.SendChatToGroup("server.op", ChatColors.Gray + user.Username + " removes " + parameters[0] + " from server.op group.");
            server.SetUserGroups(parameters[0], groups);
        }
    }
}
