using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net;
using Craft.Net.Data;
using Craft.Net.Server;

namespace PartyCraft
{
    class SpamController
    {
        public static Dictionary<string, int> sd = new Dictionary<string, int>();
        public static Dictionary<string, string> pm = new Dictionary<string, string>();

        public static void Init(string name)
        {
                sd.Add(name, 0);
                pm.Add(name, "");
        }

        public static void Remove(string name)
        {
            sd.Remove(name);
            pm.Remove(name);
        }

        public static bool CheckForSpam(string name,string message)
        {
            if (message == pm[name])
            {
                    if (sd[name] > 2)
                    {
                        return true;
                    }
                    else
                    {
                        sd[name] += 1;
                        return false;
                    }
            }
            else
            {
                pm[name] = message;
                sd[name] = 0;
                return false;
            }
        }
    }
}
