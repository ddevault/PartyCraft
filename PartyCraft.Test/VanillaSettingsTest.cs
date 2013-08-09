using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Craft.Net.Common;
using NUnit.Framework;

namespace PartyCraft.Test
{
    [TestFixture]
    public class VanillaSettingsTest
    {
        [Test]
        public void TestLoadSettings()
        {
            var settings = new VanillaSettingsProvider("server.properties");
            //settings.Load(File.Open("server.properties", FileMode.Open));
            settings.Set("test", new List<List<string>>(new[]
                {
                    new List<string>(new[] { "hello" })
                }));
            Assert.AreEqual("A test server", settings.Get<string>("server.motd"));
            Assert.AreEqual(GameMode.Survival, settings.Get<GameMode>("level.gamemode"));
            Assert.AreEqual(10, settings.Get<int>("server.viewdistance"));
            Assert.AreEqual(20, settings.Get<int>("server.maxplayers"));
            Assert.IsFalse(settings.Get<bool>("server.onlinemode"));
            var result = settings.Get<List<List<string>>>("test");
            Assert.AreEqual("hello", result[0][0]);
        }
    }
}
