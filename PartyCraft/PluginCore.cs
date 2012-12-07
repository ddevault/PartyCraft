using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyCraft
{
    /// <summary>
    /// All plugins are managed by the core
    /// </summary>
    public class PluginCore : IDisposable, ILogger
    {
        /// <summary>
        /// A list of plugin hosts
        /// </summary>
        private List<PluginHost> hosts = new List<PluginHost>();

        private ILogger logger = new TextLogger("log.txt");

        /// <summary>
        /// Tracks the domains in which the plugins are runned
        /// </summary>
        private List<AppDomain> pluginDomains = new List<AppDomain>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCore"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PluginCore(PluginCoreConfiguration configuration)
        {

        }

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public void LoadPlugins(string folder)
        {
            Log("Begin loading plugins");
            BeginSubLog();

            // First check if the core isn't already disposed
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException("The object has already been disposed");
            }

            // Get all Dll's in the folder, we only care about dlls although exe could also be used
            foreach (var pluginFilename in System.IO.Directory.GetFiles(folder, "*.dll"))
            {
                Log("Trying to load '{0}' as plugin", pluginFilename);

                // This is where we will store all the IPlugins available in the DLL
                List<IPluginSystem> plugins = new List<IPluginSystem>();                                        
                
                // Later we will load the plugin using pluginName which is without the DLL
                string pluginName = System.IO.Path.GetFileNameWithoutExtension(pluginFilename);
                
                // Creating the domain. Important is that we define the folder using AppDomainSetup. 
                // It's important that we load the plugins in another application domain as we should see it as
                // an extension and not part of the current application.
                System.Security.Policy.Evidence evidence = new System.Security.Policy.Evidence();
                AppDomainSetup appDomainSetup = new AppDomainSetup();
                appDomainSetup.ApplicationBase = folder;
                AppDomain pluginDomain = AppDomain.CreateDomain("Plugin: " + pluginFilename, evidence, appDomainSetup);

                // This list will contain all the class that have an IPlugin interface
                List<Type> pluginTypes = null;

                try
                {
                    // Loading the assembly
                    System.Reflection.Assembly pluginAssembly = pluginDomain.Load(pluginName);

                    // If the plugin folder is the same as that of that of executable, there is a good chance 
                    // that we find the assembly that manages the plugins. We should ignore that one.
                    if (pluginAssembly.GetTypes().Contains(typeof(PluginCore)))
                    {
                        Log("'{0}' is considered the SDK as PluginCore is detected, skipping the assembly", pluginName);
                        AppDomain.Unload(pluginDomain); // Unload domain as we are not using it
                        continue;
                    }

                    // Find all plugin
                    pluginTypes = pluginAssembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPluginSystem))).ToList();

                    // Check if the assembly had any plugins 
                    // (for example we might have an extra dll that is not a plugin in that folder)
                    if (pluginTypes.Count == 0)
                    {
                        Log("'{0}' holds no plugins, skipping the assembly", pluginName);
                        AppDomain.Unload(pluginDomain); // Unload domain as we are not using it
                        continue;
                    }
                }
                catch (System.Exception ex)
                {
                    // In case we find an error we just log it and continue. 
                    // We should add extra checks here to ensure proper cleanup
                    Log(ex.ToString());
                    continue;
                }

                Log("'{0}' was loaded as plugin, checking for items", pluginFilename);
                BeginSubLog();

                try
                {
                    // Now we need to create an instance of every class that has the IPlugin interface.
                    // Every IPlugin should have a public constructor that has one parameter namely the core.
                    // The reason for this is so that the plugin can use the core. Sadly we can't force a standard
                    // for the plugin using interface. But reflection can look it up for us if something is wrong.

                    foreach (Type pluginType in pluginTypes)
                    {
                        // Getting the constructor which has as argument the PluginCore
                        System.Reflection.ConstructorInfo pluginConstructor = pluginType.GetConstructor(new Type[] { typeof(PluginCore) });

                        // If the constructor doesn't exist, we have an IPlugin object who doesn't match the requirement. 
                        // Let's find out the reason why.
                        if (pluginConstructor == null)
                        {
                            // Check if the constructor is private (one might have forgotten the public keyword). It's 
                            // a separate check as this is a common problem. The first time I forgot I spend a good 5 
                            // minutes before I figured it out.
                            var privatePluginConstructor = pluginType.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                                                     .Where(_con => _con.GetParameters().Count() == 1).SingleOrDefault(_con => _con.GetParameters()[0].ParameterType == typeof(PluginCore));

                            if (privatePluginConstructor != null)
                            {
                                // There is a private constructor
                                throw new Exception(String.Format("The plugin '{0}' defined in '{1}.dll' has a private constructor. Please make the constructor public.", pluginType.FullName, pluginName)); 
                            }
                            else
                            {
                                // There is no private constructor (and we ruled out a public constructor). Only option is that is not correctly defined
                                throw new Exception(String.Format("The plugin '{0}' defined in '{1}.dll' has no constructor that accepts PluginCore as only parameter and can thus not be initialized. Please define the constructor that matches those requirements", pluginType.FullName, pluginName));
                            }
                        }

                        // Now lets invoke the constructor and instantiate an IPlugin
                        IPluginSystem plugin = pluginConstructor.Invoke(new Object[] { this }) as IPluginSystem;

                        // Now check if the plugin was already loaded (let's prevent duplicate plugins, plugins are already trouble enough)
                        if (hosts.Any(loadedPlugin => loadedPlugin.Plugin.GetName() == plugin.GetName()) || plugins.Any(loaded_plugin => loaded_plugin.GetName() == plugin.GetName()))
                        {
                            // The plugin already exists. Fix it.
                            throw new Exception(String.Format("The plugin named '{0}' is already loaded", pluginName));
                        }

                        // Everything went well, let's remember this plugin
                        plugins.Add(plugin);
                        Log("{0} from {1} was loaded as plugin", plugin.GetName(), pluginName);
                    }

                    // At this point all plugins from the assembly have been loaded (if one plugin failed, the entire assembly failed).
                    // Now let's store it in a host.
                    foreach (var plugin in plugins)
                    {
                        hosts.Add(new PluginHost(this, plugin, pluginDomain));
                    }

                    // Register plugin domain (to ensure it's correctly unloaded)
                    pluginDomains.Add(pluginDomain);
                }
                catch (System.Exception ex)
                {
                    // Couldn't load the entire assembly. So we will unload all previously loaded plugins
                    foreach (var plugin in plugins)
                    {
                        plugin.Dispose();
                    }
                    // Unload the domain
                    AppDomain.Unload(pluginDomain);
                    Log("Unable to load all plugins from the assembly: {0}", ex.ToString());
                }
                EndSubLog();
            }
            EndSubLog();
        }

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        /// <typeparam name="T">A plugin type</typeparam>
        /// <returns>A list of plugin types</returns>
        public List<T> GetPlugins<T>() where T : class, IPluginSystem
        {
            return hosts.Select(item => item.Plugin).OfType<T> ().ToList ();
        }

        /// <summary>
        /// Unloads the plugins.
        /// </summary>
        private void UnloadPlugins()
        {
            if (this.IsDisposed) throw new ObjectDisposedException("The object has already been disposed");
            foreach (var host in hosts)
            {
                host.Unload();
            }
            hosts.Clear();

            foreach (var domain in pluginDomains)
            {
                AppDomain.Unload(domain);
            }
            pluginDomains.Clear();
        }

        #region IDisposable implementation
        
        //TODO remember to make this class inherit from IDisposable -> PluginCore : IDisposable
        
        // Default initialization for a bool is 'false'
        private bool IsDisposed { get; set; }
        
        /// <summary>
        /// Implementation of Dispose according to .NET Framework Design Guidelines.
        /// </summary>
        /// <remarks>Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </remarks>
        public void Dispose()
        {
            Dispose( true );
        
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
        
            // Always use SuppressFinalize() in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize( this );
        }
        
        /// <summary>
        /// Overloaded Implementation of Dispose.
        /// </summary>
        /// <param name="isDisposing"></param>
        /// <remarks>
        /// <para><list type="bulleted">Dispose(bool isDisposing) executes in two distinct scenarios.
        /// <item>If <paramref name="isDisposing"/> equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.</item>
        /// <item>If <paramref name="isDisposing"/> equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.</item></list></para>
        /// </remarks>
        protected virtual void Dispose( bool isDisposing )
        {
            // TODO If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            try
            {
                if( !this.IsDisposed )
                {
                    if( isDisposing )
                    {
                        // Unload all plugins
                        UnloadPlugins();

                        // TODO Release all managed resources here
                        IDisposable diposeableLogger = logger as IDisposable;
                        if(diposeableLogger != null)
                        {
                            diposeableLogger.Dispose();
                        }
                    }
        
                    // TODO Release all unmanaged resources here
                    // TODO explicitly set root references to null to expressly tell the GarbageCollector
                    // that the resources have been disposed of and its ok to release the memory allocated for them.
                    logger = null;
                }
            }
            finally
            {
                // explicitly call the base class Dispose implementation
                this.IsDisposed = true;
            }
        }
        
        //TODO Uncomment this code if this class will contain members which are UNmanaged
        // 
        ///// <summary>Finalizer for PluginCore</summary>
        ///// <remarks>This finalizer will run only if the Dispose method does not get called.
        ///// It gives your base class the opportunity to finalize.
        ///// DO NOT provide finalizers in types derived from this class.
        ///// All code executed within a Finalizer MUST be thread-safe!</remarks>
        //  ~PluginCore()
        //  {
        //     Dispose( false );
        //  }
        #endregion IDisposable implementation


        #region ILogger Members

        /// <summary>
        /// Begins the sub.
        /// </summary>
        public void BeginSubLog()
        {
            logger.BeginSubLog();
        }

        /// <summary>
        /// Ends the sub level.
        /// </summary>
        public void EndSubLog()
        {
            logger.EndSubLog();
        }

        /// <summary>
        /// Gets the sub log depth.
        /// </summary>
        /// <returns>The depth of the current sub log</returns>
        public int GetSubLogDepth()
        {
            return logger.GetSubLogDepth();
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string message)
        {
            logger.Log(message);
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public void Log(string message, params object[] parameters)
        {
            logger.Log(message, parameters);
        }

        #endregion
    }
}
