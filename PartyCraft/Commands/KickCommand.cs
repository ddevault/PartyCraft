using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Server;
using Craft.Net;

namespace PartyCraft.Commands
{
    class KickCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[]{"server.op"});
        }

        
        public override string DefaultCommand
        {
            get { return "kick"; }
        }

        public override string Documentation
        {
            get {return "Usage: /kick player\n" + "Kicks a player from the game."; }
        }

        public override void Execute(Server server, Craft.Net.Server.MinecraftClient user, string text, params string[] parameters)
        {
            if (parameters.Length == 1)
            {
                if (server.MinecraftServer.GetClient(parameters[0]) != null)
                {
                server.MinecraftServer.SendChat(parameters[0] + " was kicked by " + user.Username);
                server.MinecraftServer.GetClient(parameters[0]).Disconnect("You were kicked by " + user.Username);
                }
                else
                {
                    user.SendChat("Player is not online.");
                }
            }
            else
            {
                user.SendChat("Incorrect Usage.\n" + "Usage: /kick player");
            }
        }
    }
}
