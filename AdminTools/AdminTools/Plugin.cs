using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using UnityEngine;

namespace AdminTools
{
	public class Plugin : Plugin<Config>
	{
		public override string Author { get; } = "Originally by Joker119. Modifications by KoukoCocoa & Thomasjosif";
		public override string Name { get; } = "Admin Tools";
		public override string Prefix { get; } = "AT";
		public override Version RequiredExiledVersion { get; } = new(6, 0, 0);

		public EventHandlers EventHandlers;
		public static System.Random NumGen = new();
		public static List<Jailed> JailedPlayers = new();
		public static Dictionary<Player, InstantKillComponent> IkHubs = new();
		public static Dictionary<Player, RegenerationComponent> RgnHubs = new();
		public static HashSet<Player> PryGateHubs = new();
		public static Dictionary<Player, List<GameObject>> BchHubs = new();
		public static Dictionary<Player, List<GameObject>> DumHubs = new();
		public static float HealthGain = 5;
		public static float HealthInterval = 1;
		public string OverwatchFilePath;
		public string HiddenTagsFilePath;
		public static bool RestartOnEnd = false;
		public static HashSet<Player> RoundStartMutes = new();

		public override void OnEnabled()
		{
			try
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string pluginPath = Path.Combine(appData, "Plugins");
				string path = Path.Combine(Paths.Plugins, "AdminTools");
				string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");
				string hiddenTagFileName = Path.Combine(path, "AdminTools-HiddenTags.txt");

				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				if (!File.Exists(overwatchFileName))
					File.Create(overwatchFileName).Close();

				if (!File.Exists(hiddenTagFileName))
					File.Create(hiddenTagFileName).Close();

				OverwatchFilePath = overwatchFileName;
				HiddenTagsFilePath = hiddenTagFileName;

				EventHandlers = new EventHandlers(this);
				Handlers.Player.Verified += EventHandlers.OnPlayerVerified;
				Handlers.Server.RoundEnded += EventHandlers.OnRoundEnd;
				Handlers.Player.TriggeringTesla += EventHandlers.OnTriggerTesla;
				Handlers.Player.ChangingRole += EventHandlers.OnSetClass;
				Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
				Handlers.Player.InteractingDoor += EventHandlers.OnDoorOpen;
				Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
				Handlers.Player.Destroying += EventHandlers.OnPlayerDestroyed;
				Handlers.Player.InteractingDoor += EventHandlers.OnPlayerInteractingDoor;

                Handlers.Scp096.CalmingDown += EventHandlers.Enrage; // 1
                Handlers.Player.Dying += EventHandlers.Died; // 1




            }
            catch (Exception e)
			{
				Log.Error($"Loading error: {e}");
			}
		}

		public override void OnDisabled()
		{
            
			Handlers.Player.InteractingDoor -= EventHandlers.OnDoorOpen;
			Handlers.Player.Verified -= EventHandlers.OnPlayerVerified;
			Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnd;
			Handlers.Player.TriggeringTesla -= EventHandlers.OnTriggerTesla;
			Handlers.Player.ChangingRole -= EventHandlers.OnSetClass;
			Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
			Handlers.Player.Destroying -= EventHandlers.OnPlayerDestroyed;
			EventHandlers = null;
			NumGen = null;
		}

		public override void OnReloaded() { }
	}
}