using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Craft.Net.Data;

namespace PartyCraft.Test
{
    [TestFixture]
    public class VanillaSettingsTest
    {
        [Test]
        public void TestLoadSettings()
        {
            var settings = new VanillaSettingsProvider();
            settings.Load(File.Open("server.properties", FileMode.Open));
            Assert.AreEqual("A test server", settings.Get<string>("server.motd"));
            Assert.AreEqual(GameMode.Survival, settings.Get<GameMode>("level.gamemode"));
            Assert.AreEqual(10, settings.Get<int>("server.viewdistance"));
            Assert.AreEqual(20, settings.Get<int>("server.maxplayers"));
            Assert.IsFalse(settings.Get<bool>("server.onlinemode"));
        }
    }
}
