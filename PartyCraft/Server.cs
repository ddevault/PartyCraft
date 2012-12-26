using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Data.Generation;
using Craft.Net.Server;
using System.Net;
using Craft.Net.Server.Events;
using Craft.Net;

namespace PartyCraft
{
    public class Server
    {
        public MinecraftServer MinecraftServer { get; set; }
        public ISettingsProvider SettingsProvider { get; set; }

        public Server(ISettingsProvider settingsProvider)
        {
            SettingsProvider = settingsProvider;
            var port = SettingsProvider.Get<int>("server.port");
            MinecraftServer = new MinecraftServer(new IPEndPoint(IPAddress.Any, port));
            MinecraftServer.Settings.MotD = SettingsProvider.Get<string>("server.motd");
            MinecraftServer.Settings.OnlineMode = SettingsProvider.Get<bool>("server.onlinemode");
            MinecraftServer.ChatMessage += MinecraftServerOnChatMessage;
            MinecraftServer.PlayerLoggedIn += MinecraftServerOnPlayerLoggedIn;
            MinecraftServer.PlayerLoggedOut += MinecraftServerOnPlayerLoggedOut;
        }

        public void Start()
        {
            MinecraftServer.AddLevel(new Level(Level.GetGenerator(SettingsProvider.Get<string>("level.type")), 
                SettingsProvider.Get<string>("level.name")));
            MinecraftServer.DefaultLevel.GameMode = SettingsProvider.Get<GameMode>("level.gamemode");
            MinecraftServer.Start();
        }

        public void Stop()
        {
            MinecraftServer.Stop();
        }

        public List<string> GetUserGroups(string user)
        {
            var groups = new List<string>(new[] { "server.default" });
            if (SettingsProvider.ContainsKey(user + ".groups"))
                groups.AddRange(SettingsProvider.Get<List<string>>(user + ".groups"));
            return groups;
        }

        public void SetUserGroups(string user, List<string> groups)
        {
            if (groups.Contains("server.default"))
                groups.Remove("server.default");
            SettingsProvider.Set(user + ".groups", groups);
        }

        /// <summary>
        /// Sends a chat message to all members of a specified permission group.
        /// </summary>
        public void SendChatToGroup(string group, string text)
        {
            foreach (var client in MinecraftServer.Clients)
            {
                if (GetUserGroups(client.Username).Contains(group))
                    client.SendChat(text);
            }
        }

        #region Event Handlers

        private void MinecraftServerOnChatMessage(object sender, ChatMessageEventArgs chatMessageEventArgs)
        {
            chatMessageEventArgs.Handled = true;
            if (chatMessageEventArgs.RawMessage.StartsWith("/"))
            {
                if (chatMessageEventArgs.RawMessage.StartsWith("//"))
                {
                    MinecraftServer.SendChat(string.Format(SettingsProvider.Get<string>("chat.format"),
                    chatMessageEventArgs.Origin.Username, chatMessageEventArgs.RawMessage.Substring(1)));
                }
                else
                    Command.ExecuteCommand(this, chatMessageEventArgs.Origin, chatMessageEventArgs.RawMessage);
            }
            else
            {
                MinecraftServer.SendChat(string.Format(SettingsProvider.Get<string>("chat.format"),
                    chatMessageEventArgs.Origin.Username, chatMessageEventArgs.RawMessage));
            }
        }

        private void MinecraftServerOnPlayerLoggedIn(object sender, PlayerLogInEventArgs playerLogInEventArgs)
        {
            playerLogInEventArgs.Handled = true;
            playerLogInEventArgs.Client.Tags = new Dictionary<string, object>();
            playerLogInEventArgs.Client.Tags.Add("PartyCraft.UserGroups", GetUserGroups(playerLogInEventArgs.Username));
            MinecraftServer.SendChat(string.Format(SettingsProvider.Get<string>("chat.join"), playerLogInEventArgs.Username));
        }

        private void MinecraftServerOnPlayerLoggedOut(object sender, PlayerLogInEventArgs playerLogInEventArgs)
        {
            playerLogInEventArgs.Handled = true;
            MinecraftServer.SendChat(string.Format(SettingsProvider.Get<string>("chat.leave"), playerLogInEventArgs.Username));
        }

        #endregion
    }
}
