using Craft.Net.Data;
using Craft.Net.Server;
using Craft.Net.Server.Events;
using Mono.CSharp;
using PartyCraft.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PartyCraft.Commands
{
    public class ReplCommand : Command
    {
        static ReplCommand()
        {
            Evaluators = new Dictionary<string, Evaluator>();
        }

        private static Server Server { get; set; }
        private static Dictionary<string, Evaluator> Evaluators { get; set; }

        protected internal override void LoadDefaultPermissions()
        {
            AllowedGroups = new List<string>(new[] { "server.op" });
        }

        public override string DefaultCommand
        {
            get { return "repl"; }
        }

        public override string Documentation
        {
            get
            {
                return "Enters an interactive C# REPL for real-time plugin development.\n" +
                    "Usage: /repl";
            }
        }

        public override void Execute(Server server, MinecraftClient user, string text, params string[] parameters)
        {
            Server = server;
            if (parameters.Length != 0)
            {
                user.SendChat(ChatColors.Red + "Invalid parameters. Use /help repl for more information.");
                return;
            }
            if (ReplContext.Self != null)
            {
                user.SendChat(ChatColors.Red + ReplContext.Self.Username + " is currently in REPL mode. Only one user may be in REPL mode at a time.");
                // TODO: Upgrade Mono.CSharp to Mono 3.0 and support several REPLs at once
                return;
            }
            server.ChatMessage -= HandleChatMessage;
            server.ChatMessage += HandleChatMessage;
            server.MinecraftServer.PlayerLoggedOut += (s, e) =>
                {
                    if (Evaluators.ContainsKey(e.Username))
                        Evaluators.Remove(e.Username);
                    ReplContext.Self = null;
                };
            Evaluators[user.Username] = new Evaluator(new CompilerContext(new CompilerSettings(), new MinecraftReportPrinter(user)));
            Evaluators[user.Username].ReferenceAssembly(typeof(Server).Assembly);
            Evaluators[user.Username].ReferenceAssembly(typeof(MinecraftServer).Assembly);
            Evaluators[user.Username].ReferenceAssembly(typeof(Craft.Net.IPacket).Assembly);
            Evaluators[user.Username].ReferenceAssembly(typeof(World).Assembly);
            Evaluators[user.Username].ReferenceAssembly(typeof(IServer).Assembly);
            Evaluators[user.Username].InteractiveBaseClass = typeof(ReplContext);
            Evaluators[user.Username].Run("using Craft.Net");
            Evaluators[user.Username].Run("using Craft.Net.Data");
            Evaluators[user.Username].Run("using Craft.Net.Data.Blocks");
            Evaluators[user.Username].Run("using Craft.Net.Data.Items");
            Evaluators[user.Username].Run("using Craft.Net.Server");
            Evaluators[user.Username].Run("using PartyCraft");
            ReplContext.Self = user;
            ReplContext.Server = server;
            user.SendChat(ChatColors.Blue + "Entering C# Interactive Mode");
            user.SendChat(ChatColors.Blue + "Use `Exit()` to exit REPL mode.");
        }

        void HandleChatMessage(object sender, ChatMessageEventArgs e)
        {
            if (!Evaluators.ContainsKey(e.Origin.Username))
                return;
            e.Handled = true;
            object result;
            bool result_set;
            if (ReplContext.LongExpression && e.RawMessage != ">>>")
            {
                ReplContext.WorkingExpression += e.RawMessage;
                e.Origin.SendChat(ChatColors.Yellow + e.RawMessage);
                return;
            }
            if (e.RawMessage == "<<<")
            {
                ReplContext.LongExpression = true;
                ReplContext.WorkingExpression = string.Empty;
                return;
            }
            if (e.RawMessage.EndsWith("\\"))
            {
                ReplContext.WorkingExpression += e.RawMessage.Remove(e.RawMessage.Length - 1);
                return;
            }
            if (!ReplContext.LongExpression)
                ReplContext.WorkingExpression += e.RawMessage;
            else
                ReplContext.LongExpression = false;
            try
            {
                Evaluators[e.Origin.Username].Evaluate(ReplContext.WorkingExpression, out result, out result_set);
                if (result_set)
                    e.Origin.SendChat(result.ToString());
            }
            catch (Exception ex)
            {
                // ...will be sent to the user by Mono.CSharp...
            }
            ReplContext.WorkingExpression = string.Empty;
        }

        class MinecraftReportPrinter : ReportPrinter
        {
            public MinecraftClient Client { get; set; }

            public MinecraftReportPrinter(MinecraftClient client)
            {
                Client = client;
            }

            public override void Print(AbstractMessage msg, bool showFullPath)
            {
                Client.SendChat(ChatColors.Red + msg.Text);
            }
        }

        public class ReplContext
        {
            public static bool LongExpression { get; set; }
            public static string WorkingExpression { get; set; }
            public static MinecraftClient Self { get; set; }
            public static Server Server { get; set; }

            public static void Exit()
            {
                Self.SendChat(ChatColors.Blue + "C# Interactive Mode disabled");
                Server.SettingsProvider.Set(Self.Username + ".repl.enabled", false);
                Evaluators.Remove(Self.Username);
                Self = null;
            }

            public static void Load(string path)
            {
                Evaluators[Self.Username].LoadAssembly(path);
            }
        }
    }
}
