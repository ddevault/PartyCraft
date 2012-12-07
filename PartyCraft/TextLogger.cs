using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginSystemCS
{
    public class TextLogger : ILogger, IDisposable
    {
        int level = 0;
        private String m_Filename;
        public String Filename { get { return m_Filename; } }
        public System.IO.TextWriter m_Writer;

        ~TextLogger()
        {
            Dispose(false);
        }
        public TextLogger(String filename)
            : this(filename, false)
        {
        }
        public TextLogger(String filename, Boolean append)
        {
            m_Filename = filename;
            m_Writer = new System.IO.StreamWriter(filename, append);
        }
        #region ILogger Members
        public void BeginSubLog()
        {
            if (isDisposed) throw new ObjectDisposedException("The object has already been disposed");
            level++;
        }

        public void EndSubLog()
        {
            if (isDisposed) throw new ObjectDisposedException("The object has already been disposed");
            level--;
            if (level < 0) level = 0;
        }

        public int GetSubLogDepth()
        {
            if (isDisposed) throw new ObjectDisposedException("The object has already been disposed");
            return level;
        }

        public void Log(string message)
        {
            if (isDisposed) throw new ObjectDisposedException("The object has already been disposed");
            String log_text = "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "]";
            int cur_level = 0;
            while (cur_level < (level + 1))
            {
                log_text += "    "; // Add 4 spaces
                cur_level++;
            }
            log_text += message;
            m_Writer.WriteLine(log_text);
            m_Writer.Flush();
        }

        public void Log(string message, params object[] parameters)
        {
            Log(String.Format(message, parameters));
        }

        #endregion

        #region IDisposable Members

        private bool isDisposed;
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to dispose the managed resources of the class
                m_Writer.Dispose();
            }
            // Code to dispose the un-managed resources of the class
            isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
