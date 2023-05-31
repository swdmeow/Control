namespace XPSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;
    using LiteDB;
    using MEC;
    using XPSystem.API;
    using YamlDotNet.Serialization;
    using Player = Exiled.Events.Handlers.Player;
    using Scp914 = Exiled.Events.Handlers.Scp914;
    using Server = Exiled.Events.Handlers.Server;

    public class Main : Plugin<Config>
    {
        public override string Author { get; } = "swd";
        public override string Name { get; } = "XPSystem";
        public override Version Version { get; } = new Version(1, 7, 4);

        public override PluginPriority Priority => PluginPriority.High;
        public static Main Instance { get; set; }
        public EventHandlers handlers;
        private Harmony _harmony;
        public LiteDatabase db;
        
        public Dictionary<string, string> Translations = new Dictionary<string, string>()
        {
            ["ExampleKey"] = "ExampleValue",
            ["ExampleKey2"] = "ExampleValue",
        };

        public override void OnEnabled()
        {
            if(!Directory.Exists(Path.Combine(Paths.Configs, "XPSystem")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.Configs, "XPSystem"));
            }

            db = new LiteDatabase(Path.Combine(Paths.Configs, "XPSystem/Players.db"));
            handlers = new EventHandlers();
            Instance = this;
            _harmony = new Harmony($"XPSystem - {DateTime.Now.Ticks}");
            
            Player.Verified += handlers.OnJoined;
            Player.Dying += handlers.OnKill;
            Server.RoundEnded += handlers.OnRoundEnd;
            Player.Escaping += handlers.OnEscape;
            Server.ReloadedRA += handlers.ReloadedRA;


            LoadTranslations();

            _harmony.PatchAll();
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.Verified -= handlers.OnJoined;
            Player.Dying -= handlers.OnKill;
            Server.RoundEnded -= handlers.OnRoundEnd;
            Player.Escaping -= handlers.OnEscape;
            Server.ReloadedRA -= handlers.ReloadedRA;

            _harmony.UnpatchAll(_harmony.Id);
            
            handlers = null;
            Instance = null;
            _harmony = null;
            db.Dispose();
            db = null;
            
            base.OnDisabled();
        }

        public static string GetTranslation(string key)
        {
            if (Instance.Config.Debug) // 1
            {
                Log.Debug("looking for key: " + key);
                Log.Debug($"Found key: {Instance.Translations.ContainsKey(key)}");
            }
            return Instance.Translations.TryGetValue(key, out var translation) ? translation : null;
        }

        private void LoadTranslations()
        {
            try
            {
                var serializer = new Serializer();
                var deserializer = new Deserializer();
                if (!File.Exists(Config.SavePathTranslations))
                {
                    File.Create(Config.SavePathTranslations).Close();
                    using (TextWriter sr = new StreamWriter(Config.SavePathTranslations))
                    {
                        sr.Write(serializer.Serialize(Translations));
                    }

                    return;
                }
                
                using (TextReader sr = new StreamReader(Config.SavePathTranslations))
                {
                    Translations = deserializer.Deserialize<Dictionary<string, string>>(sr.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not load translations: " + e);
            }
        }
    }
}