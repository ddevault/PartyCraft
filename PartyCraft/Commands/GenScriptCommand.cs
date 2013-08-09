using Craft.Net.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Common;

namespace PartyCraft.Commands
{
    public class GenScriptCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "genscript"; }
        }

        public override string Documentation
        {
            get
            {
                return "Generates an empty script file in plugins/scripts/.\n" +
                    "Usage: /genscript " + ChatColors.Italic + "script_file.csx\n" +
                    "Example: /genscript example.csx";
            }
        }

        public override void Execute(Server server, RemoteClient user, string text, params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                user.SendChat(ChatColors.Red + "Invalid parameters. Use /help op for more information.");
                return;
            }
            // TODO
        }
    }
}
