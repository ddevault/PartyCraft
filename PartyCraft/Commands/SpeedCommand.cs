using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Common;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class SpeedCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "speed"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Sets the movement speed for a given player.\n" +
                    "Usage: /speed " + ChatColors.Italic + "[player] speed\n" +
                    "If no player is specified, your own speed is set.\n" +
                    "Use /speed reset to reset your speed to default.\n" +
                    "Example: /speed notch 100";
            }
        }

        public override void Execute(Server server, RemoteClient user, string text, params string[] parameters)
        {
            if (parameters.Length > 2 || parameters.Length == 0)
            {
                user.SendChat(ChatColors.Red + "Incorrect usage. Use /help speed for more information.");
                return;
            }
            if (parameters[0].ToLower() == "reset")
            {
                user.Entity.Abilities.WalkingSpeed = 12;
                user.Entity.Abilities.FlyingSpeed = 24;
                return;
            }
            string player = user.Username;
            int speed;
            if (parameters.Length == 2)
            {
                player = parameters[0];
                if (!int.TryParse(parameters[1], out speed))
                {
                    user.SendChat(ChatColors.Red + "Incorrect usage. Use /help speed for more information.");
                    return;
                }
            }
            else
            {
                if (!int.TryParse(parameters[0], out speed))
                {
                    user.SendChat(ChatColors.Red + "Incorrect usage. Use /help speed for more information.");
                    return;
                }
            }
            var client = server.MinecraftServer.GetClient(player);
            if (client == null)
            {
                user.SendChat(ChatColors.Red + player + " is not online.");
                return;
            }
            client.Entity.Abilities.WalkingSpeed = (byte)speed;
            client.Entity.Abilities.FlyingSpeed = (byte)(speed * 2);
        }
    }
}
