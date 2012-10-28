using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Craft.Net.Server;

namespace PartyCraft.Commands
{
    public class StopCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "stop"; }
        }

        public override string Documentation
        {
            get
            {
                return "Stops the server.\n" +
                    "Usage: /stop";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            Process.GetCurrentProcess().Kill(); // TODO: Friendly kick message
        }
    }
}
