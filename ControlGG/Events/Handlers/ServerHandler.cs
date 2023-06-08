namespace Control.Handlers.Events
{
    using Exiled.API.Features;
    using MEC;
    using Exiled.CustomRoles.API.Features;
    using System.Linq;
    using Exiled.Events.EventArgs.Server;
    using PlayerRoles;
    using Сontrol;
    using Respawning;
    using Control.Commands;
    using Control.CustomItems;
    using UnityEngine;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Pickups;
    using Mirror;
    using System.Data;
    using System.Collections.Generic;
    using static Mono.Security.X509.X520;

    internal sealed class ServerHandler
    {
        public ServerHandler()
        {
            ServerEvent.WaitingForPlayers += OnWaitingForPlayers;
        }
        public void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingForPlayers;
        }
        private void OnWaitingForPlayers()
        {
            Log.Info($"\nWelcome, enabling ControlGG.\nVersion: {ControlGG.Singleton.Version}\nAuthor: {ControlGG.Singleton.Author}");
            try
            {
                Log.Info("Setting NextRoundAction to soft restart (sr).");
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;

                Log.Info("Setting values to default's");
                Res.StatusEffectBase.Clear();
                Res.DiedWithSCP500R.Clear();
                Res.RoleDiedWithSCP500R.Clear();
                Tesla.TeslaBoolean = false;
                Tesla.IgnoredTeams.Clear();
                Cuff.IgnoredCuffPlayers.Clear();
                GrenadeLauncher.CooldownIsEnable = false;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}