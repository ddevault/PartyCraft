using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PartyCraft.API
{
    public interface ISettingsProvider : INotifyPropertyChanged
    {
        void Set(string key, object value);
        T Get<T>(string key);
        bool ContainsKey(string key);
    }
}
