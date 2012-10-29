using System;
using System.Collections;
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
            Set("chat.private.format", ChatColors.Gray + "<{0}->{1}> {2}");
            Set("chat.self.format", "* {0} {1}");
            Set("chat.join", ChatColors.Yellow + "{0} has joined the game.");
            Set("chat.leave", ChatColors.Yellow + "{0} has left the game.");
        }

        public static void Set(string key, object value)
        {
            key = key.ToLower();
            if (value != null && value.GetType().GetInterface("IConvertible") == null)
            {
                if (value.GetType().GetInterface("IEnumerable") != null)
                {
                    int i = 0;
                    foreach (var item in (IEnumerable)value)
                        Set(key + "[" + i++ + "]", item);
                }
                else
                {
                    // TODO: Serialize
                    throw new InvalidCastException("Objects must implement IConvertible or IEnumerable.");
                }
            }
            else
                Settings[key] = (string)Convert.ChangeType(value, typeof(string));
        }

        public static T Get<T>(string key)
        {
            key = key.ToLower();
            if (!ContainsKey(key))
                throw new KeyNotFoundException();
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), Settings[key], true);
            if (typeof(T).GetInterface("IConvertible") == null)
            {
                if (typeof(T).GetInterface("IEnumerable") != null)
                {
                    var items = Settings.Where(p => p.Key.StartsWith(key + "["));
                    Type genericType = typeof(T).GetGenericArguments()[0];
                    var results = Array.CreateInstance(genericType, items.Count());
                    int i = 0;
                    foreach (var item in items)
                    {
                        int start = key.Length + 1;
                        int length = item.Key.IndexOf(']', start + 1) - start;
                        int index = int.Parse(item.Key.Substring(start, length));
                        results.SetValue(typeof(DefaultSettings).GetMethod("Get").MakeGenericMethod(genericType)
                            .Invoke(null, new object[] { key + "[" + index + "]" }), index);
                    }
                    var listType = typeof(List<>).MakeGenericType(genericType);
                    if (typeof(T) == listType)
                        return (T)Activator.CreateInstance(listType, results);
                    return (T)results.Cast<T>();
                }
            }
            return (T)Convert.ChangeType(Settings[key], typeof(T));
        }

        public static bool ContainsKey(string key)
        {
            key = key.ToLower();
            foreach (var item in Settings)
            {
                if (item.Key == key)
                    return true;
                if (item.Key.StartsWith(key + "["))
                    return true;
            }
            return false;
        }
    }
}
