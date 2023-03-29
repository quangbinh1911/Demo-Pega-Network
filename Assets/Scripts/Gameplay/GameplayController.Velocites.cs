using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace Gameplay
{
    public partial class GameplayController
    {
        [SyncObject(SendRate = 0.5f)] private readonly SyncDictionary<int, float> Velocities = new();
        public Action<VelocityChanged> VelocityChanged;


        [ServerRpc(RequireOwnership = false)]
        public void SetVelocity(int connection, float velocity)
        {
            if (!Velocities.ContainsKey(connection))
            {
                Velocities.Add(connection, velocity);
            }
            else
            {
                Velocities[connection] = velocity;
            }
        }

        public float GetVelocity(int connection)
        {
            return !Velocities.ContainsKey(connection) ? default : Velocities[connection];
        }
        
        private void OnVelocityChange(SyncDictionaryOperation op, int key, float value, bool asServer)
        {
            if (op == SyncDictionaryOperation.Set)
            {
                var changed = new VelocityChanged
                {
                    ConnectionID = key,
                    Velocity = value
                };

                InvokeVelocityChanged(changed);
            }
        }

        [ObserversRpc]
        private void InvokeVelocityChanged(VelocityChanged changed)
        {
            VelocityChanged?.Invoke(changed);
        }
    }

    public struct VelocityChanged
    {
        public int ConnectionID;
        public float Velocity;
    }
}