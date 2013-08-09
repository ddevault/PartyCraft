using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net;
using Craft.Net.Server;
using Craft.Net.Common;

namespace PartyCraft.Commands
{
    public class DifficultyCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "difficulty"; }
        }

        public override string Documentation
        {
            get 
            {
                return "Sets the difficulty for the current level.\n" +
                    "Usage: /difficulty " + ChatColors.Italic + "amount\n" +
                    ChatColors.Italic + "amount" + ChatColors.Plain + ChatColors.DarkCyan + " is 0-3 for peaceful-hard\n" +
                    ChatColors.Italic + "amount" + ChatColors.Plain + ChatColors.DarkCyan + " may also be text, such as \"peaceful\"\n" +
                    "Example: /difficulty 0";
            }
        }

        public override void Execute(Server server, RemoteClient user, string text, params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                user.SendChat(ChatColors.Red + "Incorrect usage. Use /help difficulty for more information.");
                return;
            }
            Difficulty value;
            int num;
            if (int.TryParse(text, out num))
            {
                if (num < 0 || num > 3)
                {
                    user.SendChat(ChatColors.Red + "Invalid difficulty specified.");
                    return;
                }
                value = (Difficulty)num;
            }
            else
            {
                if (!Enum.TryParse<Difficulty>(text, true, out value))
                {
                    user.SendChat(ChatColors.Red + "Invalid difficulty specified.");
                    return;
                }
            }
            //user.World.Difficulty = value; // TODO
            server.SendChatToGroup("server.op", ChatColors.Gray + user.Username + " sets difficulty of " + user.World.Name + " to " + value.ToString());
        }
    }
}
