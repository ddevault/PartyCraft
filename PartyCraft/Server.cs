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
            var port = SettingsProvider.Get<int>("Server.Port");
            MinecraftServer = new MinecraftServer(new IPEndPoint(IPAddress.Any, port));
            MinecraftServer.ChatMessage += MinecraftServerOnChatMessage;
            MinecraftServer.PlayerLoggedIn += MinecraftServerOnPlayerLoggedIn;
            MinecraftServer.PlayerLoggedOut += MinecraftServerOnPlayerLoggedOut;
        }

        public void Start()
        {
            MinecraftServer.AddLevel(new Level(Level.GetGenerator(SettingsProvider.Get<string>("Level.Type")), 
                SettingsProvider.Get<string>("Level.Name")));
            MinecraftServer.Start();
        }

        public void Stop()
        {
            MinecraftServer.Stop();
        }

        #region Event Handlers

        private void MinecraftServerOnChatMessage(object sender, ChatMessageEventArgs chatMessageEventArgs)
        {
            chatMessageEventArgs.Handled = true;
            MinecraftServer.SendChat(string.Format(SettingsProvider.Get<string>("chat.format"),
                chatMessageEventArgs.Origin.Username, chatMessageEventArgs.RawMessage));
        }

        private void MinecraftServerOnPlayerLoggedIn(object sender, PlayerLogInEventArgs playerLogInEventArgs)
        {
            playerLogInEventArgs.Handled = true;
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
