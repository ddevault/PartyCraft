using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using Craft.Net.Data;

namespace PartyCraft
{
    /// <summary>
    /// Provides an means of working with a server.properties file
    /// through the ISettingsProvider interface.
    /// </summary>
    public class VanillaSettingsProvider : ISettingsProvider
    {
        public Dictionary<string, string> ServerProperties;

        public VanillaSettingsProvider()
        {
            ServerProperties = new Dictionary<string, string>();
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
