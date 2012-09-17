using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using Craft.Net.Data;
using System.IO;

namespace PartyCraft
{
    /// <summary>
    /// Provides an means of working with a server.properties file
    /// through the ISettingsProvider interface.
    /// </summary>
    public class VanillaSettingsProvider : ISettingsProvider
    {
        public Dictionary<string, string> ServerProperties;
        private Dictionary<string, string> keyLookup;

        public VanillaSettingsProvider()
        {
            ServerProperties = new Dictionary<string, string>();
            keyLookup = new Dictionary<string, string>();
            keyLookup.Add("server.port", "server-port");
            keyLookup.Add("level.type", "level-type");
            keyLookup.Add("server.rcon", "enable-rcon");
            keyLookup.Add("level.seed", "level-seed");
            keyLookup.Add("server.buildmax", "max-build-height");
            keyLookup.Add("server.mobs.villagers.enabled", "spawn-npcs");
            keyLookup.Add("server.whitelist.enabled", "white-list");
            keyLookup.Add("server.mobs.friendly.enabled", "spawn-animals");
            keyLookup.Add("server.snoop.enabled", "snooper-enabled");
            keyLookup.Add("server.hardcore.enabled", "hardcore");
            keyLookup.Add("server.texturepack", "texture-pack");
            keyLookup.Add("server.onlinemode", "online-mode");
            keyLookup.Add("server.pvp.enabled", "pvp");
            keyLookup.Add("level.gamemode", "gamemode");
            keyLookup.Add("server.maxplayers", "max-players");
            keyLookup.Add("server.mobs.hostile.enabled", "spawn-monsters");
            keyLookup.Add("level.generator.generatestructures", "generate-structures");
            keyLookup.Add("server.viewdistance", "view-distance");
            keyLookup.Add("server.motd", "motd");
        }

        public void Load(Stream file)
        {
            StreamReader reader = new StreamReader(file);
            var lines = reader.ReadToEnd().Replace("\r", "").Split('\n');
            reader.Close();
            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index].Trim();
                if (!line.StartsWith("#") && line.Contains("="))
                {
                    string key = line.Remove(line.IndexOf('='));
                    string value = line.Substring(line.IndexOf('=') + 1);
                    if (keyLookup.ContainsValue(key))
                        Set(keyLookup.First(kvp => kvp.Value == key).Key, value);
                    else
                        Set(key, value);
                }
            }
        }

        public void Set(string key, object value)
        {
            ServerProperties[key] = (string)Convert.ChangeType(value, typeof(string));
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(key));
            Save();
        }

        public T Get<T>(string key)
        {
            if (!ServerProperties.ContainsKey(key))
                return DefaultSettings.Get<T>(key);
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), ServerProperties[key], true);
            return (T)Convert.ChangeType(ServerProperties[key], typeof(T));
        }

        public bool ContainsKey(string key)
        {
            return ServerProperties.ContainsKey(key);
        }

        private void Save()
        {
            // TODO
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
