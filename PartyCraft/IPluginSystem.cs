using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartyCraft
{
    public interface IPluginSystem : IDisposable
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>The name of the plugin</returns>
        string GetName();
    }
}
