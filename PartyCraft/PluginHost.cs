using System;

namespace PartyCraft
{

    /// <summary>
    /// The plugin hoster
    /// </summary>
    internal class PluginHost
    {
        private PluginCore pluginCore;
        private IPluginSystem plugin;
        private AppDomain pluginDomain;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginHost"/> class.
        /// </summary>
        /// <param name="pluginCore">The plugin core.</param>
        /// <param name="plugin">The plugin.</param>
        /// <param name="pluginDomain">The plugin domain.</param>
        public PluginHost(PluginCore pluginCore, IPluginSystem plugin, AppDomain pluginDomain)
        {
            this.pluginCore = pluginCore;
            this.plugin = plugin;
            this.pluginDomain = pluginDomain;
        }


        /// <summary>
        /// Gets or sets the plugin.
        /// </summary>
        /// <value>
        /// The plugin.
        /// </value>
        public IPluginSystem Plugin
        {
            get
            {
                return this.plugin;
            }
        }

        /// <summary>
        /// Unloads this instance.
        /// </summary>
        internal void Unload()
        {
            this.plugin.Dispose();
        }
    }
}
