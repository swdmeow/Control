using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;
using PlayerRoles;
using Exiled.API.Features;
using SCPSLAudioApi.AudioCore;
using Object = UnityEngine.Object;
using MEC;
using Control.API.Serialization;
using Сontrol;

namespace Control.API
{
    internal class Extensions
    {
        public static List<ReferenceHub> Dummies;
        /// <summary>Проиграть аудиофайл</summary>
        public static void PlayAudio(string audioFile, byte volume, bool loop, string eventName)
        {
            try
            {
                Dummies = new List<ReferenceHub>();
                var newPlayer = Object.Instantiate(NetworkManager.singleton.playerPrefab);
                int id = Dummies.Count;
                var fakeConnection = new FakeConnection(id++);
                var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
                Dummies.Add(hubPlayer);
                NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);

                hubPlayer.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

                try
                {
                    hubPlayer.nicknameSync.SetNick(eventName);
                }
                catch (Exception) { }

                var audioPlayer = AudioPlayerBase.Get(hubPlayer);

                var path = Path.Combine(Path.Combine(Paths.Configs, "ControlNR/Music"), audioFile);

                audioPlayer.Enqueue(path, -1);
                audioPlayer.LogDebug = false;
                audioPlayer.BroadcastChannel = VoiceChat.VoiceChatChannel.Intercom;
                audioPlayer.Volume = volume;
                audioPlayer.Loop = loop;
                audioPlayer.Play(0);
                Log.Debug($"Playing sound {path}");
            }
            catch (Exception e)
            {
                Log.Error($"Error on: {e.Data} -- {e.StackTrace}");
            }
        }
        /// <summary>Остановить прогирывание</summary>
        public static void StopAudio()
        {
            foreach (var dummies in Dummies)
            {
                var audioPlayer = AudioPlayerBase.Get(dummies);

                Timing.CallDelayed(0.1f, () => { NetworkServer.Destroy(dummies.gameObject); });
            }
            Dummies.Clear();
        }

    }
}
