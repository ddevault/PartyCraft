using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class SpawnPointCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "spawnpoint"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Sets the spawn point for the specified player.\n" +
                    "Usage: /spawnpoint " + ChatColors.Italic + "[player] [x y z]\n" +
                    "Example: /spawnpoint notch 0 60 0\n" +
                    "Uses the current player if not specified, and the current\n" +
                    "position if not specified.";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            string player = user.Username;
            int x = (int)user.Entity.Position.X;
            int y = (int)user.Entity.Position.Y;
            int z = (int)user.Entity.Position.Z;
            if (parameters.Length == 1)
                player = parameters[0];
            else if (parameters.Length == 3)
            {
                if (!int.TryParse(parameters[0], out x) &&
                    !int.TryParse(parameters[1], out y) &&
                    !int.TryParse(parameters[2], out z))
                {
                    user.SendChat(ChatColors.Red + "Incorrect usage. Use /help spawnpoint for more information.");
                    return;
                }
            }
            else if (parameters.Length == 4)
            {
                player = parameters[0];
                if (!int.TryParse(parameters[1], out x) &&
                    !int.TryParse(parameters[2], out y) &&
                    !int.TryParse(parameters[3], out z))
                {
                    user.SendChat(ChatColors.Red + "Incorrect usage. Use /help spawnpoint for more information.");
                    return;
                }
            }
            var client = server.MinecraftServer.GetClient(player);
            if (player == null)
            {
                user.SendChat(ChatColors.Red + player + " is not online."); // TODO: Set it anyway
                return;
            }
            client.Entity.SpawnPoint = new Vector3(x, y, z);
            server.SendChatToGroup("server.op", user.Username + " set " + player + " spawn point to " + client.Entity.SpawnPoint);
        }
    }
}
