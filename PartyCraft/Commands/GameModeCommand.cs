using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net;
using Craft.Net.Server;
using Craft.Net.Common;

namespace PartyCraft.Commands
{
    public class GameModeCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "gamemode"; }
        }

        public override string Documentation
        {
            get
            {
                return "Changes the game mode of a player on the server.\n" +
                    "Usage: /gamemode " + ChatColors.Italic + "mode [player]\n" +
                    "You may specify modes with the mode name (survival, creative,\n" +
                    "or adventure), or use a number, 0-2, respecitvely.\n" +
                    "Example: /gamemode survival notch";
            }
        }

        public override void Execute(Server server, RemoteClient user, string text, params string[] parameters)
        {
            string player = user.Username;
            GameMode gameMode;
            if (parameters.Length == 0 || parameters.Length > 2)
            {
                user.SendChat(ChatColors.Red + "Incorrect usage. Use /help gamemode for more information.");
                return;
            }
            int value;
            if (int.TryParse(parameters[0], out value))
            {
                if (value < 0 || value > 2)
                {
                    user.SendChat(ChatColors.Red + "Incorrect usage. Use /help gamemode for more information.");
                    return;
                }
                gameMode = (GameMode)value;
            }
            else
            {
                if (!Enum.TryParse<GameMode>(parameters[0], true, out gameMode))
                {
                    user.SendChat(ChatColors.Red + "Incorrect usage. Use /help gamemode for more information.");
                    return;
                }
            }
            if (parameters.Length == 2)
                player = parameters[1];
            var client = server.MinecraftServer.GetClient(player);
            if (client == null)
            {
                user.SendChat(ChatColors.Red + player + " is not online."); // TODO: Set it anyway
                return;
            }
            client.GameMode = gameMode;
            server.SendChatToGroup("server.op", ChatColors.Gray + user.Username + " sets " + player + " to " + gameMode + " mode.");
        }

        public override bool TabComplete(Server server, string text, out string[] matches)
        {
            int spaces = text.Count(c => c == ' ');
            var word = text;
            matches = new string[0];
            if (spaces == 0)
            {
                var modes = Enum.GetNames(typeof(GameMode));
                Array.Resize(ref modes, modes.Length * 2);
                for (int i = 0; i < modes.Length / 2; i++)
                    modes[modes.Length / 2 + i] = i.ToString();
                matches = modes.Where(m => m.StartsWith(word, StringComparison.OrdinalIgnoreCase)).ToArray();
                return matches.Any();
            }
            if (spaces == 1)
                return server.TabCompleteUsername(text, out matches);
            return false;
        }
    }
}
