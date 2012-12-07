using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PartyCraft;
using Craft.Net;
using Craft.Net.Data;
using Craft.Net.Server;
namespace PartyCraft
{
    public interface IOnLoad : IPluginSystem
    {
        void OnLoad();
    }
}
