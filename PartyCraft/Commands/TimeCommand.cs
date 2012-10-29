using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Craft.Net.Data;
using Craft.Net.Server;
using Craft.Net.Server.Packets;

namespace PartyCraft.Commands
{
    public class TimeCommand : Command
    {
        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "time"; }
        }

        public override string Documentation
        {
            get
            {
                return "Sets the time of the current world.\n" +
                    "Usage: /time " + ChatColors.Italic + "set/add amount\n" +
                    "Usage: /time " + ChatColors.Italic + "day/night/noon/midnight\n" +
                    "Usage: /time " + ChatColors.Italic + "[set] hh:mm [AM/PM]\n" +
                    "Example: /time set 0, /time add 1000, /time day, /time 12:00 PM.\n" +
                    "With no parameters, /time will simply display the current time.";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            bool add = false;
            var current = server.MinecraftServer.GetLevel(user).Time;
            long? time = null;
            if (parameters.Length == 0)
            {
                user.SendChat("The current time is " + current + ", or " + LongToTimeString(current));
                return;
            }
            if (parameters[0].ToLower() == "day")
                time = 0;
            else if (parameters[0].ToLower() == "night")
                time = 12000;
            else if (parameters[0].ToLower() == "noon")
                time = 6000;
            else if (parameters[0].ToLower() == "midnight")
                time = 18000;
            else
            {
                string timeString;
                if (parameters[0].ToLower() == "set")
                    timeString = parameters[1];
                else if (parameters[0].ToLower() == "add")
                {
                    timeString = parameters[1];
                    add = true;
                }
                else
                    timeString = parameters[0];
                if (timeString.Contains(":"))
                {
                    try
                    {
                        time = TimeStringToLong(timeString);
                    }
                    catch { }
                }
                else
                {
                    long _time;
                    if (long.TryParse(timeString, out _time))
                        time = _time;
                }
                if (add)
                    time += current;
            }

            if (time == null)
            {
                user.SendChat(ChatColors.Red + "Invalid time specified.");
                return;
            }

            time = time.Value % 24000;
            
            server.MinecraftServer.GetLevel(user).Time = time.Value;
            foreach (var client in server.MinecraftServer.EntityManager.GetClientsInWorld(user.World))
                client.SendPacket(new TimeUpdatePacket(time.Value));
            server.SendChatToGroup("server.op", ChatColors.Gray + user.Username + " set the time in " + user.World.Name +
                " to " + time.Value + ", or " + LongToTimeString(time.Value));
        }

        private string LongToTimeString(long current)
        {
            current = current + 6000 % 24000;
            int hour = (int)(current / 1000);
            int minute = (int)((current % 1000 / 1000d) * 60);
            bool pm = false;
            if (hour > 12)
            {
                hour -= 12;
                pm = true;
            }
            if (hour == 0)
                hour = 12;
            return hour + ":" + minute.ToString("D2") + " " + (pm ? "PM" : "AM"); // TODO: Localization?
        }

        private long TimeStringToLong(string time)
        {
            string[] parts = time.Split(':', ' ');
            if (parts.Length > 3)
                throw new InvalidOperationException();
            int hour = int.Parse(parts[0]);
            int minute = int.Parse(parts[1]);
            bool pm = false;
            if (parts.Length == 3)
                pm = parts[2].ToLower() == "pm";
            if (hour == 12 && !pm)
                hour = 0;
            if (pm && hour < 12)
                hour += 12;
            hour %= 24;
            minute %= 60;
            long value = hour * 1000 + (long)((minute / 60d) * 1000) - 6000;
            if (value < 0)
                value += 24000;
            return value;
        }
    }
}
