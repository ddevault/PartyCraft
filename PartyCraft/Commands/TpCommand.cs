using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Common;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class TpCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "tp"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Teleports players.\n" +
                    "Usage: /tp " + ChatColors.Italic + "[player] target\n" +
                    "Usage: /tp " + ChatColors.Italic + "[player] x y z\n" +
                    "Example: /tp notch jeb_, /tp 0 60 0";
            }
        }

        public override void Execute(Server server, RemoteClient user, string text, params string[] parameters)
        {
            string player = user.Username;
            Vector3 target;
            if (parameters.Length < 1 || parameters.Length > 4)
            {
                user.SendChat(ChatColors.Red + "Incorrect usage. Use /help tp for more information.");
                return;
            }
            if (parameters.Length == 1)
            {
                var client = server.MinecraftServer.GetClient(parameters[0]);

            }
        }
    }
}
