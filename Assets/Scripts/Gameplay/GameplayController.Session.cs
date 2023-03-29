using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Object;
using Gameplay.Pega;
using Gameplay.Stats;
using UnityEngine;

namespace Gameplay
{
    public partial class GameplayController
    {
        private async UniTaskVoid StartSession(CancellationToken ct)
        {
            state = GameState.Waiting;

            Debug.Log("Spawn Pegas");

            await UniTask.Delay(1000, cancellationToken: ct);

            var notifyWait = AddNotify.New("Start Session, wait for 5 seconds to loaded");
            InstanceFinder.ServerManager.Broadcast(notifyWait);

            foreach (var connection in NetworkManager.ServerManager.Clients)
            {
                var pegaPrefab = Resources.Load<PegaPredicted>($"Pega/Pega001"); //switch based on user's pega
                var pegaConfig = Resources.Load<PegaConfig>("PegaConfig/PegaConfig001"); //switch based on user's pega

                var position = new Vector3(-3.354303f, 0.2500011f, 56.96f);
                var rotation = Quaternion.Euler(0, -90, 0);
                var pega = Instantiate(pegaPrefab, position, rotation);

                Spawn(pega.gameObject, connection.Value);
                Pegas[connection.Key] = pega;
                pega.LoadStats(pegaConfig);
            }

            // wait for sure client connected
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: ct);
            state = GameState.Playing;
            
            InstanceFinder.ServerManager.Broadcast(notifyWait.GetRemove());
        }
    }
}