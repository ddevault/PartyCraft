using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginSystemCS
{
    /// <summary>
    /// The interface for a logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Begins the sub.
        /// </summary>
        void BeginSubLog();
        /// <summary>
        /// Ends the sub level.
        /// </summary>
        void EndSubLog();

        /// <summary>
        /// Gets the sub log depth.
        /// </summary>
        /// <returns></returns>
        int GetSubLogDepth();
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Log(String message);
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        void Log(String message, params object[] parameters);
    }
}
