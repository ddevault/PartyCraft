using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyCraft.API
{
    public abstract class Plugin
    {
        public virtual void OnInstall() { }
        public virtual void OnInitialize(IServer server) { }
        public virtual void OnShutDown() { }
    }
}
