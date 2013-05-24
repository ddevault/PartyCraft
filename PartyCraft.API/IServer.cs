using Craft.Net.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyCraft.API
{
    public interface IServer
    {
        MinecraftServer MinecraftServer { get; }
        ISettingsProvider SettingsProvider { get; }
    }
}
