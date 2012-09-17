using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;

namespace PartyCraft
{
    /// <summary>
    /// Provides the default settings for a server.
    /// </summary>
    public static class DefaultSettings
    {
        private static Dictionary<string, string> Settings;

        static DefaultSettings()
        {
            Settings = new Dictionary<string, string>();
            Set("server.port", "25565");
            Set("level.type", Level.DefaultGenerator.GeneratorName);
            Set("level.name", "world");
            Set("server.motd", "PartyCraft Server");
            Set("server.rcon", false);
            Set("server.buildmax", 256);
            Set("server.mobs.villagers.enabled", true);
            Set("server.mobs.friendly.enabled", true);
            Set("server.mobs.hostile.enabled", true);
            Set("server.texturepack", null);
            Set("server.onlinemode", true);
            Set("server.pvp.enabled", true);
            Set("level.gamemode", GameMode.Survival);
            Set("server.maxplayers", 25);
            Set("chat.format", "<{0}> {1}");
            Set("chat.join", ChatColors.Yellow + "{0} has joined the game.");
            Set("chat.leave", ChatColors.Yellow + "{0} has left the game.");
        }

        public static void Set(string key, object value)
        {
            key = key.ToLower();
            Settings[key] = (string)Convert.ChangeType(value, typeof(string));
        }

        public static T Get<T>(string key)
        {
            key = key.ToLower();
            if (!Settings.ContainsKey(key))
                throw new KeyNotFoundException();
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), Settings[key], true);
            return (T)Convert.ChangeType(Settings[key], typeof(T));
        }

        public static bool ContainsKey(string key)
        {
            key = key.ToLower();
            return Settings.ContainsKey(key);
        }
    }
}
