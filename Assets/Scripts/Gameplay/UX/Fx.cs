using FishNet.Object;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gameplay.UX
{
    public class Fx : NetworkBehaviour
    {
        public static Fx Instance;
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            Instance = this;
        }
        

        [ServerRpc(RequireOwnership = false)]
        public void Spawn(string path, Vector3 position, Quaternion rotation)
        {
            Spawn_Client(path, position, rotation);
        }

        [ObserversRpc]
        private void Spawn_Client(string path, Vector3 position, Quaternion rotation)
        {
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null) return;

            Object.Instantiate(prefab, position, rotation);
        }
    }
}