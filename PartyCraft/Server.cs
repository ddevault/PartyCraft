using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Data.Generation;
using Craft.Net.Server;
using System.Net;
using Craft.Net.Server.Events;

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
            MinecraftServer.ChatMessage += MinecraftServerOnChatMessage;
            MinecraftServer.PlayerLoggedIn += MinecraftServerOnPlayerLoggedIn;
            MinecraftServer.PlayerLoggedOut += MinecraftServerOnPlayerLoggedOut;
        }

        public void Start()
        {
            MinecraftServer.AddLevel(new Level(Level.GetGenerator(SettingsProvider.Get<string>("level.type")), 
                SettingsProvider.Get<string>("level.name")));
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
                groups.AddRange(SettingsProvider.Get<List<string>>(user + "groups"));
            return groups;
        }

        #region Event Handlers

        private void MinecraftServerOnChatMessage(object sender, ChatMessageEventArgs chatMessageEventArgs)
        {
            chatMessageEventArgs.Handled = true;
            if (chatMessageEventArgs.RawMessage.StartsWith("/"))
                Command.ExecuteCommand(this, chatMessageEventArgs.Origin, chatMessageEventArgs.RawMessage);
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
