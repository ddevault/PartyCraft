using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Common;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class OpCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "op"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Adds the specified player to the server.op group.\n" +
                    "Usage: /op " + ChatColors.Italic + "player\n" +
                    "Example: /op notch";
            }
        }

        public override void Execute(Server server, RemoteClient user, string text, params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                user.SendChat(ChatColors.Red + "Invalid parameters. Use /help op for more information.");
                return;
            }
            var groups = server.GetUserGroups(parameters[0]);
            if (groups.Contains("server.op"))
            {
                user.SendChat(ChatColors.Red + "User is already an op.");
                return;
            }
            groups.Add("server.op");
            server.SetUserGroups(parameters[0], groups);
            server.SendChatToGroup("server.op", ChatColors.Gray + user.Username + " adds " + parameters[0] + " to server.op group.");
        }
    }
}
