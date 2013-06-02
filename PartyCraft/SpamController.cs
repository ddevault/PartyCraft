using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net;
using Craft.Net.Data;
using Craft.Net.Server;

namespace PartyCraft
{
    internal class SpamController
    {
        public Dictionary<string, int> numberOfMessages;
        public Dictionary<string, string> previousMessages;

        public SpamController()
        {
            numberOfMessages = new Dictionary<string, int>();
            previousMessages = new Dictionary<string, string>();
        }
        
        
        public void Init(string name)
        {

                numberOfMessages.Add(name, 0);
                previousMessages.Add(name, string.Empty);
        }

        public void Remove(string name)
        {
            numberOfMessages.Remove(name);
            previousMessages.Remove(name);
        }

        public bool CheckForSpam(string name,string message)
        {
            if (message == previousMessages[name])
            {
                    if (numberOfMessages[name] > 2)
                    {
                        return true;
                    }
                    else
                    {
                        numberOfMessages[name] += 1;
                        return false;
                    }
            }
            else
            {
                previousMessages[name] = message;
                numberOfMessages[name] = 0;
                return false;
            }
        }
    }
}
