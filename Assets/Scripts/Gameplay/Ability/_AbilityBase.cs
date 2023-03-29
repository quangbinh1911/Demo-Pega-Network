using FishNet.Object;
using Gameplay.Pega;
using UnityEngine;

namespace Gameplay.Ability
{
    public abstract class _AbilityBase : NetworkBehaviour
    {
        public PegaPredicted owner;

        
        public virtual void Cast()
        {
            Use();
        }

        [ServerRpc(RequireOwnership = false)]
        public void Use()
        {
            Debug.Log($"Use {name}");
            
            Execute();
            ReturnPool();
        }

        protected abstract void Execute();
        
        private void ReturnPool()
        {
            ServerManager.Despawn(gameObject);
        }
    }
}