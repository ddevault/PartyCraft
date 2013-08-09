using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using Craft.Net.Common;
using System.IO;
using PartyCraft.API;

namespace PartyCraft
{
    /// <summary>
    /// Provides an means of working with a vanilla server.properties file through the ISettingsProvider interface.
    /// </summary>
    public class VanillaSettingsProvider : ISettingsProvider
    {
        public Dictionary<string, string> ServerProperties;
        private Dictionary<string, string> keyLookup;
        private string settingsFile;

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
            foreach (var key in keyLookup) // Set default keys
            {
                if (DefaultSettings.ContainsKey(key.Key))
                    Set(key.Key, Get<string>(key.Key)); // This works because it goes to the default setting provider when it can't find a key
            }
        }

        public VanillaSettingsProvider(string file) : this()
        {
            settingsFile = file;
            if (File.Exists(file))
                Load(File.Open(file, FileMode.Open));
            else
                Save();
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
            if (value == null)
            {
                Remove(key);
                return;
            }
            if (value.GetType().GetInterface("IConvertible") == null)
            {
                if (value.GetType().GetInterface("IEnumerable") != null)
                {
                    List<string> keysToRemove = new List<string>();
                    foreach (var pair in ServerProperties)
                    {
                        if (pair.Key.StartsWith(key + "["))
                            keysToRemove.Add(pair.Key);
                    }
                    foreach (var item in keysToRemove)
                        Remove(item);
                    int i = 0;
                    foreach (var item in (IEnumerable)value)
                        Set(key + "[" + i++ + "]", item); // TODO: key+=value, instead of key[i]=value
                }
                else
                {
                    // TODO: Serialize
                    throw new InvalidCastException("Objects must implement IConvertible or IEnumerable.");
                }
            }
            else
                ServerProperties[key] = (string)Convert.ChangeType(value, typeof(string));
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(key));
            Save();
        }

        public void Remove(string key)
        {
            if (ServerProperties.ContainsKey(key))
                ServerProperties.Remove(key);
        }

        public T Get<T>(string key)
        {
            // TODO: Can this be made better?
            if (!ContainsKeyNoRecurse(key))
                return DefaultSettings.Get<T>(key);
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), ServerProperties[key], true);
            if (typeof(T).GetInterface("IConvertible") == null)
            {
                if (typeof (T).GetInterface("IEnumerable") != null)
                {
                    var items = ServerProperties.Where(p => p.Key.StartsWith(key + "["));
                    Type genericType = typeof(T).GetGenericArguments()[0];
                    var results = Array.CreateInstance(genericType, items.Count());
                    int i = 0;
                    foreach (var item in items)
                    {
                        int start = key.Length + 1;
                        int length = item.Key.IndexOf(']', start + 1) - start;
                        int index = int.Parse(item.Key.Substring(start, length));
                        results.SetValue(typeof (VanillaSettingsProvider).GetMethod("Get").MakeGenericMethod(genericType)
                            .Invoke(this, new object[] { key + "[" + index + "]" }), index);
                    }
                    var listType = typeof(List<>).MakeGenericType(genericType);
                    if (typeof(T) == listType)
                        return (T)Activator.CreateInstance(listType, results);
                    return (T)results.Cast<T>();
                }
            }
            return (T)Convert.ChangeType(ServerProperties[key], typeof(T));
        }

        private bool ContainsKeyNoRecurse(string key)
        {
            foreach (var item in ServerProperties)
            {
                if (item.Key == key)
                    return true;
                if (item.Key.StartsWith(key + "["))
                    return true;
            }
            return false;
        }

        public bool ContainsKey(string key)
        {
            bool local = ContainsKeyNoRecurse(key);
            if (local)
                return true;
            return DefaultSettings.ContainsKey(key);
        }

        private void Save()
        {
            if (settingsFile == null)
                return;
            var writer = new StreamWriter(settingsFile);
            writer.WriteLine("# PartyCraft server properties");
            writer.WriteLine("# Last modified " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            foreach (var kvp in ServerProperties)
            {
                if (keyLookup.ContainsKey(kvp.Key))
                    writer.WriteLine(keyLookup[kvp.Key] + "=" + kvp.Value);
                else
                    writer.WriteLine(kvp.Key + "=" + kvp.Value);
            }
            writer.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
