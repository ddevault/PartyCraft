using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class SeedCommand : Command
    {
        public override string DefaultCommand
        {
            get { return "seed"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Displays the seed of the current world.\n" +
                    "Usage: /seed";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            user.SendChat(server.MinecraftServer.GetLevel(user.World).Seed.ToString()); // TODO: Maybe add some stuff in Craft.Net to make this less cumbersome
        }
    }
}
