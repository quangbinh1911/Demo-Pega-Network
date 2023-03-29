using System;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Gameplay.Pega;
using UnityEngine;
using UnityEngine.UI;


namespace Gameplay
{
    public partial class GameplayController : NetworkBehaviour
    {
        public static GameplayController Singleton;


        [SerializeField] private Button startRaceBtn;


        [SyncVar(OnChange = nameof(OnStateChanged))]
        public GameState state = GameState.Unset;

        public Action<GameState, GameState, bool> onStateChanged;

        private void OnStateChanged(GameState old, GameState value, bool asServer)
        {
            onStateChanged?.Invoke(old, value, asServer);
        }


        [SyncObject] public readonly SyncDictionary<int, PlayerInfo> Players = new();
        public Action PlayersCountChanged;

        [SyncObject] public readonly SyncDictionary<int, PegaPredicted> Pegas = new();
        


        private void Awake()
        {
            Singleton = this;
            Application.runInBackground = true;
        }


        public override void OnStartServer()
        {
            base.OnStartServer();
            
            startRaceBtn.gameObject.SetActive(IsServer);


            if (!IsServer) return;

            InstanceFinder.NetworkManager.SceneManager.OnClientLoadedStartScenes += (connection, asServer) =>
            {
                if (!asServer) return;
                OnClientStart(connection);
            };

            InstanceFinder.ServerManager.OnRemoteConnectionState += (connection, stateArgs) =>
            {
                if (stateArgs.ConnectionState != RemoteConnectionState.Stopped) return;
                OnClientDisconnect(connection);
            };

            Players.OnChange += OnPlayersChange;
            Velocities.OnChange += OnVelocityChange;
            
            startRaceBtn.onClick.AddListener(() =>
            {
                startRaceBtn.gameObject.SetActive(false);
                StartSession(this.GetCancellationTokenOnDestroy()).Forget();
            });
        }

        private void OnClientStart(NetworkConnection connection)
        {
            var connectionID = connection.ClientId;
            Debug.Log($"Add player: {connectionID}");

            var player = new PlayerInfo
            {
                ConnectionId = connectionID,
                AvatarID = connectionID,
                Name = Guid.NewGuid().ToString(),
            };

            Players.Add(connectionID, player);


            if (Pegas.ContainsKey(connectionID) && Pegas[connectionID] != null)
            {
                Pegas[connectionID].SetOwnerShip(connection);
            }
        }

        private void OnClientDisconnect(NetworkConnection connection)
        {
            var connectionID = connection.ClientId;
            Debug.Log($"Remove player: {connectionID}");


            if (Pegas.ContainsKey(connectionID) && Pegas[connectionID] != null)
            {
                Pegas[connectionID].SetOwnerShip(null);
            }

            if (Players.ContainsKey(connectionID))
            {
                Players.Remove(connectionID);
            }
        }
        
        
        private void OnPlayersChange(SyncDictionaryOperation op, int key, PlayerInfo value, bool asServer)
        {
            InvokePlayersCountChanged();
        }
        
        [ObserversRpc]
        private void InvokePlayersCountChanged()
        {
            PlayersCountChanged?.Invoke();
        }
    }

    public enum GameState
    {
        Unset = -1,
        Waiting,
        Playing,
        Finished,
    }

    public struct PlayerInfo
    {
        public int ConnectionId;
        public int AvatarID;
        public string Name;
    }
}