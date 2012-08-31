using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartyCraft
{
    public abstract class Command
    {
        public abstract string DefaultCommand { get; }
        public abstract string Documentation { get; set; }
        public abstract void Execute(params string[] parameters);

        public List<string> Aliases { get; set; }
        public List<string> AllowedGroups { get; set; }
    }
}
